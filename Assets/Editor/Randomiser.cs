using UnityEngine;
using UnityEditor;

public class Randomiser : EditorWindow
{
    private bool randomX, randomY, randomZ;
    
    [MenuItem("CustomScripts/Randomize")]

    //Init window
    static void Init()
    {
        Randomiser window = (Randomiser)GetWindow(typeof(Randomiser));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Randomize selected objects", EditorStyles.boldLabel);

        randomX = EditorGUILayout.Toggle("Randomise X", randomX);
        randomY = EditorGUILayout.Toggle("Randomise Y", randomY);
        randomZ = EditorGUILayout.Toggle("Randomise Z", randomZ);

        if (GUILayout.Button("Randomise"))
        {
            foreach (GameObject go in Selection.gameObjects)
                go.transform.rotation = Quaternion.Euler(GetRandomRotations(go.transform.rotation.eulerAngles));
        }
    }

    private Vector3 GetRandomRotations(Vector3 currentRotation)
    {
        float x = randomX ? Random.Range(0f, 360f) : currentRotation.x;
        float y = randomY ? Random.Range(0f, 360f) : currentRotation.y;
        float z = randomZ ? Random.Range(0f, 360f) : currentRotation.z;

        return new Vector3(x, y, z);
    }

}
