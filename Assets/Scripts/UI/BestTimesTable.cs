using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class BestTimesTable : MonoBehaviour
{
    [SerializeField] private TMP_Text[] names;
    [SerializeField] private TMP_Text[] times;
    
    private void Awake()
    {
        int switcher = 0;
        List<string> nameAndTimeTexts = new List<string>();
        string allText = File.ReadAllText(Application.dataPath + "/totalTime.txt");
        string[] separateText = allText.Split(char.Parse("\n"),StringSplitOptions.RemoveEmptyEntries);
        
        foreach (string registry in separateText)
        {
            string[] nameAndTime = registry.Split(char.Parse(","),StringSplitOptions.RemoveEmptyEntries);
            foreach (string text in nameAndTime)
            {
                nameAndTimeTexts.Add(text);
            }
        }

        for (int i = 0; i < nameAndTimeTexts.Count; i+=2)
        {
            names[switcher].text = nameAndTimeTexts[i];
            times[switcher].text = nameAndTimeTexts[i + 1];
            switcher++;
        }
    }
}
