/*
 * Build solutions using MSBuild
 */

#region Addins

#addin nuget:?package=Cake.VersionReader&version=5.1.0

#endregion

#region Variables

// The version of MSBuild to use
var msBuildPlatform = MSBuildPlatform.Automatic;
// The target platform to build for (MSIL is AnyCPU)
var platformTarget = PlatformTarget.MSIL;

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
                                MSBuildPlatform = msBuildPlatform,
                                PlatformTarget = platformTarget,
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