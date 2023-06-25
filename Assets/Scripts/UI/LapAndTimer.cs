using Checkpoints;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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

		public UnityEvent onFinish;

		//Time related variables
		private static float _lapTime;
		private float _bestTime;
		public float totalTime;

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
			if (other.TryGetComponent(out Checkpoint _))
			{
				currentLap.text = _currentLap.ToString();
				_startLap = true;

				if (_lapCp.nextCpNumber != 0) return;

				if (_currentLap == maxNumLaps)
				{
					onFinish.Invoke(); //TODO: Bind actions: Disable ship controls and enable the text input field on the winner
				}
				else
				{
					_currentLap++;
				}

				totalTime += _lapTime;
				bestTime.text = ShowTimer(totalTime);

				if (_lapTime < _bestTime || _bestTime == 0)
				{
					_bestTime = _lapTime;
				}

				_startLap = false;
				_lapTime = 0;
			}
		}

		//Method for converting the time in float in racing time (Minutes:Seconds:Fraction)
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
