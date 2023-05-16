using UnityEngine;
using UnityEngine.Serialization;

public class Ship : MonoBehaviour
{
	[Header("Physics")]
	[SerializeField] private float rotationSpeed = 10f;
	[SerializeField] private float magnetLength = 10f;
	[SerializeField] private float hoverHeight = 5f;
	[SerializeField] private float hoverForce = 1f;

	[Header("Controls")]
	[SerializeField] private float thrust = 10f;
	[SerializeField] private float turnSpeed = 10f;

	private Rigidbody _rigidbody;

	private void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		_rigidbody.AddForce(transform.forward * (Input.GetAxis("Vertical") * -thrust));
		_rigidbody.AddTorque(transform.up * (Input.GetAxis("Horizontal") * turnSpeed));

		//raycast downwards
		//find normal of surface at point of contact
		//rotate ship to match normal

		Transform t = transform;
		Vector3 dir = -t.up;
		Debug.DrawRay(t.position, dir * magnetLength, Color.red);
		if (Physics.Raycast(t.position, dir, out RaycastHit result, magnetLength))
		{
			Debug.DrawRay(t.position, dir * result.distance, Color.green);
			Debug.DrawRay(result.point, result.normal, Color.blue);
			float force = (hoverHeight - result.distance) * hoverForce;
			_rigidbody.AddForce(result.normal * force, ForceMode.Acceleration);
			t.rotation = Quaternion.Lerp(t.rotation, Quaternion.LookRotation(Vector3.Cross(t.right, result.normal), result.normal),
				Time.deltaTime * rotationSpeed);
		}
	}
}
