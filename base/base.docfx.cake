/*
 * Generate DocFX documentation and push it to the repository
 */

#region Addins

#addin nuget:?package=Cake.DocFx
#addin nuget:?package=Cake.Git

#endregion

#region Tools

#tool nuget:?package=docfx.console

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
var branch = "gh-pages";
// Git repository URL
var gitRepo = "";

#endregion

#region Tasks

// Generates DocFX documentation and if the build is master pushes it to the repo
Task ("Documentation")
	.Does (() => {
		GitReset(".", GitResetMode.Hard);

        if(!CheckIfDocumentationShouldBeGenerated())
        {
            return;
        }

		var tool = Context.Tools.Resolve("docfx.exe");
		StartProcess(tool, new ProcessSettings{Arguments = "docfx_project/docfx.json"});
		var newDocumentationPath = MakeAbsolute(Directory("docfx_project/_site"));
		
		Information("Cloning documentation branch");
		GitClone(gitRepo, MakeAbsolute(Directory("docClone")), new GitCloneSettings{
			BranchName = branch
		});

		Information("Preparing updated site");
		CopyDirectory(MakeAbsolute(Directory("docClone/.git")), MakeAbsolute(Directory("docfx_project/_site/.git")));
		GitAddAll(newDocumentationPath);

		Information("Pushing updated documentation to repo");
		GitCommit(newDocumentationPath, botName, botEmail, "Documentation for " + version);
		GitPush(newDocumentationPath, botName, botToken, branch);
		Information("Completed Documentation update");
	});

#endregion

#region Private Methods

// Check if documentation should be generated
private bool CheckIfDocumentationShouldBeGenerated()
{
    if(buildType != "master")
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