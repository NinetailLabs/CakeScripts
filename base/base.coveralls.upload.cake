/*
 * Upload coverage results to OpenCover
 */

#region Addins

#addin nuget:?package=Cake.Coveralls

#endregion

#region Tools

#tool nuget:?package=coveralls.io

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