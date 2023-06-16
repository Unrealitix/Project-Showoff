using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sound
{
	public class BackgroundMusic : MonoBehaviour
	{
	   [SerializeField] private EventReference music;
		private EventInstance _music;
		private bool _once = true;

		private void Start()
		{
			_music = RuntimeManager.CreateInstance(music);
		}

		private void Update()
		{
			if (Input.anyKeyDown && _once) {
				_music.start();
				_once = false;
			}
		}
	}
}
