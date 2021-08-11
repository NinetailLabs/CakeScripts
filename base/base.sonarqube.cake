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

// The secret key for the SonarQube project
var sonarQubeKey = "";
// The branch that is being analyzed
var sonarBranch = "";
// The organization that the code is being analyzed for
var sonarOrganization = "";
// SonarQube server URL
var sonarQubeServerUrl = "";
// The login token for the analyzer account
var sonarLogin = "";

#endregion

#region Tasks

// Run this before MSBuild start if SonarQube analysis is required
Task("SonarQubeStartup")
	.Does(() =>{
		StartBlock("SonarQube Startup");

		var coveragePath = MakeAbsolute(File(coverPath));
		
		SonarBegin(new SonarBeginSettings
		{
			Key = sonarQubeKey,
			Branch = sonarBranch,
			Organization = sonarOrganization,
			Url = sonarQubeServerUrl,
			Verbose = true,
			OpenCoverReportsPath = coveragePath.ToString(),
			Login = sonarLogin
		});

		EndBlock("SonarQube Startup");
	});

// Run this once unit testing is completed to execute SonarQube analysis
Task("SonarQubeShutdown")	
	.Does(() => {
		StartBlock("SonarQube Shutdown");
		
		SonarEnd(new SonarEndSettings
		{
			Login = sonarLogin
		});
		
		EndBlock("SonarQube Shutdown");
	});

#endregion