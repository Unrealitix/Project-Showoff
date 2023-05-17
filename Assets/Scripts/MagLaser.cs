using JetBrains.Annotations;
using UnityEngine;

public class MagLaser : MonoBehaviour
{
	[SerializeField] private float magnetLength = 10f;
	[Tooltip("Target height to hover at, above the ground")] [SerializeField] private float targetHoverHeight = 5f;
	[SerializeField] private float hoverForce = 5f;

	[CanBeNull] private PhysicMaterial _groundMaterial;

	/// <summary>
	/// Dynamic friction of the ground the MagLaser is hitting.
	/// </summary>
	public float Friction => _groundMaterial != null ? _groundMaterial.dynamicFriction : 0f;

	/// <summary>
	/// Hover force multiplier of the ground the MagLaser is hitting.
	/// </summary>
	private float HoverForceMultiplier => _groundMaterial != null ? _groundMaterial.staticFriction : 1f;

	private Rigidbody _shipRigidbody;
	private Transform _thisTransform;

	private void OnValidate()
	{
		_thisTransform = transform;
	}

	private void Awake()
	{
		OnValidate();
		if (transform.parent.GetComponent<ShipControls>() == null)
			Debug.LogError("MagLaser must be a child of a Ship!");

		_shipRigidbody = transform.parent.GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		Vector3 pos = _thisTransform.position;
		Vector3 dir = -_thisTransform.up;
		Debug.DrawRay(pos, dir * magnetLength, Color.red);
		if (Physics.Raycast(pos, dir, out RaycastHit result, magnetLength))
		{
			Debug.DrawRay(pos, dir * result.distance, Color.green);
			Debug.DrawRay(result.point, result.normal, Color.blue);
			_groundMaterial = result.collider.sharedMaterial;

			float force = (targetHoverHeight - result.distance) * (hoverForce * HoverForceMultiplier);
			_shipRigidbody.AddForceAtPosition(result.normal * force, pos);
		}
		else
		{
			_groundMaterial = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying) return;
		Gizmos.color = Color.red;
		Gizmos.DrawRay(_thisTransform.position, -_thisTransform.up * magnetLength);
	}
}
