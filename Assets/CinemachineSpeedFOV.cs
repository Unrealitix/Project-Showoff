using UnityEngine;
using Cinemachine;

[AddComponentMenu("")] // Hide in menu
[ExecuteAlways]
public class CinemachineSpeedFOV : CinemachineExtension
{
	private const float MIN = 1f;
	private const float MAX = 179f;

	[Tooltip("When to apply the offset")] [SerializeField] private CinemachineCore.Stage applyAfter = CinemachineCore.Stage.Aim;
	[Range(MIN, MAX)] [Tooltip("Lower limit for the FOV that this behaviour will generate.")] [SerializeField] private float minFOV = 60f;
	[Range(MIN, MAX)] [Tooltip("Upper limit for the FOV that this behaviour will generate.")] [SerializeField] private float maxFOV = 90f;
	[Tooltip("Speed of the target at which the max FOV will be reached")] [SerializeField] private float maxSpeed = 80f;
	[Tooltip("If the FOV should be clamped between the min and max, or if it should be allowed to go further")] [SerializeField] private bool clamp;

	private void OnValidate()
	{
		minFOV = Mathf.Clamp(minFOV, MIN, MAX);
		maxFOV = Mathf.Clamp(maxFOV, minFOV, MAX);
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
		}

		if (stage == applyAfter)
		{
			state.Lens.FieldOfView = Mathf.Clamp(Mathf.Lerp(minFOV, maxFOV, _followRigidbody.velocity.magnitude / maxSpeed), minFOV, clamp ? maxFOV : MAX);
		}
	}
}
