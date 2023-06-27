using Cinemachine;
using Physics;
using UnityEngine;

namespace Camera
{
	[AddComponentMenu("")] // Hide in menu
	[ExecuteAlways]
	public class CinemachineSpeedFOV : CinemachineExtension
	{
		private const float MIN_FOV = 1f;
		private const float MAX_FOV = 179f;

		private const float MIN_DISTANCE = 0f;
		private const float MAX_DISTANCE = 50f;

		[Tooltip("When to apply the offset")] [SerializeField] private CinemachineCore.Stage applyAfter = CinemachineCore.Stage.Aim;
		[Range(MIN_FOV, MAX_FOV)] [Tooltip("FOV at zero speed.")] [SerializeField] private float minFOV = 60f;
		[Range(MIN_FOV, MAX_FOV)] [Tooltip("FOV at max speed.")] [SerializeField] private float maxFOV = 90f;
		[Range(MIN_DISTANCE, MAX_DISTANCE)] [Tooltip("Camera distance at zero speed.")] [SerializeField] private float minDist = 15f;
		[Range(MIN_DISTANCE, MAX_DISTANCE)] [Tooltip("Camera distance at max speed.")] [SerializeField] private float maxDist = 5f;
		[Tooltip("Speed of the target at which the max FOV will be reached")] [SerializeField] private float maxSpeed = 80f;
		[Tooltip("If the FOV should be clamped between the min and max, or if it should be allowed to go further")] [SerializeField] private bool clamp;

		private void OnValidate()
		{
			minFOV = Mathf.Clamp(minFOV, MIN_FOV, MAX_FOV);
			maxFOV = Mathf.Clamp(maxFOV, minFOV, MAX_FOV);
		}

		private Rigidbody _followRigidbody;

		/// <summary>
		/// Applies the specified offset to the camera state
		/// </summary>
		/// <param name="vcam">The virtual camera being processed</param>
		/// <param name="stage">The current pipeline stage</param>
		/// <param name="state">The current virtual camera state</param>
		/// <param name="deltaTime">The current applicable deltaTime</param>
		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (_followRigidbody == null)
			{
				_followRigidbody = vcam.Follow.GetComponent<Rigidbody>();
				if (_followRigidbody == null) //if it's still null, the follow target doesn't have a Rigidbody
				{
					Debug.LogError($"CinemachineSpeedFOV: Follow target ({vcam.Follow.name}) must have a Rigidbody component!");
					return;
				}

				if (_followRigidbody.TryGetComponent(out ShipControls shipControls))
					maxSpeed = shipControls.MaxSpeed;
			}

			if (stage == applyAfter)
			{
				state.Lens.FieldOfView = Mathf.Clamp(Mathf.LerpUnclamped(minFOV, maxFOV, _followRigidbody.velocity.magnitude / maxSpeed), minFOV, clamp ? maxFOV : MAX_FOV);

				float dot = Vector3.Dot(_followRigidbody.velocity, _followRigidbody.transform.forward);
				float dir = dot >= maxSpeed/5f ? -1f : 1f;
				float dist = Mathf.Clamp(Mathf.LerpUnclamped(minDist, maxDist, _followRigidbody.velocity.magnitude / maxSpeed), minDist, clamp ? maxDist : MAX_DISTANCE);
				state.PositionCorrection = transform.forward * dir * dist;
			}
		}
	}
}
