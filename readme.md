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

## QuickStart
To quickly create and set up a new Git repository grab the QuickStart PowerShell script with the following
```
Invoke-WebRequest https://raw.githubusercontent.com/NinetailLabs/CakeScripts/master/quickstart.ps1 -OutFile quickstart.ps1
```

Then invoke the script
```
.\quickstart.ps1 -repo <repo-name>
```
The QuickStart script does the following
- Create a new Git repository
- Pull down the `build.ps1` bootstrap file
- Add the VisualStudio `.gitignore` file from GitHub
- Add the CakeScript repository as a Git submodule 
- Copy the `template.build.cake` to the repository root and rename it to `build.cake`

## NetStandard2.0 Project QuickStart
The Quickstart also support the creation of a pre-configured NetStandard2.0 project.  
Invoke the quickstart script as follows
```
.\quickstart.ps1 -repo <repo-name> -initProject $true
```

Beside the basic Quickstart configuration, a Visual Studio solutions will be created with the following:
- Solution file named the same as the `repo` value passed
- NetStandard2.0 class library with:
  - [SemVer.Git.MSBuild](https://www.nuget.org/packages/SemVer.Git.MSBuild/) for automated SemVer versioning
  - [Microsoft.SourceLink.GitHub](https://www.nuget.org/packages?q=Microsoft.SourceLink.GitHub) for GitHub based SourceLink
  - Nuspec template
  - `SemVer.MSBuild.props` pre-configured to create version numbers starting with 1.0.0
- NetCore2.1 XUnit test project with:
  - [FluentAssertions](https://www.nuget.org/packages/FluentAssertions/)
  - [Moq](https://www.nuget.org/packages/Moq/)
- Git tag `1.0.0.0` that is used by `SemVer.Git.MSBuild` for versioning start point
- `NineTailLabs.DotSettings` that contains the ReSharper settings used by NineTail Labs for code layout (Currently this has to be hooked into ReSharper manually)

### Repository auto-creation
The quickstart script can be used to automatically set up a GitHub repository, AppVeyor build for the repository as well as a Coveralls code coverage project for the repository.  
In order for auto-creation to work the script requires an access token for each of the services that should be auto-configured. Tokens are retrieved from the `Tokens.json` file which
should be in the same directory as the `quickstart.ps1` script.  
A template can be downloaded using:
```
Invoke-WebRequest https://raw.githubusercontent.com/NinetailLabs/CakeScripts/master/scripts/Tokens.json -OutFile Tokens.json
```

To run the quickstart with auto-creation invoke it as follows
```
.\quickstart.ps1 -repoOwner <repo-owner> -repo <repo-name> -repoDescription <repo-description> [-documentBotname <document-bot-name>] -initProject $true
```

Tokens can be generated as follows:
#### Github
Token can be generated [here](https://github.com/settings/tokens).  
The token needs the *public_repo* claim (currently private repos are not supported by the script)

#### AppVeyor
Token can be found [here](https://coveralls.io/account)

#### Coveralls
Token can be generated [here](https://ci.appveyor.com/api-token)

Tokens allow access to your account and should be carefully gaurded! It is recommended to store the Tokens.json file in a password safe, that way the tokens are kept secure
and can be easily retrieved when needed.

## Special Thanks
Thanks to [Jeffery Hicks](https://jdhitsolutions.com/blog/about-me/) who provided the excellent [code](https://jdhitsolutions.com/blog/powershell/5373/creating-a-github-repository-from-powershell/) for creating GitHub repositories from powershell