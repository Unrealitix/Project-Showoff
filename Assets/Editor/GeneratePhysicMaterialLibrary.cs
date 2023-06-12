using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Editor
{
	public class GeneratePhysicMaterialLibrary : AssetPostprocessor
	{
		private const string SUFFIX = ".physicMaterial";
		private const string GENERATED_FILE = "Assets/Scripts/Generated/PhysicMaterialLibrary.cs";
		private const string TEMPLATE = @"// This file was automatically generated.
// Any modifications you do to this file will be overwritten.

using UnityEditor;
using UnityEngine;

namespace Generated
{
	public static class PhysicMaterialLibrary
	{
//here
	}
}";

		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths
		)
		{
			if (importedAssets.Any(s => s.EndsWith(SUFFIX)) ||
			    deletedAssets.Any(s => s.EndsWith(SUFFIX)) ||
			    movedAssets.Any(s => s.EndsWith(SUFFIX)) ||
			    movedFromAssetPaths.Any(s => s.EndsWith(SUFFIX)))
			{
				Generate();
			}
		}

		private static void Generate()
		{
			string[] guids = AssetDatabase.FindAssets("t:PhysicMaterial");
			StringBuilder insert = new();
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				string name = Path.GetFileNameWithoutExtension(path);
				insert.AppendLine($"\t\tpublic static PhysicMaterial {name} => AssetDatabase.LoadAssetAtPath<PhysicMaterial>(\"{path}\");");
			}

			File.WriteAllText(GENERATED_FILE, TEMPLATE.Replace("//here", insert.ToString()));
			AssetDatabase.Refresh();
		}
	}
}
