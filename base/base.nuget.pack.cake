/*
 * Generate NuGet packages from nuspec files
 */

#region Variables

// Directory where NuGet packages should be placed
var nugetDirectory = "./nupkg/";
// String used to locate nuspec files
var nuspecFileString = "./{0}/{0}.nuspec";

#endregion

#region Tasks

// Create Nuget Package
Task ("NugetPack")
	.Does (() => {
		if(!testPassed)
		{
			Error("Unit tests failed - Nuget package won't be generated");
			return;
		}
        if(buildConfiguration == "debug")
        {
            Information("Nuget package will not be built in Debug mode");
            return;
        }

        CreateDirectory (nugetDirectory);

        foreach(var project in projectFiles)
        {
            Information($"Generating NuGet package package for {project.Key}");
            var nuspecFile = string.Format(nuspecFileString, project.Key);

            ReplaceRegexInFiles(nuspecFile, "0.0.0", version);
            ReplaceRegexInFiles(nuspecFile, "ReleaseNotesHere", releaseNotesText);
            
            if(gitHash != "none")
            {
                ReplaceRegexInFiles(nuspecFile, "GitHashHere", gitHash);
            }
            
            NuGetPack (nuspecFile, GetNuGetPackSettings());	
        }
	});

#endregion

#region Private Methods

// Generates the NuGetPackSettings
private NuGetPackSettings GetNuGetPackSettings()
{
    return new NuGetPackSettings 
    { 
        Verbosity = NuGetVerbosity.Detailed,
        OutputDirectory = nugetDirectory
    };
}

#endregion