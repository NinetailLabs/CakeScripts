/*
 * Execute method required to push results to SonarQube.
 */

#region Addins

#addin nuget:?package=Cake.Sonar&version=1.1.25

#endregion

#region Tools

#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0

#endregion

#region Variables

// SonarQube server URL
var sonarQubeServerUrl = "";
// SonarQube project name
var sonarQubeProject = "";
// The secret key for the SonarQube project
var sonarQubeKey = "";

#endregion

#region Tasks

// Run this before MSBuild start if SonarQube analysis is required
Task("SonarQubeStartup")
	.Does(() =>{
		StartBlock("SonarQube Startup");

		var coveragePath = MakeAbsolute(File(coverPath));
		
		SonarBegin(new SonarBeginSettings{
			Url = sonarQubeServerUrl,
			Verbose = true,
			Key = sonarQubeKey,
			Name = sonarQubeProject,
			OpenCoverReportsPath = coveragePath.ToString(),
			NUnitReportsPath = testResultFile
		});

		EndBlock("SonarQube Startup");
	});

// Run this once unit testing is completed to execute SonarQube analysis
Task("SonarQubeShutdown")	
	.Does(() => {
		StartBlock("SonarQube Shutdown");
		
		SonarEnd(new SonarEndSettings{});
		
		EndBlock("SonarQube Shutdown");
	});

#endregion