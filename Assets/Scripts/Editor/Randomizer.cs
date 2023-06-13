using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class Randomizer : EditorWindow
	{
		private bool _randomX, _randomY, _randomZ;

		[MenuItem("CustomScripts/Randomize")]
		private static void Init()
		{
			Randomizer window = (Randomizer)GetWindow(typeof(Randomizer));
			window.Show();
		}

		private void OnGUI()
		{
			GUILayout.Label("Randomize selected objects", EditorStyles.boldLabel);

			_randomX = EditorGUILayout.Toggle("Randomize X", _randomX);
			_randomY = EditorGUILayout.Toggle("Randomize Y", _randomY);
			_randomZ = EditorGUILayout.Toggle("Randomize Z", _randomZ);

			if (GUILayout.Button("Randomize"))
			{
				foreach (GameObject go in Selection.gameObjects)
					go.transform.rotation = Quaternion.Euler(GetRandomRotations(go.transform.rotation.eulerAngles));
			}
		}

		private Vector3 GetRandomRotations(Vector3 currentRotation)
		{
			float x = _randomX ? Random.Range(0f, 360f) : currentRotation.x;
			float y = _randomY ? Random.Range(0f, 360f) : currentRotation.y;
			float z = _randomZ ? Random.Range(0f, 360f) : currentRotation.z;

			return new Vector3(x, y, z);
		}
	}
}
