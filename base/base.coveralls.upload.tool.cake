/*
 * Upload coverage results to OpenCover using the Coveralls.net tool (https://github.com/csmacnz/coveralls.net?tab=readme-ov-file)
 * Script requires the coveralls.net Global tool to be available on the build server in order to run.
 * To install the tool locally run: `dotnet tool install coveralls.net`
 * If you haven't installed a tool locally before first run: `dotnet new tool-manifest`
 */
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

		var args = $"csmacnz.Coveralls --opencover -i {coverPath} --repoToken {coverallRepoToken} --commitId {gitHash} --commitBranch {branch}";
        StartProcess("dotnet", args);

        EndBlock(blockText);
	});

#endregion