/*
 * Retrieve details about the project that has to be built. This include finding the solution
 * files, project files and test projects
 */

#region Variables

// String used to search for solution files
var slnSearchString = "./*.sln";
// String used to search for project files
var projSearchString = "./**/*.csproj";
// String used to search for test projects
var testSearchString = "./*Tests/*.Tests.csproj";
// Solution files that was found
Dictionary<string, FilePath> solutionFiles;
// Project files that were found split into project names and filepaths
Dictionary<string, FilePath> projectFiles;
// Test projects that were found split into project names and filepaths
Dictionary<string, FilePath> testFiles;

#endregion

#region Task

Task ("LocateFiles")
    .Does(() => {
        var blockText = "LocateFiles";
        StartBlock(blockText);

        solutionFiles = new Dictionary<string, FilePath>();
        projectFiles = new Dictionary<string, FilePath>();
        testFiles = new Dictionary<string, FilePath>();        

        FindSolutionFiles();
        FindProjectFiles();
        FindTestFiles();

        if(solutionFiles.Count == 0)
        {
            throw new CakeException("No solution file could be found");
        }
        if(projectFiles.Count == 0)
        {
            throw new CakeException("No project files could be found");
        }
        if(testFiles.Count == 0)
        {
            Warning("No test projects were found");
        }

        EndBlock(blockText);
    });

#endregion

#region Private methods

// Find solution files using the slnSearchString
private void FindSolutionFiles()
{
    Information("Searching for Solution files...");
    var solutions = GetFiles(slnSearchString);
    Information("Located Solution files:");
    foreach(var entry in solutions)
    {
        var solutionName = GetSolutionName(entry);
        Information($"{solutionName} - {entry}");
        solutionFiles.Add(solutionName, entry);
    }
}

// Find project files using the projSearchString
private void FindProjectFiles()
{
    Information("Searching for Project files...");
    var projects = GetFiles(projSearchString);
    Information("Found project files:");
    foreach(var project in projects)
    {
        var projName = GetProjectName(project);
        if(projName.EndsWith(".Tests"))
        {
            continue;
        }
        
        Information($"{projName} - {project}");
        projectFiles.Add(projName, project);
    }
}

// Find test files using the testSearchString
private void FindTestFiles()
{
    Information("Searching for Test files...");
    var testProjects = GetFiles(testSearchString);
    Information("Found test files:");
    foreach(var test in testProjects)
    {
        var projectName = GetProjectName(test);
        Information($"{projectName} - {test}");
        testFiles.Add(projectName, test);
    }
}

// Get project name without .csproj and the folder path
private string GetProjectName(FilePath projPath)
{
    return projPath
        .GetFilename()
        .ToString()
        .Replace(".csproj", "");
}

// Get the solution name without .sln or the folder path
private string GetSolutionName(FilePath solutionPath)
{
    return solutionPath
        .GetFilename()
        .ToString()
        .Replace(".sln", "");
}

#endregion