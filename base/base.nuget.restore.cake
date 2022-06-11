/*
 * Restore packages from NuGet using NuGet restore
 */

# region Variables

// The path where MSBuild is located. Leave empty to ignore
var msBuildPath = string.Empty;

# endregion

#region Tasks

// Restore NuGet packages for all solutions
Task ("NugetRestore")
    .Does(() => {
        var blockText = "Nuget Restore";
        StartBlock(blockText);

        foreach(var solution in solutionFiles)
        {
            Information($"Restoring NuGet packages for {solution.Key}");
            if(string.IsNullOrEmpty(msBuildPath))
            {
                NuGetRestore(solution.Value);
            }
            else
            {
                NuGetRestore(solution.Value, new NuGetRestoreSettings
                {
                    MSBuildPath = new DirectoryPath(msBuildPath)
                });
            }
            Information($"Completed NuGet restore for {solution.Key}");
        }

        EndBlock(blockText);
    });

#endregion