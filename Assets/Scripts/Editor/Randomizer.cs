using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class Randomizer : EditorWindow
	{
		private bool _randomX, _randomY, _randomZ;

		[MenuItem("CustomTools/Randomize Rotation")]
		private static void Init()
		{
			Randomizer window = (Randomizer)GetWindow(typeof(Randomizer), false, "Randomize Rotation");
			window.Show();
		}

		private void OnGUI()
		{
			GUILayout.Label("Randomize selected objects", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Currently selected objects: " + Selection.objects.Length);

			_randomX = EditorGUILayout.Toggle("Randomize X", _randomX);
			_randomY = EditorGUILayout.Toggle("Randomize Y", _randomY);
			_randomZ = EditorGUILayout.Toggle("Randomize Z", _randomZ);

			if (GUILayout.Button("Randomize Rotation"))
			{
				foreach (GameObject gameObject in Selection.gameObjects)
					gameObject.transform.rotation = Quaternion.Euler(GetRandomRotations(gameObject.transform.rotation.eulerAngles));
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
