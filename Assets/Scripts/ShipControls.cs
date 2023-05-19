using UnityEngine;

public class ShipControls : MonoBehaviour
{
	//Only controls and physics forces in this class!
	//Health and stuff like that should be in a separate class

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
	}
}
