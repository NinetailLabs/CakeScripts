/*
 * Restore NuGet packages using Paket
 */

#region Addins

#addin nuget:?package=Cake.Paket

#endregion

#region Tools

#tool nuget:?package=Paket&version=5.181.1

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