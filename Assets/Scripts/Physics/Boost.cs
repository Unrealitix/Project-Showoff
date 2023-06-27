using UnityEngine;

namespace Physics
{
	[RequireComponent(typeof(Collider))]
	public class Boost : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Collider>().isTrigger = true;
		}
	}
}
