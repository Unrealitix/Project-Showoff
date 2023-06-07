using System;
using System.Linq;
using UnityEngine;

public class ShipControls : MonoBehaviour
{
	//Only controls and physics forces in this class!
	//Health and stuff like that should be in a separate class

	[SerializeField] private float thrust = 10f;
	[SerializeField] private float turnSpeed = 10f;
	[SerializeField] private Transform centerOfMass;
	[SerializeField] private Transform engineForcePosition;

	[Header("Drag")]
	[SerializeField] private float driveDrag = 1;
	[SerializeField] private float underwaterDrag = 2f;
	[SerializeField] private float flightDrag = 0.5f;

	[Header("Flight")]
	[SerializeField] private float duration = 2f;
	[Tooltip("The factor of the Thrust when the duration has run out")] [SerializeField] private float lowestThrustFactor = 0.2f;
	[SerializeField] private float rollSpeed = 0.1f;
	[SerializeField] private float pitchSpeed = 10f;
	[SerializeField] private float maxPitch = 50f;
	[SerializeField] private float pitchCorrectionSpeed = 1f;

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

		_rigidbody.centerOfMass = centerOfMass.localPosition;
	}

	private void OnTriggerEnter(Collider other)
	{
		//Underwater drag
		if (other.sharedMaterial == waterMaterial)
		{
			_rigidbody.drag = underwaterDrag;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.sharedMaterial == waterMaterial)
		{
			_rigidbody.drag = driveDrag;
		}
	}

	private void FixedUpdate()
	{
		Transform t = transform;
		Vector3 forward = t.forward;
		Vector3 right = t.right;
		Vector3 up = t.up;

		//Friction
		float friction = _magLasers.Average(magLaser => magLaser.GroundFriction);
		_rigidbody.AddForce(-_rigidbody.velocity * friction);

		bool attached = _magLasers.Any(magLaser => magLaser.IsAttached);

		//Input
		float axisHorizontal = Input.GetAxis("Horizontal");
		float axisVertical = Input.GetAxis("Vertical");

		_rigidbody.AddTorque(up * (axisHorizontal * turnSpeed)); //always just rotate around UP based on horizontal input
		if (attached)
		{
			_flightTimer = duration;
			_rigidbody.AddForceAtPosition(forward * (axisVertical * -thrust), engineForcePosition.position);
			if (Math.Abs(_rigidbody.drag - underwaterDrag) > 0.01f) //Don't overwrite underwater drag
				_rigidbody.drag = driveDrag;
		}
		else
		{
			_rigidbody.drag = flightDrag;

			//==Thrust==
			_flightTimer -= Time.fixedDeltaTime;
			if (_flightTimer < 0f) _flightTimer = 0f;

			float flightFactor = Mathf.Clamp(_flightTimer / duration, lowestThrustFactor, 1.0f);
			// Debug.Log("Flight Factor: " + flightFactor);

			_rigidbody.AddForce(forward * (-thrust * flightFactor)); //Always full throttle

			//==Pitch==
			Vector3 localEulerAngles = t.localEulerAngles;
			float pitch = Mathf.DeltaAngle(localEulerAngles.x, 0f);
			float pitchFactor = pitch / maxPitch;
			float pitchFactor01 = Mathf.Clamp01(1.0f - Mathf.Abs(pitchFactor));
			if (pitch < 0f)
			{
				switch (axisVertical)
				{
					case < 0f:
						_rigidbody.AddTorque(right * (axisVertical * -pitchSpeed * pitchFactor01));
						break;
					case > 0f:
						_rigidbody.AddTorque(right * (axisVertical * -pitchSpeed));
						break;
				}
			}
			else
			{
				switch (axisVertical)
				{
					case < 0f:
						_rigidbody.AddTorque(right * (axisVertical * -pitchSpeed));
						break;
					case > 0f:
						_rigidbody.AddTorque(right * (axisVertical * -pitchSpeed * pitchFactor01));
						break;
				}
			}

			_rigidbody.AddTorque(right * (pitchFactor * pitchCorrectionSpeed)); //Center pitch to 0, mostly for cases of overshoot

			//==Roll==
			float roll = Mathf.DeltaAngle(localEulerAngles.z, 0f);
			_rigidbody.AddTorque(forward * (roll * rollSpeed)); //Roll to align UP
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(centerOfMass.position, 0.3f);
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(engineForcePosition.position, 0.3f);
	}
}
