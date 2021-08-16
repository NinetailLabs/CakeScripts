/*
 * Execute unit tests.
 * Script uses coverlet to calculate test coverage and provide coverage as a single OpenCover xml file.
 * This script is recommended for use in project with multiple test assemblies when only a single coverage file is required.
 * To work correctly the coverlet.msbuild nuget packages must be added to the test project.
 * See https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/Examples/MSBuild/MergeWith/HowTo.md for details of how multi-file merging works in coverlet
 */

#region Addins

#addin nuget:?package=Cake.Coverlet&version=2.5.4

#endregion

#region Variables

// Indicate if the unit tests passed
var testPassed = false;
// Directory where coverage results should be generated
var coverDirectory = "./coverage/";
// Name of the coverage file for the final result
var coverFilename = "coverage.xml";
// Path to the final coverage file
var coverPath = $"{coverDirectory}{coverFilename}";
// The name of the temporary file where coverage results will be combined
var sharedCoverageFile = "coverage.json";
// Filter used to locate unit test dlls
var unitTestFilter = "./*Tests/*.Tests.csproj";
// Collection of namespaces to exclude
var excludedNamespaces = new List<string> { "NUnit", "xunit", "Microsoft*" };
// Collection of namespaces to include
var includedNamespaces = new List<string>();

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
    CleanDirectories(coverDirectory);
}

// Execute NUnit tests
private void ExecuteUnitTests()
{
    testPassed = true;

    var testAssemblies = GetFiles(unitTestFilter);
    var totalAssemblies = testAssemblies.Count;
    var currentAssembly = 0;

    foreach(var assembly in testAssemblies)
    {
        try
        {
            var assemblyFilename = assembly.GetFilenameWithoutExtension();

            Information($"Testing: {assembly}");

            var dotNetTestSettings = new DotNetCoreTestSettings {
                Configuration = buildConfiguration,
                NoBuild = true
            };

            CoverletSettings coverletSettings = null;
            if(totalAssemblies == 1)
            {
                coverletSettings = GetCoverletSettingsForSingleAssembly(assembly.ToString());
            }
            else if (currentAssembly == 0)
            {
                coverletSettings = GetCoverletSettingsForFirstAssembly(assembly.ToString());
            }
            else if (currentAssembly < totalAssemblies - 1)
            {
                coverletSettings = GetCoverletSettingsForMiddleAssembly(assembly.ToString());
            }
            else
            {
                coverletSettings = GetCoverletSettingsForLastAssembly(assembly.ToString());
            }

            if(coverletSettings == null)
            {
                throw new Exception("Could not configure Coverlet settings");
            }

            DotNetCoreTest(assembly, dotNetTestSettings, coverletSettings);
        }
        catch(Exception ex)
        {
            testPassed = false;
            Error(ex.Message);
            Error("There was an error while executing tests");
        }

        currentAssembly++;
    }
}

// Get the coverlet settings for a solution with only a single assembly
private CoverletSettings GetCoverletSettingsForSingleAssembly(string assembly)
{
    return new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(coverDirectory),
        CoverletOutputName = coverFilename,
        Exclude = excludedNamespaces,
        Include = includedNamespaces
    };
}

// Get the coverlet settings for the first assembly in a multi-assembly solution
private CoverletSettings GetCoverletSettingsForFirstAssembly(string assembly)
{
    return new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputDirectory = Directory(coverDirectory),
        CoverletOutputName = sharedCoverageFile,
        Exclude = excludedNamespaces,
        Include = includedNamespaces
    };
}

// Get the coverlet settings for assembly in a multi-assembly solution that is neither the first nor the last assembly
private CoverletSettings GetCoverletSettingsForMiddleAssembly(string assembly)
{
    var sharedFile = File($"{coverDirectory}{sharedCoverageFile}");
    return new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputDirectory = Directory(coverDirectory),
        CoverletOutputName = sharedCoverageFile,
        MergeWithFile = sharedFile,
        Exclude = excludedNamespaces,
        Include = includedNamespaces
    };
}

// Get the coverlet settings for the last assembly in a multi-assembly solution
private CoverletSettings GetCoverletSettingsForLastAssembly(string assembly)
{
    var sharedFile = File($"{coverDirectory}{sharedCoverageFile}");
    return new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = Directory(coverDirectory),
        CoverletOutputName = coverFilename,
        MergeWithFile = sharedFile,
        Exclude = excludedNamespaces,
        Include = includedNamespaces
    };
}

#endregion