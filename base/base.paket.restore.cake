/*
 * Restore NuGet packages using Paket
 */

#region Addins

#addin nuget:?package=Cake.Paket&version=4.0.0

#endregion

#region Tools

#tool nuget:?package=Paket&version=5.242.2

#endregion

#region Tasks

// Restore NuGet packages using Paket
Task ("PaketRestore")
    .Does(() => {
        var blockText = "Paket Restore";
		StartBlock(blockText);
		
		PaketRestore();
		
		EndBlock(blockText);
    });

#endregion