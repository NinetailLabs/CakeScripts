Param(
    [string]$repoOwner,
    [string]$repo, 
    [string]$repoDescription,
    [string]$documentBotName,
    [bool]$initProject
);

if(!$repo)
{
    Write-Error "Repo name is required";
    return;
}

$executionDir = Get-Location

# Initialize a new repository
Write-Host "Initializing Git repo...";
git init $repo;
Set-Location $repo;

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
    dotnet new classlib -f netcoreapp2.1 -o "$repo.Tests"
    dotnet sln "$repo.sln" add ".\$repo.Tests\$repo.Tests.csproj"

    # Add test Nuget packages
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package Microsoft.NET.Test.Sdk
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package xunit
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package xunit.runner.visualstudio
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package FluentAssertions
    dotnet add ".\$repo.Tests\$repo.Tests.csproj" package Moq

    # Do package restore
    dotnet restore

    # Copy appveyor.yml template to the project root
    Copy-Item -Path ".\CakeScripts\templates\appveyor.yml" -Destination "appveyor.yml";

    # Do not continue if the user did not provide a token file
    if (!(Test-Path "$executionDir\Tokens.json")) 
    {
        Write-Host "Could not find Tokens.json - Automated repository creation will not execute";
        return;
    }

    # Read the tokens from the Tokens.json file
    $tokens = Get-Content -Raw -Path $executionDir\Tokens.json | ConvertFrom-Json
        
    if($tokens.githubToken)
    {
        Write-Host "Creating GitHub repository...";
        . .\CakeScripts\scripts\GitHubRepo.ps1
        New-GitHubRepository -Name $repo -Description $repoDescription -UserToken $tokens.githubToken

        git remote add origin "git@github.com:$repoOwner/$repo.git"
        
        if($documentBotName)
        {
            Write-Host "Setting up document publishing bot link";
            .\CakeScripts\scripts\GitHubCollaborator.ps1 -repoOwner $repoOwner -repo $repo -collaboratorName $documentBotName -githubToken $tokens.githubToken
        }
    }
    else
    {
        Write-Host "No Github token could be found - No service setup will be done" -ForegroundColor Yellow;
        return;
    }

    if($tokens.appVeyortoken)
    {
        Write-host "Creating AppVeyor project...";
        .\CakeScripts\scripts\CreateAppVeyorRepo.ps1 -repoOwner $repoOwner -repo $repo -appveyortoken $tokens.appVeyortoken
    }

    if($tokens.coverallsToken)
    {
        Write-Host "Creating Coveralls repo...";
        .\CakeScripts\scripts\CreateCoverallsRepo.ps1 -repoOwner $repoOwner -repo $repo -coverallstoken $tokens.coverallsToken
    }

    Write-Host "Repository setup completed";
    Write-Host "";
    Write-Host "User interaction is required to complete setup of AppVeyor.yml" -ForegroundColor Green;
    Write-Host "Please open the appveyor.yml file then press any key to continue" -ForegroundColor Green;
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

    # Guide user to get Nuget token, encrypt it and copy it into the appveyor.yml file
    Write-Host "---";
    Write-Host "To publish your project to Nuget an access token is required";
    Write-Host "If you already have a valid, encrypted token you can add it to appveyor.yml and skip this step.";
    Write-Host "Do you need a token (y/n)" -ForegroundColor Green;
    $key = Read-Host;
    if($key -eq "y")
    {
        Write-Host "---";
        Write-Host "The Nuget access token page will be opened as well as the AppVeyor page to encrypt data.";
        Write-Host "Copy your access token from Nuget, then encrypt it on the AppVeyor page and paste it into appveyor.yml in the place of EncryptedNugetKeyGoesHere";
        Write-Host "Press any key to open the pages" -ForegroundColor Green;
        $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
        Start-Process "https://ci.appveyor.com/tools/encrypt";
        Start-Process "https://www.nuget.org/account/apikeys";

        Write-Host "Press any key to continue" -ForegroundColor Green;
        $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
    }

    # Guide user to get Coveralls token, encrypt it and copy it into the appveyor.yml file
    Write-Host "---";
    Write-Host "To be able to push coverage results to Coveralls an access token is required, this token was created when the Coveralls project was set up";
    Write-Host "The Coveralls project page will be opened. Please copy the token, encrypt it on the AppVeyor page and paste it into appveyor.yml in the place of EncryptedCoverallsRepoTokenGoesHere";
    Write-Host "Press any key to open the Coverall project page" -ForegroundColor Green;
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
    Start-Process "https://coveralls.io/github/$repoOwner/$repo";
    Write-Host "Press any key to continue" -ForegroundColor Green;
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

    # Guide user to get token for publishing documentation to gh-pages
    Write-host "---"
    Write-Host "To be able to publish documentation a token is required for the user that can publish the documentation.";
    Write-Host "If you already have an encrypted token for publishing you can add it to appvyor.yml and skip this step";
    Write-Host "Do you need a token (y/n)" -ForegroundColor Green;
    $key = Read-Host;
    if($key -eq "y")
    {
        Write-Host "The GitHub personal access token page will be opened";
        Write-Host "Create a new token with the public_repo claim, encrypt it on the AppVeyor page and paste it into appveyor.yml in the place of EncryptedBotTokenGoesHere";
        Write-Host "Press any key to continue" -ForegroundColor Green;
        $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
    }

    # Ask user to close and save appveyor.yml
    Write-Host "---";
    Write-Host "Please save and close appveyor.yml";
    Write-Host "Press any key to continue" -ForegroundColor Green;
    $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');

    # Ask user if new repository should be pushed to GitHub
    Write-Host "---";
    Write-Host "Do you want to auto-push the repository to GitHub (y/n)?" -ForegroundColor Green;
    $key = Read-Host;
    if($key -eq "y")
    {
        Write-Host "Before the repository can be pushed to GitHub the build.cake script has to be completed";
        Write-Host "Please complete the script, then press any key to continue" -ForegroundColor Green;
        $null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
        
        git add .
        git commit -a -m "Initial commit"
        # Create Git tag so that SemVer.Git.MSBuild has a valid starting point
        git tag 1.0.0.0
        git push --set-upstream origin master
    }   
    else
    {
        Write-Host "Repository will not be pushed to GitHub";
    }
}

Write-Host "Repository quickstart done";