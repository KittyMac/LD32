/*
using UnityEditor;
using System.Diagnostics;

public class CopyHiddenResources 
{
    [MenuItem("MyTools/Windows Build With Postprocess")]
    public static void BuildGame ()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        string[] levels = new string[] {"Assets/Scene1.unity", "Assets/Scene2.unity"};

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/BuiltGame.exe", BuildTarget.StandaloneWindows, BuildOptions.None);

        // Copy a file from the project folder to the build folder, alongside the built game.
        FileUtil.CopyFileOrDirectory("Assets/WebPlayerTemplates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        Process proc = new Process();
        proc.StartInfo.FileName = path + "BuiltGame.exe";
        proc.Start();
    }
}
*/

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

// /Volumes/Development/smallplanet/domvs_Prototype/Builds/iOS_Enterprise
using System.IO;
using System;
	
public class CopyHiddenResources {
	
	[PostProcessBuildAttribute(1)]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
		Debug.Log (pathToBuiltProject);

		Copy (pathToBuiltProject + "/../../Movies/", pathToBuiltProject + "/Data/Raw/");

	}

	public static void Copy(string sourceDir, string targetDir)
	{
		Directory.CreateDirectory (targetDir);

		foreach (var file in Directory.GetFiles(sourceDir)) {
			try{
				FileUtil.CopyFileOrDirectory(file, Path.Combine (targetDir, Path.GetFileName (file)));
			}catch(IOException e){

			}
		}

		foreach (var directory in Directory.GetDirectories(sourceDir)) {
			FileUtil.CopyFileOrDirectory(directory, Path.Combine (targetDir, Path.GetFileName (directory)));
		}
	}
}