#define ALT_FLIGHT_SCHEME

using System.Linq;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
	//Only controls and physics forces in this class!
	//Health and stuff like that should be in a separate class

	[SerializeField] private float thrust = 10f;
	[SerializeField] private float turnSpeed = 10f;
	[SerializeField] private float flightRollSpeed = 0.1f;
	[SerializeField] private float flightDuration = 2f;
	[SerializeField] private float underwaterDrag = 0.5f;

	[Tooltip("To know when to apply the underwater drag")] [SerializeField] private PhysicMaterial waterMaterial;

	private Rigidbody _rigidbody;
	private MagLaser[] _magLasers;

	private float _flightTimer;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_magLasers = GetComponentsInChildren<MagLaser>();
		if (waterMaterial == null)
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
		Transform t = transform;
		Vector3 forward = t.forward;
		Vector3 right = t.right;
		Vector3 up = t.up;

		//Friction
		float friction = _magLasers.Average(magLaser => magLaser.Friction);
		_rigidbody.AddForce(-_rigidbody.velocity * friction);

		bool attached = _magLasers.Any(magLaser => magLaser.IsAttached);
		//Input
		_rigidbody.AddTorque(up * (Input.GetAxis("Horizontal") * turnSpeed)); //always just rotate around UP based on horizontal input
		if (attached)
		{
			_flightTimer = flightDuration;
			_rigidbody.AddForce(forward * (Input.GetAxis("Vertical") * -thrust));
		}
		else
		{
			_flightTimer -= Time.fixedDeltaTime;
			if (_flightTimer < 0f) _flightTimer = 0f;

			float flightFactor = _flightTimer / flightDuration;
			Debug.Log(flightFactor);

#if ALT_FLIGHT_SCHEME
			_rigidbody.AddForce(forward * (-thrust * flightFactor)); //Always full throttle
			_rigidbody.AddTorque(right * (Input.GetAxis("Vertical") * -turnSpeed)); //Pitch based on input

			_rigidbody.AddTorque(forward * (Mathf.DeltaAngle(t.localEulerAngles.z, 0f) * flightRollSpeed)); //Roll to align UP
#else
			_rigidbody.AddForce(forward * (Input.GetAxis("Vertical") * -thrust * flightFactor)); //Throttle based on input

			Vector3 localEulerAngles = t.localEulerAngles;
			_rigidbody.AddTorque(forward * (Mathf.DeltaAngle(localEulerAngles.z, 0f) * flightRollSpeed)); //Roll to align UP
			_rigidbody.AddTorque(right * (Mathf.DeltaAngle(localEulerAngles.x, 0f) * flightRollSpeed)); //Pitch to align UP
#endif
		}
	}
}
