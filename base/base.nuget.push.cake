/*
 * Push NuGet package to nuget.org
 */

#region Variables

// NuGet API key
var apiKey = "";

#endregion

#region Tasks

//Push to Nuget
Task ("NugetPush")
	.WithCriteria (branch == "master")
	.Does (() => {
        var blockText = "NugetPush";
        StartBlock(blockText);

        if(!CheckIfPackagesCanBePushed())
        {
            return;
        }

        PushPackagesToNuget();
        
        EndBlock(blockText);
	});

#endregion

#region Private Methods

// Check if NuGet packages can be pushed to nuget.org
private bool CheckIfPackagesCanBePushed()
{
    if(!testPassed)
    {
        Error("Unit tests failed - Not pushing NuGet packages");
        return false;
    }
    
    apiKey = EnvironmentVariable("NugetKey");
    if(string.IsNullOrEmpty(apiKey))
    {
        Warning("NuGet API key is empty - Not pushing to NuGet");
        return false;
    }

    return true;
}

// Push NuGet packages to nuget.org
private void PushPackagesToNuget()
{
    foreach(var project in projectFiles)
    {
        // Get the newest (by last write time) to publish
        var newestNupkg = GetFiles ($"nupkg/{project.Key}*.nupkg")
            .OrderBy (f => new System.IO.FileInfo (f.FullPath).LastWriteTimeUtc)
            .LastOrDefault();        

        NuGetPush (newestNupkg, new NuGetPushSettings 
        { 
            Verbosity = NuGetVerbosity.Detailed,
            Source = "https://www.nuget.org/api/v2/package/",
            ApiKey = apiKey
        });
    }
}

#endregion