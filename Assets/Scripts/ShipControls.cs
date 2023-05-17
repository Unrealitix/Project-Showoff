using System.Linq;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
	//Only controls and physics forces in this class!
	//Health and stuff like that should be in a separate class

	[SerializeField] private float thrust = 10f;
	[SerializeField] private float turnSpeed = 10f;
	[SerializeField] private float underwaterDrag = 0.5f;

	[Tooltip("To know when to apply the underwater drag")] [SerializeField] private PhysicMaterial waterMaterial;

	private Rigidbody _rigidbody;
	private MagLaser[] _magLasers;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_magLasers = GetComponentsInChildren<MagLaser>();
		if(waterMaterial == null)
			Debug.LogError(name + " ShipControls: waterMaterial is null!");
	}

	private void OnTriggerStay(Collider other)
	{
		//Underwater drag
		if (other.sharedMaterial == waterMaterial)
		{
			_rigidbody.AddForce(-_rigidbody.velocity * underwaterDrag);
		}
	}

	private void FixedUpdate()
	{
		//Friction
		float friction = _magLasers.Average(magLaser => magLaser.Friction);
		_rigidbody.AddForce(-_rigidbody.velocity * friction);

		//Input
		_rigidbody.AddForce(transform.forward * (Input.GetAxis("Vertical") * -thrust));
		_rigidbody.AddTorque(transform.up * (Input.GetAxis("Horizontal") * turnSpeed));
	}
}
