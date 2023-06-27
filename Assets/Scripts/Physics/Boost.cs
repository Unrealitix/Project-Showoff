using System;
using UnityEngine;

namespace Physics
{
	[RequireComponent(typeof(Collider))]
	public class Boost : MonoBehaviour
	{
		private ParticleSystem ps;

		private void Awake()
		{
			GetComponent<Collider>().isTrigger = true;
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
			ps = GetComponent<ParticleSystem>();
		}

		private void OnTriggerEnter()
		{
			ps.Play();
		}

	}
}
