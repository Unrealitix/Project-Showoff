using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class GeneratePhysicMaterialLibrary : AssetPostprocessor
	{
		private const string SUFFIX = ".physicMaterial";

		private const string GENERATED_CODE_PATH = "Assets/Scripts/Generated/PhysicMaterialLibrary.cs";
		private const string CODE_TEMPLATE = @"// This file and its accompanying .meta file were automatically generated.
// Any modifications you do to either of them will be overwritten.

using UnityEngine;

namespace Generated
{
	public class PhysicMaterialLibrary : ScriptableObject
	{
		private static PhysicMaterialLibrary _instance;
		public static void Init()
		{
			if (_instance == null)
				_instance = Resources.Load<PhysicMaterialLibrary>(""PhysicMaterialLibrary"");
		}
";
		private const string CODE_LINE_TEMPLATE = @"
		public static PhysicMaterial {0} => _instance.prop{0};
		[SerializeField] private PhysicMaterial prop{0};
";

		private const string GENERATED_ASSET_PATH = "Assets/Scripts/Generated/Resources/PhysicMaterialLibrary.asset";
		private const string ASSET_TEMPLATE = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: c3fd5956597cab9dc8683d072d84ec79, type: 3}
  m_Name: PhysicMaterialLibrary
  m_EditorClassIdentifier: 
";

		private const string ASSET_LINE_TEMPLATE = "  prop[name]: {fileID: 13400000, guid: [guid], type: 2}";

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
			StringBuilder code = new(CODE_TEMPLATE);
			StringBuilder asset = new(ASSET_TEMPLATE);
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				string name = Path.GetFileNameWithoutExtension(path);
				if (name.Contains(" "))
				{
					Debug.LogError($"PhysicMaterial name '{name}' contains spaces. This is not allowed. Please rename the PhysicMaterial. Aborting code generation.");
					return;
				}

				code.AppendFormat(CODE_LINE_TEMPLATE, name);
				asset.AppendLine(ASSET_LINE_TEMPLATE.Replace("[name]", name).Replace("[guid]", guid));
			}

			code.AppendLine("\t}\n}");

			File.WriteAllText(GENERATED_CODE_PATH, code.ToString());
			File.WriteAllText(GENERATED_ASSET_PATH, asset.ToString());
			AssetDatabase.Refresh();
		}
	}
}
