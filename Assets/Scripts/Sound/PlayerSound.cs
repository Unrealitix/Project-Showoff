using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace Sound
{
	public class PlayerSound : MonoBehaviour
	{
		[SerializeField] private EventReference engine;
		[SerializeField] private EventReference boost;
		[SerializeField] private EventReference checkpoint;
		
		private EventInstance _engine;
		private EventInstance _boost;
		private EventInstance _checkpoint;

		private void Start()
		{
			_engine = RuntimeManager.CreateInstance(engine);
			_boost = RuntimeManager.CreateInstance(boost);
			_checkpoint = RuntimeManager.CreateInstance(checkpoint);
			_engine.start();
		}

		public void Acceleration(float acceleration)
		{
			_engine.setParameterByName("acceleration", (acceleration + 1) / 2 );
		}
	}
}
