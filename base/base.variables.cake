/*
 * Contains global variables and arguments for general scripts.
 */

#region Arguments

// The target to build
var target = Argument ("target", "Default");

// The branch that should be built
var branch = Argument<string>("branch", "develop");

// The number of builds that have been built by the build system
var buildCounter = Argument<int>("buildCounter", 0);

// The build configuration to use for building the solutions
var buildConfiguration = Argument("buildConfiguration", "Release");

// Git SHA for the commit
var gitHash = Argument<string>("gitHash", "none");

// The commit message
var commitMessage = Argument<string>("commitMessage", "");

#endregion

#region Variables

// Version of the executable that was built
var version = "0.0.0";
// Version of the executable that was built with the CI number specification
var ciVersion = "0.0.0-CI00000";
// Formattable string used to locate release folder for a solution. Should contain 
// two replacements. {0} for the solution folder and {1} for the buildConfiguration
var releaseFolderString = "./{0}/bin/{1}/";
// The type of binary (dll or exe)
var releaseBinaryType = "dll";
// Name of the project being built
var projectName = "";

#endregion

#region Tasks

Task ("OutputVariables")
    .Does(() => {
        var blockText = "Output Variables";
        StartBlock(blockText);

        Information($"Branch: {branch}");
        Information($"Build Counter: {buildCounter}");
        Information($"Build Configuration: {buildConfiguration}");
        Information($"Git Hash: {gitHash}");
        Information($"Commit Message: {commitMessage}");

        EndBlock(blockText);
    });

#endregion