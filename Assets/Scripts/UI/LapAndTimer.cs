using Checkpoints;
using TMPro;
using UnityEngine;
using System.IO;

namespace UI
{
	public class LapAndTimer : MonoBehaviour
	{
		//Objects for the Time and Best/Total time
		[SerializeField] private TMP_Text timer;
		[SerializeField] private TMP_Text bestTime;

		//Objects for the laps UI
		[SerializeField] private TMP_Text currentLap;
		[SerializeField] private TMP_Text maxLaps;

		//Lap related variables
		private int _currentLap;
		[SerializeField] private int maxNumLaps = 2;

		//Time related variables
		private float _lapTime;
		private float _bestTime;
		private float _totalTime;

		private CheckpointTracker _lapCp;

		private bool _startLap;

		private void Awake()
		{
			_lapCp = GetComponent<CheckpointTracker>();
			maxLaps.text = maxNumLaps.ToString();
		}

		private void Update()
		{
			if (_startLap)
			{
				_lapTime += Time.deltaTime;
				timer.text = ShowTimer(_lapTime);
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Checkpoint checkpoint))
			{
				currentLap.text = _currentLap.ToString();
				_startLap = true;
				if (_lapCp.nextCpNumber != 0) return;

				_currentLap++;
				_totalTime += _lapTime;
				bestTime.text = ShowTimer(_totalTime);

				if (_lapTime < _bestTime || _bestTime == 0)
				{
					_bestTime = _lapTime;
					//bestTime.text = ShowTimer(_bestTime);
				}

				if (_currentLap > maxNumLaps)
				{
					File.AppendAllText(Application.dataPath + "/totalTime.txt", ShowTimer(_totalTime) + "\n");
				}

				_startLap = false;
				_lapTime = 0;
			}
		}

		//Method for converting the time in float in racing time (Minutes:Seconds:Fraction)
		private string ShowTimer(float time)
		{
			int intTime = (int) time;
			int seconds = intTime % 60;
			int minutes = intTime / 60;
			float fraction = _lapTime * 1000 % 1000;
			string timeText = $"{minutes:00}:{seconds:00}.{fraction:000}";
			return timeText;
		}
	}
}
