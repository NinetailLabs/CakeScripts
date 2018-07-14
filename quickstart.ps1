Param([string]$repo);

# Initialize a new repository
Write-Host "Initializing Git repo..."
git init $repo;

# Add VisualStudio .gitignore from GitHub
Write-Host "Grabbing VisualStudio .gitignore..."
Invoke-WebRequest https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -OutFile .gitignore

# Add CakeScripts as a Submodule
Write-Host "Adding CakeScripts as Git submodule..."
git submodule add -b master https://github.com/DeadlyEmbrace/CakeScripts.git

# Copy template.build.cake to root
Write-Host "Copying build.cake to repo root...";
Copy-Item -Path ".\CakeScript\template.build.cake" -Destination "build.cake"

Write-Host "Repository quickstart done";