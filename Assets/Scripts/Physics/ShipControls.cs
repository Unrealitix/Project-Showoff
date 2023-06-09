using System;
using System.Collections;
using System.Linq;
using Checkpoints;
using Cinemachine;
using Generated;
using NaughtyAttributes;
using TMPro;
using Track;
using UI.PerPlayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Physics
{
	public class ShipControls : MonoBehaviour
	{
		//Only controls and physics forces in this class!
		//Health and stuff like that should be in a separate class

		private const string EVENT_FOLDOUT_NAME = "Events";

		[SerializeField] private string spawnLocationName;
		[SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

		[SerializeField] private float thrust = 10f;
		[SerializeField] internal float turnSpeed = 10f;
		[Tooltip("You need to manually measure this and fill it in!")] [SerializeField] private float maxSpeed = 10f;

		[Header("Countdown")]
		[SerializeField] private int countdownSeconds = 5;
		[SerializeField] private TMP_Text countdown;

		[SerializeField] private Transform centerOfMass;
		[SerializeField] private Transform engineForcePosition;

		[Header("Boost")]
		[SerializeField] private float boostThrust = 20f;
		[SerializeField] private float boostDurationSeconds = 1f;

		[Header("Dash")]
		[SerializeField] private float dashForce = 20f;
		[SerializeField] private float dashCooldownSeconds = 2f;
		[SerializeField] private float dashDurationSeconds = 1f;

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

		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent<float> isAccelerating;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent<bool> isFlying;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent hitBoost;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent dashed;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent dashRecharged;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent respawnedByKillPlane;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent respawnedManually;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent enterWater;
		[Foldout(EVENT_FOLDOUT_NAME)] public UnityEvent exitWater;

		public float MaxSpeed => maxSpeed;

		private Rigidbody _rigidbody;
		private MagLaser[] _magLasers;

		private Transform _spawnLocation;

		private float _currentThrust;
		private Controls _controls;
		private (float vertical, float horizontal, float acceleration) _controllerInput;

		private Vector3 _dashThrust;
		private float _dashLastUsed = -1f;

		private void OnValidate()
		{
			dashDurationSeconds = Mathf.Clamp(dashDurationSeconds, 0, dashCooldownSeconds);
		}

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
			_spawnLocation = spawn;

			Vector3 spawnPosition = _spawnLocation.position;
			Quaternion spawnRotation = _spawnLocation.rotation;

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

		private void Respawn()
		{
			if (_rigidbody.constraints == RigidbodyConstraints.FreezeAll) return;

			CheckpointTracker checkpointTracker = GetComponent<CheckpointTracker>();

			Transform location;
			Quaternion spawnRotation;
			if (checkpointTracker.HasPassedThroughACheckpoint)
			{
				int cpIndex = (checkpointTracker.NextCpNumber - 1 + CheckpointManager.Instance.cpList.Count) % CheckpointManager.Instance.cpList.Count;
				location = CheckpointManager.Instance.cpList[cpIndex].transform;
				spawnRotation = location.rotation * Quaternion.Euler(0, -90, 0);
			}
			else
			{
				location = _spawnLocation;
				spawnRotation = location.rotation;
			}

			Vector3 spawnPosition = location.position;

			Transform shipTransform = transform;
			shipTransform.position = spawnPosition;
			shipTransform.rotation = spawnRotation;

			_rigidbody.position = spawnPosition;
			_rigidbody.rotation = spawnRotation;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.angularVelocity = Vector3.zero;

			foreach (MagLaser magLaser in _magLasers)
			{
				magLaser.ResetMagLaser();
			}

			_currentThrust = 0;
			cinemachineVirtualCamera.PreviousStateIsValid = false;
		}

		private void OnFinish(float _)
		{
			_controls.Disable();
			GetComponent<PlayerInput>().DeactivateInput();
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
			Dash(transform.right);
		}

		public void OnDashRight()
		{
			Dash(-transform.right);
		}

		private void Dash(Vector3 dir)
		{
			if (Time.time - _dashLastUsed < dashCooldownSeconds) return;

			_dashThrust = dir * dashForce;
			_dashLastUsed = Time.time;
			dashed.Invoke();
			StartCoroutine(DashRecharge());
		}

		private IEnumerator DashRecharge()
		{
			yield return new WaitForSeconds(dashCooldownSeconds);
			dashRecharged.Invoke();
		}

		public void OnResetButton(InputValue value)
		{
			if (value.isPressed)
			{
				Respawn();
				respawnedManually.Invoke();
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
				hitBoost.Invoke();
			}

			if (other.TryGetComponent(out KillZone _))
			{
				Respawn();
				respawnedByKillPlane.Invoke();
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
			isAccelerating.Invoke(_controllerInput.acceleration);
		}

		private void FixedUpdate()
		{
			Transform t = transform;
			Vector3 forward = t.forward;
			Vector3 right = t.right;
			Vector3 up = t.up;

			//Boost
			_currentThrust = Mathf.Lerp(_currentThrust, thrust, Time.fixedDeltaTime / boostDurationSeconds);

			//Dash
			_rigidbody.AddForce(_dashThrust * Mathf.Clamp01(_rigidbody.velocity.magnitude / maxSpeed));
			_dashThrust = Vector3.Lerp(_dashThrust, Vector3.zero, Time.fixedDeltaTime / dashDurationSeconds);

			//Gravity
			bool track = _magLasers.Any(magLaser => magLaser.IsAttachedToTrack);
			float g = track ? trackGravity : gravity;
			_rigidbody.AddForce(Vector3.down * g);

			//Friction
			float friction = _magLasers.Average(magLaser => magLaser.GroundFriction);
			_rigidbody.AddForce(-_rigidbody.velocity * friction);

			bool attached = _magLasers.Any(magLaser => magLaser.IsAttached);
			isFlying.Invoke(!attached);

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
