using System;
using TMPro;
using UnityEngine;
using System.IO;

namespace UI
{
   public class SaveTrackTime : MonoBehaviour
   {
      private LapAndTimer _lapAndTimer;
      private TMP_InputField _inputField;
      private String _playerTime;

      private void Awake()
      {
         GameObject ship = GameObject.Find("ship");
         _lapAndTimer = ship.GetComponent<LapAndTimer>();
         _inputField = GetComponent<TMP_InputField>();
      }

      public void SaveScore()
      {
         if (_inputField.text.Length != 3) return;
         _playerTime = _inputField.text + "/" + _lapAndTimer.ShowTimer(_lapAndTimer.totalTime) + "/";
         File.AppendAllText(Application.dataPath + "/totalTime.txt", _playerTime + "\n");
         Debug.Log("YA");
      }
   }
}
