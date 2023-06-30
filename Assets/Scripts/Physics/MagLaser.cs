using Generated;
using JetBrains.Annotations;
using UnityEngine;

namespace Physics
{
	public class MagLaser : MonoBehaviour
	{
		[SerializeField] private float magnetLength = 10f;
		[Tooltip("Target height to hover at, above the ground")] [SerializeField] private float targetHoverHeight = 5f;
		[Tooltip("Speed at which the target hover height extends to the magnet length when not attached")] [SerializeField] private float heightAdjustSpeed = 2f;
		[Tooltip("Horizontal axis is height, vertical axis is resulting thrust factor")] [SerializeField] private AnimationCurve thrustCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField] private float hoverForce = 10f;

		[CanBeNull] private PhysicMaterial _groundMaterial;
		private float _currentHoverHeight;

		/// <summary>
		/// Dynamic friction of the ground the MagLaser is hitting.
		/// </summary>
		public float GroundFriction => _groundMaterial != null ? _groundMaterial.dynamicFriction : 0f;

		/// <summary>
		/// Hover force multiplier of the ground the MagLaser is hitting.
		/// </summary>
		private float GroundHoverForceMultiplier => _groundMaterial != null ? _groundMaterial.staticFriction : 1f;

		/// <summary>
		/// If the MagLaser is attached to anything.
		/// </summary>
		public bool IsAttached => _groundMaterial != null;

		/// <summary>
		/// If the MagLaser is attached to the track.
		/// </summary>
		public bool IsAttachedToTrack => _groundMaterial == PhysicMaterialLibrary.Track;

		private Rigidbody _shipRigidbody;
		private Transform _thisTransform;

		private void OnValidate()
		{
			_thisTransform = transform;
			thrustCurve.preWrapMode = WrapMode.PingPong;
		}

		private void Awake()
		{
			OnValidate();
			if (transform.parent.GetComponent<ShipControls>() == null)
				Debug.LogError("MagLaser must be a child of a Ship!");

			_shipRigidbody = transform.parent.GetComponent<Rigidbody>();
			ResetMagLaser();
		}

		public void ResetMagLaser()
		{
			_currentHoverHeight = targetHoverHeight;
		}

		private void FixedUpdate()
		{
			Vector3 pos = _thisTransform.position;
			Vector3 down = -_thisTransform.up;
			Debug.DrawRay(pos, down * magnetLength, Color.red);
			Debug.DrawRay(pos, down * _currentHoverHeight, Color.yellow);
			if (UnityEngine.Physics.Raycast(pos, down, out RaycastHit result, magnetLength))
			{
				Debug.DrawRay(pos, down * result.distance, Color.green);
				Debug.DrawRay(result.point, result.normal, Color.blue);
				_groundMaterial = result.collider.sharedMaterial;

				float heightFac = (_currentHoverHeight - result.distance) / _currentHoverHeight; // [-1, 1]
				heightFac = Mathf.Sign(heightFac) * thrustCurve.Evaluate(heightFac);
				float force = heightFac * hoverForce * GroundHoverForceMultiplier;
				_shipRigidbody.AddForceAtPosition(result.normal * force, pos);
				_currentHoverHeight = Mathf.Lerp(_currentHoverHeight, targetHoverHeight, Time.fixedDeltaTime * 2*heightAdjustSpeed);
			}
			else
			{
				_groundMaterial = null;
				_currentHoverHeight = Mathf.Lerp(_currentHoverHeight, magnetLength, Time.fixedDeltaTime * heightAdjustSpeed);
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (Application.isPlaying) return;
			Gizmos.color = Color.red;
			Vector3 position = _thisTransform.position;
			Vector3 down = -_thisTransform.up;
			Gizmos.DrawRay(position, down * magnetLength);
			Gizmos.DrawSphere(position + down * targetHoverHeight, 0.1f);
		}
	}
}
