using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;


[InitializeOnLoad]
[ExecuteInEditMode]
class AddProjectScenes : AssetPostprocessor
{
	static AddProjectScenes ()
	{
		ReloadBuildScenes ();
	}

	static void ReloadBuildScenes ()
	{
		//Debug.Log ("AddProjectScenes");

		List<string> SceneFiles = new List<string> ();

		DirectoryInfo dir = new DirectoryInfo (Application.dataPath + "/Scenes");
		FileInfo[] info = dir.GetFiles("*.*");
		foreach (FileInfo fileInfo in info)  {
			if (fileInfo.FullName.EndsWith (".unity")) {
				SceneFiles.Add (fileInfo.Name);
			}
		}

		SceneFiles.Sort(delegate(string x, string y)
			{
				int indexX = 99;
				int indexY = 99;

				if(x.Contains("_")){
					indexX = int.Parse(x.Substring(0, x.IndexOf("_")));
				}

				if(y.Contains("_")){
					indexY = int.Parse(y.Substring(0, y.IndexOf("_")));
				}

				return indexX.CompareTo(indexY);
			});

		EditorBuildSettingsScene[] newSettings = new EditorBuildSettingsScene[SceneFiles.Count];

		int index = 0;
		foreach (string sceneFile in SceneFiles) {
			newSettings [index] = new EditorBuildSettingsScene ("Assets/Scenes/" + sceneFile, true);
			index++;
		}

		EditorBuildSettings.scenes = newSettings;
	}


	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
	{
		ReloadBuildScenes ();
	}
}