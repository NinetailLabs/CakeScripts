/*
 * Execute DotNetCore tests
 * Script uses MiniCover to provide code coverage
 * MiniCover requires setting  [SetMiniCoverToolsProject("");] to a project that contains the MiniCover cli tool
 */

// Deprecation notice: This script is no longer supported. It will be removed in a future version.

#region AddIns

#addin Cake.MiniCover&version=0.29.0-next20180721071547

#endregion

#region Variables

// Indicate if the unit tests passed
var testPassed = false;
// Path where coverage results should be saved
var coverPath = "./coverage.xml";
// Test result output file
var testResultFile = "./TestResult.xml";
// Filter used to locate unit test csproj files
var unitTestProjectFilter = "./*Tests/*.Tests.csproj";
// Filter used to locate unit test dlls
var unitTestFilter = "./*Tests/bin/Release/netcoreapp2.1/*.dll";
// Filter for source code files that must be included in the coverage results
var sourceCodeFilter = "./**/*.cs";
// Filter for source code files that must be excluded from the coverage results
var testCodeFilter = "./*Tests/*.cs";
var testCodeInFolderFilter = "./*Tests/**/*.cs";
// Coverall token
var coverallRepoToken = EnvironmentVariable("CoverallRepoToken");

#endregion

#region Tasks

// Execute unit tests
Task ("UnitTests")
    .Does (() =>
    {
        var blockText = "Unit Tests";
        StartBlock(blockText);

        RemoveCoverageResults();
        ExecuteUnitTests();

        var resultFiles = GetFiles("TestResult_*.xml");
        foreach(var file in resultFiles)
        {
            Information($"Pushing test result file '{file.FullPath}'");
            PushTestResults(file.FullPath);
        }

        EndBlock(blockText);
    });

Task ("FailBuildIfTestFailed")
    .Does(() => {
        var blockText = "Build Success Check";
        StartBlock(blockText);

        if(!testPassed)
        {
            throw new CakeException("Unit test have failed - Failing the build");
        }

        EndBlock(blockText);
    });

#endregion

#region Private Methods

// Delete the coverage results if it already exists
private void RemoveCoverageResults()
{
    if(FileExists(coverPath))
    {
        Information("Clearing existing coverage results");
        DeleteFile(coverPath);
    }
}

// Execute NUnit tests
private void ExecuteUnitTests()
{
    Information("Running tests");
    var testAssemblies = GetFiles(unitTestProjectFilter);
             
    try
    {
        MiniCover(tool => {
            foreach(var project in testAssemblies)
            {
                Information($"Running tests for {project}");
                tool.DotNetCoreTest(project.FullPath, GetTestSettings());
            }
        },
        GetMiniCoverSettings());
        MiniCoverUninstrument();

        if(string.IsNullOrEmpty(coverallRepoToken))
        {
            MiniCoverReport(GetMiniCoverSettings()
            .GenerateReport(ReportType.XML));
        }
        else
        {
            try
            {
                MiniCoverReport(GetMiniCoverSettingsWithCoveralls()
                .GenerateReport(ReportType.COVERALLS | ReportType.XML));
            }
            catch(Exception exception)
            {
                Warning(exception);
            }
        }
        

        testPassed = true;
    }
    catch(Exception exception)
    {
        Error("There was an error while executing tests");
        Error(exception);
    }    
}

// Get settings for Coverall result posting
private CoverallsSettings GetCoverallSettings()
{
    return new CoverallsSettings
    {
        Branch = branch,
        CommitHash = gitHash,
        RepoToken = coverallRepoToken,
        CommitMessage = commitMessage
    };
}

// Get settings for DotNetCoreTests
private DotNetCoreTestSettings GetTestSettings()
{
    return new DotNetCoreTestSettings
    {
        NoBuild = true,
        Configuration = buildConfiguration,
        ResultsDirectory = ".",
        Logger = $"trx;LogFileName={testResultFile}"
    };
}

// Get setting for MiniCover
private MiniCoverSettings GetMiniCoverSettings()
{
    return new MiniCoverSettings()
        .WithAssembliesMatching(unitTestFilter)
        .WithoutSourcesMatching(testCodeFilter)
        .WithoutSourcesMatching(testCodeInFolderFilter)
        .WithSourcesMatching(sourceCodeFilter)
        .WithNonFatalThreshold();
}

private MiniCoverSettings GetMiniCoverSettingsWithCoveralls()
{
    return new MiniCoverSettings
        {
            Coveralls = GetCoverallSettings()
        }
        .WithAssembliesMatching(unitTestFilter)
        .WithoutSourcesMatching(testCodeFilter)
        .WithoutSourcesMatching(testCodeInFolderFilter)
        .WithSourcesMatching(sourceCodeFilter)
        .WithNonFatalThreshold();
}

#endregion