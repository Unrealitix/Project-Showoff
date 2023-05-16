using UnityEngine;

public class MagLaser : MonoBehaviour
{
	[SerializeField] private float magnetLength = 10f;
	[Tooltip("Target height to hover at, above the ground")] [SerializeField] private float targetHoverHeight = 5f;
	[SerializeField] private float hoverForce = 5f;

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
			float force = (targetHoverHeight - result.distance) * hoverForce;
			_shipRigidbody.AddForceAtPosition(result.normal * force, pos);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying) return;
		Gizmos.color = Color.red;
		Gizmos.DrawRay(_thisTransform.position, -_thisTransform.up * magnetLength);
	}
}
