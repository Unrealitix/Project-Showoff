using System;
using System.Collections;
using System.Linq;
using Checkpoints;
using Generated;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Physics
{
	public class ShipControls : MonoBehaviour
	{
		//Only controls and physics forces in this class!
		//Health and stuff like that should be in a separate class

		[SerializeField] private string spawnLocationName;

		[SerializeField] private float thrust = 10f;
		[SerializeField] private float turnSpeed = 10f;
		[Tooltip("You need to manually measure this and fill it in!")] [SerializeField] private float maxSpeed = 10f;

		[Header("Countdown")]
		[SerializeField] private int countdownSeconds = 5;
		[SerializeField] private TMP_Text countdown;

		[SerializeField] private Transform centerOfMass;
		[SerializeField] private Transform engineForcePosition;

		[Header("Boost")]
		[SerializeField] private float boostThrust = 20f;
		[SerializeField] private float boostDuration = 1f;


		[Header("Gravity")]
		[SerializeField] private float gravity = 9.81f;
		[SerializeField] private float trackGravity = 3.14f;

		[Header("Drag")]
		[SerializeField] private float driveDrag = 1;
		[SerializeField] private float underwaterDrag = 2f;
		[SerializeField] private float flightDrag = 0.5f;

		[Header("Flight")]
		[SerializeField] private float rollSpeed = 0.1f;
		[SerializeField] private float pitchSpeed = 10f;
		[SerializeField] private float maxPitch = 50f;
		[SerializeField] private float pitchCorrectionSpeed = 1f;

		public UnityEvent<float> isAccelerating;
		public UnityEvent enterWater;
		public UnityEvent exitWater;

		public float MaxSpeed => maxSpeed;

		private Rigidbody _rigidbody;
		private MagLaser[] _magLasers;

		private float _currentThrust;
		private Controls _controls;
		private (float vertical, float horizontal, float acceleration) _controllerInput;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_magLasers = GetComponentsInChildren<MagLaser>();

			//Physics
			_currentThrust = thrust;
			_rigidbody.centerOfMass = centerOfMass.localPosition;
			_rigidbody.useGravity = false; //we'll do it ourselves
			PhysicMaterialLibrary.Init();

			//Controls
			_controls = new Controls();
			_controls.Enable();

			//Spawn
			if (!string.IsNullOrWhiteSpace(spawnLocationName))
			{
				Spawn(GameObject.Find(spawnLocationName).transform, null);
			}

			//Finish
			GetComponent<LapAndTimer>().onFinish.AddListener(OnFinish);
		}

		public void Spawn(Transform spawn, UnityEvent onStart)
		{
			Vector3 spawnPosition = spawn.position;
			Quaternion spawnRotation = spawn.rotation;

			Transform parent = transform.parent;
			parent.position = spawnPosition;
			parent.rotation = spawnRotation;

			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.position = spawnPosition;
			_rigidbody.rotation = spawnRotation;

			if (onStart != null)
			{
				Freeze();
				onStart.AddListener(StartCountdown);
			}
		}

		private void StartCountdown()
		{
			StartCoroutine(RunCountdown());
		}

		private IEnumerator RunCountdown()
		{
			for (int i = countdownSeconds; i >= 1; i--)
			{
				countdown.text = $"{i:D}";
				yield return new WaitForSeconds(1f);
			}

			countdown.text = "Go!";
			Unfreeze();
			GetComponent<LapAndTimer>().startLap = true;
			yield return new WaitForSeconds(1f);
			countdown.gameObject.SetActive(false);
		}

		private void Freeze()
		{
			_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		}

		private void Unfreeze()
		{
			_rigidbody.constraints = RigidbodyConstraints.None;
		}

		public void Respawn(Checkpoint at)
		{
			Transform target = at.transform;
			Vector3 spawnPosition = target.position;
			Quaternion spawnRotation = target.rotation;

			Transform shipTransform = transform;
			shipTransform.position = spawnPosition;
			shipTransform.rotation = spawnRotation * Quaternion.Euler(0,-90,0);

			_rigidbody.position = spawnPosition;
			_rigidbody.rotation = spawnRotation * Quaternion.Euler(0,-90,0);
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;

			foreach (MagLaser magLaser in _magLasers)
			{
				magLaser.Reset();
			}

			_currentThrust = 0;
		}

		private void OnFinish()
		{
			_controls.Disable();
			GetComponent<PlayerInput>().DeactivateInput();
		}

		private void Start()
		{
			exitWater.Invoke();
		}

		public void OnRotation(InputValue value)
		{
			Vector2 direction = value.Get<Vector2>();

			_controllerInput.vertical = direction.y;
			_controllerInput.horizontal = direction.x;
		}

		public void OnThrust(InputValue value)
		{
			_controllerInput.acceleration = value.Get<float>();
		}

		public void OnDashLeft()
		{
			Debug.Log("Dash left");
		}

		public void OnDashRight()
		{
			Debug.Log("Dash right");
		}

		public void OnResetButton(InputValue value)
		{
			CheckpointTracker cpT = GetComponent<CheckpointTracker>();
			if (value.isPressed)
			{
				Respawn(CheckpointManager.Instance.cpList[(cpT.nextCpNumber - 1 + CheckpointManager.Instance.cpList.Count)
					% CheckpointManager.Instance.cpList.Count]);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			//Underwater drag
			if (other.sharedMaterial == PhysicMaterialLibrary.Water)
			{
				_rigidbody.drag = underwaterDrag;
				enterWater.Invoke();
			}

			if (other.TryGetComponent(out Boost _))
			{
				_currentThrust = boostThrust;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.sharedMaterial == PhysicMaterialLibrary.Water)
			{
				_rigidbody.drag = driveDrag;
				exitWater.Invoke();
			}
		}

		private void Update()
		{
			//TODO: Allow negative numbers for reversing
			isAccelerating.Invoke(Mathf.Abs(_controllerInput.acceleration));
		}

		private void FixedUpdate()
		{
			Transform t = transform;
			Vector3 forward = t.forward;
			Vector3 right = t.right;
			Vector3 up = t.up;

			//Boost
			_currentThrust = Mathf.Lerp(_currentThrust, thrust, Time.fixedDeltaTime / boostDuration);

			//Gravity
			bool track = _magLasers.Any(magLaser => magLaser.IsAttachedToTrack);
			float g = track ? trackGravity : gravity;
			_rigidbody.AddForce(Vector3.down * g);

			//Friction
			float friction = _magLasers.Average(magLaser => magLaser.GroundFriction);
			_rigidbody.AddForce(-_rigidbody.velocity * friction);

			bool attached = _magLasers.Any(magLaser => magLaser.IsAttached);

			//Input
			_rigidbody.AddTorque(up * (_controllerInput.horizontal * turnSpeed)); //always just rotate around UP based on horizontal input
			if (attached)
			{
				_rigidbody.AddForceAtPosition(forward * (_controllerInput.acceleration * -_currentThrust), engineForcePosition.position); //apply at engine force position, to pitch down a bit
				if (Math.Abs(_rigidbody.drag - underwaterDrag) > 0.01f) //Don't overwrite underwater drag
					_rigidbody.drag = driveDrag;
			}
			else
			{
				_rigidbody.drag = flightDrag;

				_rigidbody.AddForce(forward * (_controllerInput.acceleration * -_currentThrust)); //apply forward force at center of mass, to not pitch down

				//==Pitch==
				Vector3 localEulerAngles = t.localEulerAngles;
				float pitch = Mathf.DeltaAngle(localEulerAngles.x, 0f);
				float pitchFactor = pitch / maxPitch;
				float pitchFactor01 = Mathf.Clamp01(1.0f - Mathf.Abs(pitchFactor));
				if (pitch < 0f)
				{
					switch (_controllerInput.vertical)
					{
						case < 0f:
							_rigidbody.AddTorque(right * (_controllerInput.vertical * -pitchSpeed * pitchFactor01));
							break;
						case > 0f:
							_rigidbody.AddTorque(right * (_controllerInput.vertical * -pitchSpeed));
							break;
					}
				}
				else
				{
					switch (_controllerInput.vertical)
					{
						case < 0f:
							_rigidbody.AddTorque(right * (_controllerInput.vertical * -pitchSpeed));
							break;
						case > 0f:
							_rigidbody.AddTorque(right * (_controllerInput.vertical * -pitchSpeed * pitchFactor01));
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
