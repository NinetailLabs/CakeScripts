#region ScriptImports

// Scripts
#load "Cake.Scripts/base/base.buildsystem.cake"
#load "Cake.Scripts/base/base.variables.cake"
#load "Cake.Scripts/base/base.setup.cake"
#load "Cake.Scripts/base/base.nuget.restore.cake"
#load "Cake.Scripts/base/base.paket.restore.cake"
#load "Cake.Scripts/base/base.msbuild.cake"
#load "Cake.Scripts/base/base.nunit.cake"
#load "Cake.Scripts/base/base.coveralls.upload.cake"
#load "Cake.Scripts/base/base.gitreleasenotes.cake"
#load "Cake.Scripts/base/base.nuget.pack.cake"
#load "Cake.Scripts/base/base.nuget.push.cake"
#load "Cake.Scripts/base/base.docfx.cake"

#endregion

#region Tasks

// Set up variables specific for the project
Task ("VariableSetup")
	.Does(() => {
		releaseFolderString = "./{0}/bin/{1}/netstandard2.0";
		releaseBinaryType = "dll";
		repoOwner = "";
		botName = "";
		botEmail = "";
		botToken = EnvironmentVariable("BotToken");
		gitRepo = string.Format("https://github.com/{0}/{1}.git", repoOwner, projectName);
	});

Task ("Default")
	.IsDependentOn ("DiscoverBuildDetails")
	.IsDependentOn ("OutputVariables")
	.IsDependentOn ("LocateFiles")
	.IsDependentOn ("VariableSetup")
	.IsDependentOn ("NugetRestore")
	.IsDependentOn ("PaketRestore")
	.IsDependentOn ("Build")
	.IsDependentOn ("UnitTests")
	.IsDependentOn ("CoverageUpload")
	.IsDependentOn ("GenerateReleaseNotes")
	.IsDependentOn ("NugetPack")
	.IsDependentOn ("NugetPush")
	.IsDependentOn ("Documentation");

#endregion

#region RunTarget

RunTarget (target);

#endregion