using System;
using System.Linq;
using Generated;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Physics
{
	public class ShipControls : MonoBehaviour
	{
		//Only controls and physics forces in this class!
		//Health and stuff like that should be in a separate class

		[SerializeField] private string spawnLocationName = "Input Manager";

		[SerializeField] private float thrust = 10f;
		[SerializeField] private float turnSpeed = 10f;
		[SerializeField] private Transform centerOfMass;
		[SerializeField] private Transform engineForcePosition;

		[Header("Gravity")]
		[SerializeField] private float gravity = 9.81f;
		[SerializeField] private float trackGravity = 3.14f;

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

		private Rigidbody _rigidbody;
		private MagLaser[] _magLasers;

		private float _flightTimer;
		private Controls _controls;
		private (float vertical, float horizontal) _direction;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_magLasers = GetComponentsInChildren<MagLaser>();

			//Physics
			_rigidbody.centerOfMass = centerOfMass.localPosition;
			_rigidbody.useGravity = false; //we'll do it ourselves

			//Controls
			_controls = new Controls();
			_controls.Enable();

			//Spawn
			Transform spawn = GameObject.Find(spawnLocationName).transform;
			Vector3 spawnPosition = spawn.position;
			Quaternion spawnRotation = spawn.rotation;

			Transform parent = transform.parent;
			parent.position = spawnPosition;
			parent.rotation = spawnRotation;

			_rigidbody.position = spawnPosition;
			_rigidbody.rotation = spawnRotation;
		}

		public void OnMovement(InputValue value)
		{
			Vector2 direction = value.Get<Vector2>();

			_direction.vertical = direction.y;
			_direction.horizontal = direction.x;
		}

		private void OnTriggerEnter(Collider other)
		{
			//Underwater drag
			if (other.sharedMaterial == PhysicMaterialLibrary.Water)
			{
				_rigidbody.drag = underwaterDrag;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.sharedMaterial == PhysicMaterialLibrary.Water)
			{
				_rigidbody.drag = driveDrag;
			}
		}

		public void OnButtons()
		{

		}

		private void FixedUpdate()
		{
			Transform t = transform;
			Vector3 forward = t.forward;
			Vector3 right = t.right;
			Vector3 up = t.up;

			//Gravity
			bool track = _magLasers.Any(magLaser => magLaser.IsAttachedToTrack);
			float g = track ? trackGravity : gravity;
			_rigidbody.AddForce(Vector3.down * g);

			//Friction
			float friction = _magLasers.Average(magLaser => magLaser.GroundFriction);
			_rigidbody.AddForce(-_rigidbody.velocity * friction);

			bool attached = _magLasers.Any(magLaser => magLaser.IsAttached);

			//Input
			// Debug.Log($"Current Controller Input: Vertical({_direction.vertical}) | Horizontal({_direction.horizontal})");
			_rigidbody.AddTorque(up * (_direction.horizontal * turnSpeed)); //always just rotate around UP based on horizontal input
			if (attached)
			{
				_flightTimer = duration;
				_rigidbody.AddForceAtPosition(forward * (_direction.vertical * -thrust), engineForcePosition.position);
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
					switch (_direction.vertical)
					{
						case < 0f:
							_rigidbody.AddTorque(right * (_direction.vertical * -pitchSpeed * pitchFactor01));
							break;
						case > 0f:
							_rigidbody.AddTorque(right * (_direction.vertical * -pitchSpeed));
							break;
					}
				}
				else
				{
					switch (_direction.vertical)
					{
						case < 0f:
							_rigidbody.AddTorque(right * (_direction.vertical * -pitchSpeed));
							break;
						case > 0f:
							_rigidbody.AddTorque(right * (_direction.vertical * -pitchSpeed * pitchFactor01));
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
}
