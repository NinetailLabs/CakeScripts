/*
 * Execute NUnit tests.
 * Script uses AltCover to calculate test coverage.
 * To work correctly the AltCover and Appveyor.TestLogger nuget packages must be added to the test project.
 * The test output is geared entirely towards AppVeyor, however there are packages available that can also output NUnit3 xml
 */

 // Deprecation notice: This script is no longer supported. It will be removed in a future version.

#region Variables

// Indicate if the unit tests passed
var testPassed = false;
// Path where coverage results should be saved
var coverPath = "./coverageResults.xml";
// Test result output file
var testResultFile = "./TestResult.xml";
// Filter used to locate unit test dlls
var unitTestFilter = "./*Tests/*.Tests.csproj";
// Collection of namespaces to exclude
var excludedNamespaces = new List<string> { "NUnit", "xunit", "Microsoft*" };

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

#region Public Methods

// Add namespaces to the exclusion list
public void AddNamespaceExclusion(string namespaceToExclude)
{
    excludedNamespaces.Add(namespaceToExclude);
}

#endregion

#region Private Methods

// Delete the coverage results if it already exists
private void RemoveCoverageResults()
{
    var outputPath = MakeAbsolute(File(coverPath));
    if(FileExists(outputPath))
    {
        Information("Clearing existing coverage results");
        DeleteFile(outputPath);
    }
}

// Execute NUnit tests
private void ExecuteUnitTests()
{
    testPassed = true;

    var testAssemblies = GetFiles(unitTestFilter);

    foreach(var assembly in testAssemblies)
    {
        try
        {
            var assemblyFilename = assembly.GetFilenameWithoutExtension();

            var coverOutput = MakeAbsolute(File(coverPath));
            var testResultOutput = MakeAbsolute(File(testResultFile));

            Information($"Testing: {assembly}");

            excludedNamespaces.Add(assemblyFilename.ToString());
            var excludeString = string.Join("|", excludedNamespaces);
            
            var testSettings = new DotNetCoreTestSettings {
			Configuration = buildConfiguration,
			NoBuild = true,
            ArgumentCustomization = args=> args
                .Append("/p:AltCover=true")
                .Append($"/p:AltCoverXmlReport={coverOutput}")
                .Append($"/p:AltCoverAssemblyFilter={excludeString}")
                .Append("--test-adapter-path:.")
                .Append("--logger:Appveyor")
		};

            DotNetCoreTest(assembly.ToString(), testSettings);
        }
        catch(Exception ex)
        {
            testPassed = false;
            Error(ex.Message);
            Error("There was an error while executing tests");
        }
    }
}

#endregion