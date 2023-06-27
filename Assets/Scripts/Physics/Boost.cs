using UnityEngine;

namespace Physics
{
	[RequireComponent(typeof(Collider))]
	public class Boost : MonoBehaviour
	{
		private ParticleSystem _ps;

		private void Awake()
		{
			GetComponent<Collider>().isTrigger = true;
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			_ps = GetComponent<ParticleSystem>();
		}

		private void OnTriggerEnter(Collider other)
		{
			_ps.Play();
		}
	}
}
