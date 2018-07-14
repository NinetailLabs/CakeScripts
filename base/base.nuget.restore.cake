/*
 * Restore packages from NuGet using NuGet restore
 */

#region Tasks

// Restore NuGet packages for all solutions
Task ("NugetRestore")
    .Does(() => {
        var blockText = "Nuget Restore";
        StartBlock(blockText);

        foreach(var solution in solutionFiles)
        {
            Information($"Restoring NuGet packages for {solution.Key}");
            NuGetRestore(solution.Value);
            Information($"Completed NuGet restore for {solution.Key}");
        }

        EndBlock(blockText);
    });

#endregion