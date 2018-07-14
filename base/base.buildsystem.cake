/*
 * Checks the build environment when a build is started.
 * Will retrieve details such as the Build Server where the build is running as well as
 * provide methods that can be used to write information to the build server
 */

#region Variables

// Indicates that the build is not being executed by a Build system
var runningOnLocal = false;

// Indicates that the build is being executed by a TeamCity server
var runningOnTeamCity = false;

// Indicates that the build is being executed by an AppVeyor server
var runningOnAppVeyor = false;

#endregion

#region Tasks

Task ("DiscoverBuildDetails")
    .Does(() => {
        var blockText = "DiscoverBuildDetails";
        StartBlock(blockText);

        runningOnLocal = BuildSystem.IsLocalBuild;
        runningOnTeamCity = TeamCity.IsRunningOnTeamCity;
        runningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;

        Information("Execution environment:");
        Information($"Running Local build: {runningOnLocal}");
        Information($"Running on TeamCity: {runningOnTeamCity}");
        Information($"Running on AppVeyor: {runningOnAppVeyor}");

        EndBlock(blockText);
    });

#endregion

#region Public Method

// Start a log block for build systems that support it
public void StartBlock(string blockName)
{
		if(runningOnTeamCity)
		{
			TeamCity.WriteStartBlock(blockName);
		}
}

// Start a build block for build systems that support it
public void StartBuildBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteStartBuildBlock(blockName);
	}
}

// End a log block for build systems that support it
public void EndBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteEndBlock(blockName);
	}
}

// End a build block for build systems that support it
public void EndBuildBlock(string blockName)
{
	if(runningOnTeamCity)
	{
		TeamCity.WriteEndBuildBlock(blockName);
	}
}

// Push the Version number to the build system
public void PushVersion(string version)
{
	if(runningOnTeamCity)
	{
		TeamCity.SetBuildNumber(version);
	}
	if(runningOnAppVeyor)
	{
		Information("Pushing version to AppVeyor: " + version);
		AppVeyor.UpdateBuildVersion(version);
	}
}

// Push the Test Results to AppVeyor for display purposess
public void PushTestResults(string filePath)
{
	var file = MakeAbsolute(File(filePath));
	if(runningOnAppVeyor)
	{
		AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.NUnit3);
	}
}

#endregion