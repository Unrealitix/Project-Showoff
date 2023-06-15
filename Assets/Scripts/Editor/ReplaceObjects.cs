using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Editor
{
	public class ReplaceObjects : EditorWindow
	{
		private int _size = 1;
		public List<GameObject> gameObjectList = new List<GameObject>();

		[MenuItem("CustomTools/Replace Object")]
		private static void Init()
		{
			ReplaceObjects window = (ReplaceObjects)GetWindow(typeof(ReplaceObjects), false, "Replace Object");
			window.Show();
			
		}
		
		private void OnGUI()
		{
			//Selection information display
			GUILayout.Label("Replace selected object(s) with assets", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Currently selected object(s): " + Selection.objects.Length);
			GUILayout.Space(10);
			
			//List display
			GUILayout.Label("List of replacement assets", EditorStyles.boldLabel);
			GUILayout.Space(10);
			
			_size = Mathf.Max(0,EditorGUILayout.IntField("Amount", gameObjectList.Count));
			
			while (_size > gameObjectList.Count)
			{
				gameObjectList.Add(null);
			}
			
			while (_size < gameObjectList.Count)
			{
				gameObjectList.RemoveAt(gameObjectList.Count -1);
			}

			//display list items
			for (var i = 0; i < gameObjectList.Count; i++)
			{
				GUILayout.Space(2);
				EditorGUI.indentLevel++;
				GUILayout.BeginHorizontal();
				
				gameObjectList [i] = EditorGUILayout.ObjectField("Asset " + i,gameObjectList[i], typeof(GameObject), false) 
					as GameObject;
				
				if (GUILayout.Button("Replace", GUILayout.MaxWidth(80)))
					ReplaceWithPrefab(gameObjectList[i]);
				
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
				
			}
			
			GUILayout.Space(10);
			if (gameObjectList.Count == 0)
				GUI.enabled = false;
			if (GUILayout.Button("Replace with random asset from list"))
				ReplaceWithRandom();
			
		}

		private static void ReplaceWithPrefab(GameObject prefab)
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

		private void ReplaceWithRandom()
		{
			var selection = Selection.gameObjects;

			for (var i = selection.Length - 1; i >= 0; --i)
			{
				GameObject selected = selection[i];
				GameObject newObject;
				GameObject prefab = gameObjectList[Random.Range(0, gameObjectList.Count)];

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
