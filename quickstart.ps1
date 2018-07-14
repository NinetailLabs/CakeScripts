Param([string]$repo);

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

Write-Host "Repository quickstart done";