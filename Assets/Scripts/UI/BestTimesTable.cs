using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BestTimesTable : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] names;
        [SerializeField] private TMP_Text[] times;

        private void Awake()
        {
            int switcher = 0;
            string allText = File.ReadAllText(Application.dataPath + "/totalTime.txt");
            string[] separateText = allText.Split(char.Parse("\n"),StringSplitOptions.RemoveEmptyEntries);

            List<string> nameAndTimeTexts = separateText.SelectMany(registry => registry.Split(char.Parse(","), StringSplitOptions.RemoveEmptyEntries)).ToList();

            for (int i = 1; i < nameAndTimeTexts.Count; i+=2)
            {
                for (int j = i+2; j < nameAndTimeTexts.Count; j+=2)
                {
                    if (!(float.Parse(nameAndTimeTexts[j], CultureInfo.InvariantCulture.NumberFormat) <
                          float.Parse(nameAndTimeTexts[i], CultureInfo.InvariantCulture.NumberFormat))) continue;
                    (nameAndTimeTexts[i], nameAndTimeTexts[j]) = (nameAndTimeTexts[j], nameAndTimeTexts[i]);
                    (nameAndTimeTexts[i-1], nameAndTimeTexts[j-1]) = (nameAndTimeTexts[j-1], nameAndTimeTexts[i-1]);
                }
            }

            for (int i = 0; i < (names.Length + times.Length); i+=2)
            {
                names[switcher].text = nameAndTimeTexts[i];
                times[switcher].text = LapAndTimer.ShowTimer(float.Parse(nameAndTimeTexts[i+1], CultureInfo.InvariantCulture.NumberFormat));
                switcher++;
            }
        }
    }
}
