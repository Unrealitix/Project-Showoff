using Checkpoints;
using Sound;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI.PerPlayer
{
	public class LapAndTimer : MonoBehaviour
	{
		//Objects for the Time and Best/Total time
		[SerializeField] private TMP_Text timer;
		[SerializeField] private TMP_Text bestTime;
		private string _timerTemplate;

		//Objects for the laps UI
		[SerializeField] private TMP_Text currentLap;
		[SerializeField] private TMP_Text maxLaps;

		//Lap related variables
		private int _currentLap;
		[SerializeField] private int maxNumLaps = 2;

		public UnityEvent<float> onFinish;

		//Time related variables
		private float _lapTime;
		private float _bestTime;
		public float totalTime;
		private int _prevCheckpoint;

		private CheckpointTracker _lapCp;

		[HideInInspector] public bool startLap;

		private void Awake()
		{
			_lapCp = GetComponent<CheckpointTracker>();
			maxLaps.text = $"/{maxNumLaps}";
			_timerTemplate = timer.text;
			timer.text = string.Format(_timerTemplate, ShowTimer(0));
		}

		private void Update()
		{
			if (startLap)
			{
				_lapTime += Time.deltaTime;
				timer.text = string.Format(_timerTemplate, ShowTimer(_lapTime));
			}
		}

		public void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Checkpoint _))
			{
				startLap = true;

				if (_lapCp.NextCpNumber != 1) return;
				if (_prevCheckpoint == _lapCp.NextCpNumber) return;

				if (_currentLap != 0)
				{
					totalTime += _lapTime;
					bestTime.text = ShowTimer(totalTime);

					if (_lapTime < _bestTime || _bestTime == 0)
					{
						_bestTime = _lapTime;
					}

					startLap = false;
					_lapTime = 0;
					startLap = true;
				}

				if (_currentLap == maxNumLaps)
				{
					onFinish.Invoke(totalTime);
					BackgroundMusic.Instance.ResetRaceProgress();
					startLap = false;
				}
				else
				{
					_currentLap++;
					BackgroundMusic.Instance.RaceProgressUpdate(_currentLap, maxNumLaps);
				}

				currentLap.text = _currentLap.ToString();
			}

			_prevCheckpoint = _lapCp.NextCpNumber;
		}

		//Method for converting the time in float in racing time (Minutes:Seconds.Fraction)
		public static string ShowTimer(float time)
		{
			int intTime = (int) time;
			int seconds = intTime % 60;
			int minutes = intTime / 60;
			float fraction = time * 1000 % 1000;
			string timeText = $"{minutes:00}:{seconds:00}.{fraction:000}";
			return timeText;
		}
	}
}
