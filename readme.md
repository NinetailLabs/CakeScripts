# NineTail Labs - CakeScripts
This is a collection of base scripts for [Cake](https://cakebuild.net) that is used by NineTail Labs to build our dotnet projects.
The goal is to provide basic scripts that can be reused between project with minimal change while also allowing easy extensibility for specific project by simply swapping out a base script for a custom one for the specific project.

## Usage
- Grab the Cake bootstrap file from the Cake website by invoking
```
Invoke-WebRequest https://cakebuild.net/download/bootstrapper/windows -OutFile build.ps1
```  

- Add the CakeScripts repository as a Git submodule to the target project, then copy the `template.build.cake` file to the root of the repository (next to the `build.ps1` file from the step above) and rename it to `build.cake`. It is also possible to just copy the scripts directly over to the repository

- Build the project by calling from PowerShell
```
.\build.ps1
```
If PowerShell has a security issue, read the Cake guide [here](https://cakebuild.net/docs/tutorials/powershell-security)