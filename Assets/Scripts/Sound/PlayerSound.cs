using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sound
{
	public class PlayerSound : MonoBehaviour
	{
		[SerializeField] private EventReference engine;
		private EventInstance _engine;

		private void Start()
		{
			_engine = RuntimeManager.CreateInstance(engine);
			_engine.start();
		}

		public void OnAcceleration(float acceleration)
		{
			_engine.setParameterByName("acceleration", acceleration);
		}
	}
}
