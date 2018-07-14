/*
 * Generate Release Notes from Git issues
 */

#region Addins

#addin nuget:?package=Cake.FileHelpers

#endregion

#region Tools

#tool nuget:?package=GitReleaseNotes

#endregion

#region Variables

// Release notes
var releaseNotesText = "";
// Path where release notes should be generated
var releaseNotes = "./ReleaseNotes.md";

#endregion

#region Tasks

// Generates Release notes
Task ("GenerateReleaseNotes")
	.Does (() => {
        var blockText = "GeneratingReleaseNotes";
        StartBlock(blockText);

		var releasePath = MakeAbsolute(File(releaseNotes));
		GitReleaseNotes(releasePath, GetReleaseNoteSettings());

		releaseNotesText = FileReadText(releasePath);
		Information(releaseNotesText);

        EndBlock(blockText);
	});

#endregion

#region Private Methods

// Generates settings used by GitReleaseNotes
private GitReleaseNotesSettings GetReleaseNoteSettings()
{
    return new GitReleaseNotesSettings
    {
        WorkingDirectory = ".",
        Verbose = true,
        Version = version,
        AllLabels = true
    };
}

#endregion