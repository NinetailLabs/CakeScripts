/*
 * Generate DocFX documentation and push it to the repository.
 * The script will download the msdn.4.5.2 and extract is to the ./tools/MsdnDocs folder making it easy to have this available when building documents.
 * Simply add the snippet below to the docfx.json to ensure it has access to the package content.
 * "xref": [
 *     "../tools/MsdnDocs/content/msdn.4.5.2.zip",
 *     "../tools/MsdnDocs/content/namespaces.4.5.2.zip"
 *   ]
 */

#region Addins

#addin nuget:?package=Cake.DocFx&version=1.0.0
#addin nuget:?package=Cake.Git&version=1.1.0

#endregion

#region Tools

#tool nuget:?package=docfx.console&version=2.58.0

#endregion

#region Variables

// Owner of Git repository for which update should be done
var repoOwner = "";
// Name of the Bot used to push documentation to repo
var botName = "";
// Token used to authenticate bot to git repo
var botToken = "";
// Email address of the bot used to update git repo
var botEmail = "";
// Branch that documentation should be pushed to
var documentBranch = "gh-pages";
// Git repository URL
var gitRepo = "";

#endregion

#region Tasks

// Generates DocFX documentation and if the build is master pushes it to the repo
Task ("Documentation")
	.Does (() => {
        var blockText = "Build Success Check";
        StartBlock(blockText);

        GitReset(".", GitResetMode.Hard);

        if(!CheckIfDocumentationShouldBeGenerated())
        {
            return;
        }

        DownloadMsdnLinkPackage();
        GenerateDocumentation();

        EndBlock(blockText);
	});

// Generate the actual DocFX documentation and push it to the repo
private void GenerateDocumentation()
{
    var tool = Context.Tools.Resolve("docfx.exe");
    StartProcess(tool, new ProcessSettings{Arguments = "docfx_project/docfx.json"});
    var newDocumentationPath = MakeAbsolute(Directory("docfx_project/_site"));
    
    Information("Cloning documentation branch");
    GitClone(gitRepo, MakeAbsolute(Directory("docClone")), new GitCloneSettings{
        BranchName = documentBranch
    });

    Information("Preparing updated site");
    CopyDirectory(MakeAbsolute(Directory("docClone/.git")), MakeAbsolute(Directory("docfx_project/_site/.git")));
    GitAddAll(newDocumentationPath);

    Information("Pushing updated documentation to repo");
    GitCommit(newDocumentationPath, botName, botEmail, "Documentation for " + version);
    GitPush(newDocumentationPath, botName, botToken, documentBranch);
    Information("Completed Documentation update");
}

// Download the MSDN link Nuget package and extract it to the Tools directory
private void DownloadMsdnLinkPackage()
{
    Information("Downloading MSDN Nuget package");
    DownloadFile("https://www.nuget.org/api/v2/package/msdn.4.5.2/0.1.0-alpha-1611021200", $"./tools/MsdnDocs.nupkg");
    Unzip("./tools/MsdnDocs.nupkg", "./tools/MsdnDocs");
}

#endregion

#region Private Methods

// Check if documentation should be generated
private bool CheckIfDocumentationShouldBeGenerated()
{
    if(branch != "master")
    {
        Information("Documentation is only pushed for master branch");
        return false;
    }

    if(string.IsNullOrEmpty(botName) 
        || string.IsNullOrEmpty(botEmail) 
        || string.IsNullOrEmpty(botToken)
        || string.IsNullOrEmpty(repoOwner)
        || string.IsNullOrEmpty(gitRepo))
    {
        Warning("Missing details required to push documentation to git repo");
        return false;
    }

    return true;
}

#endregion