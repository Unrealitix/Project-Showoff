using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sound
{
	public class BackgroundMusic : MonoBehaviour
	{
		public static BackgroundMusic Instance;

		[SerializeField] private EventReference music;
		[SerializeField] private EventReference countdown;
		private EventInstance _music;
		private EventInstance _countdown;
		
		private bool _once = true;

		private float _raceProgress;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(this.gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		private void Start()
		{
			_music = RuntimeManager.CreateInstance(music);
			_countdown = RuntimeManager.CreateInstance(countdown);
		}

		private void Update()
		{
			if (Input.anyKeyDown && _once)
			{
				_music.start();
				_once = false;
			}
		}

		public void RaceProgressUpdate(int currentLap, int maxNumLaps)
		{
			// This function gets called every time a player finishes a lap.
			float progress = (float) currentLap / maxNumLaps;
			if (progress > _raceProgress)
			{
				_raceProgress = progress;
				Debug.Log($"TODO: Update music intensity: {_raceProgress}"); //TODO
			}
		}

		public void Countdown()
		{
			Debug.Log("TODO: Music stops being calm menu music, becomes race music"); //TODO
			_countdown.start();
			
		}

		public void ResetRaceProgress()
		{
			// This function gets called when a player crosses the finish line.
			// It can be called multiple times.
			_raceProgress = 0;
			Debug.Log("TODO: Reset music intensity"); //TODO
		}

		public void BackToMenu()
		{
			Debug.Log("TODO: Music back to calm menu music"); //TODO
		}
	}
}
