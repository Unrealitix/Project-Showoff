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
		[SerializeField] private EventReference splash;
		[SerializeField] private EventReference dash;
		[SerializeField] private EventReference dashrecharged;

		private EventInstance _engine;
		private EventInstance _boost;
		private EventInstance _checkpoint;
		private EventInstance _splash;
		private EventInstance _dash;
		private EventInstance _dashrecharged;

		private void Start()
		{
			_engine = RuntimeManager.CreateInstance(engine);
			_boost = RuntimeManager.CreateInstance(boost);
			_checkpoint = RuntimeManager.CreateInstance(checkpoint);
			_splash = RuntimeManager.CreateInstance(splash);
			_dash = RuntimeManager.CreateInstance(dash);
			_dashrecharged = RuntimeManager.CreateInstance(dashrecharged);
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
			_dash.start();
		}

		public void DashRecharged()
		{
			_dashrecharged.start();
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
			_splash.start();
		}

		public void ExitWater()
		{
			_splash.start();
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
