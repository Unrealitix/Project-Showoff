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

		public void Flying(bool isFlying)
		{
			if (!isFlying) return;
			Debug.Log($"TODO: Flying sound effect"); //TODO
		}

		public void Boost()
		{
			_boost.start();
		}

		public void Dash()
		{
			Debug.Log("Dash sound effect"); //TODO
		}

		public void DashRecharged()
		{
			Debug.Log("Dash has been recharged sound effect"); //TODO
		}

		public void RespawnByKillPlane()
		{
			Debug.Log("TODO: Respawn sound effect"); //TODO
		}

		public void RespawnManually()
		{
			Debug.Log("TODO: Respawn sound effect"); //TODO
		}

		public void EnterWater()
		{
			Debug.Log("TODO: Enter water sound effect"); //TODO
		}

		public void ExitWater()
		{
			Debug.Log("TODO: Exit water sound effect"); //TODO
		}

		public void Checkpoint()
		{
			_checkpoint.start();
		}

		public void Finish()
		{
			Debug.Log("TODO: Finish sound effect"); //TODO
		}
	}
}
