using System;
using System.IO;
using UnityEngine;
using UnityEngine.IO;

public class BestTimesTable : MonoBehaviour
{
    private String _allText;
    private String[] _separateText;
    private void Awake()
    {
        _allText = File.ReadAllText(Application.dataPath + "/totalTime.txt");
        _separateText = _allText.Split(char.Parse(","));
        foreach (string text in _separateText)
        {
            Debug.Log(text);
        }
    }
}
