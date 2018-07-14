/*
 * Build solutions using MSBuild
 */

#region Addins

#addin nuget:?package=Cake.VersionReader

#endregion

#region Tasks

Task ("Build")
    .Does(() => {
        var blockText = "Build";
        StartBuildBlock(blockText);

        foreach(var solution in solutionFiles)
        {
            Information($"Building: {solution.Key}...");

            MSBuild (solution.Value, new MSBuildSettings 
                            {
                                Verbosity = Verbosity.Quiet,
                                Configuration = buildConfiguration
                            });

            var releaseFolder = string.Format(releaseFolderString, solution.Key, buildConfiguration);
            var file = MakeAbsolute(Directory(releaseFolder)) + $"/{solution.Key}.{releaseBinaryType}";
            version = GetVersionNumber(file);
            ciVersion = GetVersionNumberWithContinuesIntegrationNumberAppended(file, buildCounter);
            Information("Version: " + version);
            Information("CI Version: " + ciVersion);
            PushVersion(ciVersion);
        }

        EndBuildBlock(blockText);
    });

#endregion