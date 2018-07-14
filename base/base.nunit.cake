/*
 * Execute NUnit tests.
 * Script uses OpenCover to calculate coverage results
 */

#region Tools

#tool nuget:?package=NUnit.ConsoleRunner
#tool nuget:?package=OpenCover

#endregion

#region Variables

// Indicate if the unit tests passed
var testPassed = false;
// Path where coverage results should be saved
var coverPath = "./coverageResults.xml";
// Test result output file
var testResultFile = "./TestResult.xml";
// Filter used to locate unit test dlls
var unitTestFilter = "./*Tests/bin/Release/*.Tests.dll";

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
        PushTestResults(testResultFile);

        EndBlock(blockText);
    });

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
    var testAssemblies = GetFiles(unitTestFilter);
    try
    {
        OpenCover(tool =>
            {
                tool.NUnit3(testAssemblies, GetNunitSettings());
            },
            new FilePath(coverPath),
            GetOpenCoverSettings()
        );
        testPassed = true;
    }
    catch(Exception)
    {
        Error("There was an error while executing tests");
    }
}

// Create NUnit3 settings
private NUnit3Settings GetNunitSettings()
{
    return new NUnit3Settings
    {
        OutputFile = testResultFile,
        TeamCity = runningOnTeamCity,
        WorkingDirectory = ".",
        Work = MakeAbsolute(Directory("."))
    };
}

// Get filter string for OpenCover to ensure all project files are included and all test 
// projects are excluded
private OpenCoverSettings GetOpenCoverSettings()
{
    var inclusionFilter = "";
    var exclusionFilter = "";
    foreach(var test in testFiles)
    {
        exclusionFilter += $"-[{test.Key}]* ";
    }
    foreach(var project in projectFiles)
    {
        inclusionFilter += $"+[{project.Key}]* ";
    }

    return new OpenCoverSettings
    {
        MergeOutput = true,
        ReturnTargetCodeOffset = 0
    }
    .WithFilter($"{inclusionFilter} {exclusionFilter}")
    .ExcludeByAttribute("System.CodeDom.Compiler.GeneratedCodeAttribute");
}

#endregion