/*
 * Upload coverage results to OpenCover
 */

#region Addins

#addin nuget:?package=Cake.Coveralls&version=0.9.0

#endregion

#region Tools

#tool nuget:?package=coveralls.io&version=1.3.4

#endregion

#region Tasks

// Uploads Code Coverage results
Task ("CoverageUpload")
	.Does (() => {
        var blockText = "CoverageUpload";
        StartBlock(blockText);

        if(runningOnLocal)
        {
            Information("Coverage results are not uploaded for local builds");
            return;
        }

		var coverallRepoToken = EnvironmentVariable("CoverallRepoToken");
        if(string.IsNullOrEmpty(coverallRepoToken))
        {
            Warning("Could not find Coverall token - Coverage results will not be uploaded");
            return;
        }

		CoverallsIo(coverPath, new CoverallsIoSettings()
		{
			RepoToken = coverallRepoToken
		});

        EndBlock(blockText);
	});

#endregion