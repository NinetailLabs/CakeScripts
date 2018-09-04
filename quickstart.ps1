Param([string]$repo);
Param([bool]$initProject);

if(!$repo)
{
    Write-Error "Repo name is required";
    return;
}

# Initialize a new repository
Write-Host "Initializing Git repo...";
git init $repo;
cd $repo;

# Add VisualStudio .gitignore from GitHub
Write-Host "Grabbing VisualStudio .gitignore...";
Invoke-WebRequest https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -OutFile .gitignore;

# Add Cake bootstrap script
Write-Host "Grabbing Cake bootstrap script...";
Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1

# Add CakeScripts as a Submodule
Write-Host "Adding CakeScripts as Git submodule...";
git submodule add -b master https://github.com/NinetailLabs/CakeScripts.git;

# Copy template.build.cake to root
Write-Host "Copying build.cake to repo root...";
Copy-Item -Path ".\CakeScripts\templates\template.build.cake" -Destination "build.cake";

if($initProject)
{
    Write-Host "Initializing DotNetCore library";
    Param([string]$repo);

    # Create solution file
    dotnet new sln

    # Copy the NineTailLabs ReSharper setting for file layouts
    Copy-Item -Path ".\CakeScripts\visualstudio\NineTailLabs.DotSettings" -Destination "NineTailLabs.DotSettings";

    # Create the ClassLibrary project for NetStandard2.0 and add it to the solution
    dotnet new classlib -f netstandard2.0 -o $repo
    dotnet sln "$repo.sln" add ".\$repo\$repo.csproj"

    # Add base Nuget packages
    dotnet add ".\$repo\$repo.csproj" package SemVer.Git.MSBuild
    dotnet add ".\$repo\$repo.csproj" package Microsoft.SourceLink.GitHub -v 1.0.0-beta-63127-02

    # Copy empty nuspec file to the project directory
    Copy-Item -Path ".\CakeScripts\visualstudio\Empty.nuspec" -Destination ".\$repo\$repo.nuspec";
    # Copt the SemVer.Git.MSBuild .props file we use as the one from the pacakge is not copied (and details are different)
    Copy-Item -Path ".\CakeScripts\visualstudio\SemVer.MSBuild.props" -Destination ".\$repo\SemVer.MSBuild.props";

    # Create the test project and add it to the solution
    dotnet new classlib -o "$repo.Tests"
    dotnet sln "$repo.sln" add ".\$repo.Tests\$repo.Tests.csproj"

    # Add test Nuget packages
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package Microsoft.NET.Test.Sdk
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package xunit
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package xunit.runner.visualstudio
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package FluentAssertions
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package Moq

    # Do package restore
    dotnet restore

    git tag 1.0.0.0
}

Write-Host "Repository quickstart done";