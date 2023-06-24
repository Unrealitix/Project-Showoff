using TMPro;
using UnityEngine;
using System.IO;

namespace UI
{
	public class SaveTrackTime : MonoBehaviour
	{
		[SerializeField] private LapAndTimer lapAndTimer;
		private TMP_InputField _inputField;
		private string _playerTime;

		private void Awake()
		{
			_inputField = GetComponent<TMP_InputField>();
			_inputField.onValueChanged.AddListener(text => { _inputField.text = text.ToUpper(); });
			_inputField.onEndEdit.AddListener(SaveScore);
		}

		private void SaveScore(string text)
		{
			if (text.Length != 3) return;
			_playerTime = text + "," + lapAndTimer.totalTime;
			File.AppendAllText(Application.dataPath + "/totalTime.txt", _playerTime + "\n");
		}
	}
}
