using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class ReplaceObjects : EditorWindow
	{
		[SerializeField] private GameObject prefab1;
		[SerializeField] private GameObject prefab2;


		[MenuItem("CustomTools/Replace Object")]
		private static void Init()
		{
			ReplaceObjects window = (ReplaceObjects)GetWindow(typeof(ReplaceObjects), false, "Replace Object");
			window.Show();
		}

		private void OnGUI()
		{
			GUILayout.Label("Replace selected object(s) with Prefabs", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Currently selected object(s): " + Selection.objects.Length);
			
			prefab1 = (GameObject)EditorGUILayout.ObjectField("Prefab 1", prefab1, typeof(GameObject), false);
			prefab2 = (GameObject)EditorGUILayout.ObjectField("Prefab 2", prefab2, typeof(GameObject), false);

			if (GUILayout.Button("Replace with Prefab 1"))
				ReplaceWithPrefab(prefab1);
			

			if (GUILayout.Button("Replace with Prefab 2"))
				ReplaceWithPrefab(prefab2);

			if (GUILayout.Button("Replace with random Prefab"))
				ReplaceWithRandom(prefab1, prefab2);

		}

		private void ReplaceWithPrefab(GameObject prefab)
		{
			var selection = Selection.gameObjects;

			for (var i = selection.Length - 1; i >= 0; --i)
			{
				GameObject selected = selection[i];
				PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefab);
				GameObject newObject;
				
				//spawn new object
				if (prefabType == PrefabAssetType.Regular)
				{
					newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				}
				else
				{
					newObject = Instantiate(prefab);
					newObject.name = prefab.name;
				}
				
				
				//Add new object to undo, copy transforms and destroy old object
				Undo.RegisterCreatedObjectUndo(newObject, "Replace Object");
				newObject.transform.parent = selected.transform.parent;
				newObject.transform.localPosition = selected.transform.localPosition;
				newObject.transform.localRotation = selected.transform.localRotation;
				newObject.transform.localScale = selected.transform.localScale;
				newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
				Undo.DestroyObjectImmediate(selected);

			}

		}

		private void ReplaceWithRandom(GameObject object1, GameObject object2)
		{
			var selection = Selection.gameObjects;

			for (var i = selection.Length - 1; i >= 0; --i)
			{
				GameObject selected = selection[i];
				GameObject newObject;
				
				var choice =Random.Range(0,2);
				GameObject prefab = choice == 0 ? object1 : object2;
				
				PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefab);

				//spawn new object
				if (prefabType == PrefabAssetType.Regular)
				{
					newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				}
				else
				{
					newObject = Instantiate(prefab);
					newObject.name = prefab.name;
				}


				//Add new object to undo, copy transforms and destroy old object
				Undo.RegisterCreatedObjectUndo(newObject, "Replace Object");
				newObject.transform.parent = selected.transform.parent;
				newObject.transform.localPosition = selected.transform.localPosition;
				newObject.transform.localRotation = selected.transform.localRotation;
				newObject.transform.localScale = selected.transform.localScale;
				newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
				Undo.DestroyObjectImmediate(selected);
				

			}
		}
	}
}
