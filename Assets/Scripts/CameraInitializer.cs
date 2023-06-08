using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraInitializer : MonoBehaviour
{
	[SerializeField] private GameObject[] virtualPlayerCams;

	private Camera _thisCamera;

	private void Start()
	{
		_thisCamera = GetComponent<Camera>();

		ShipControls[] shipControlsArray = FindObjectsOfType<ShipControls>();
		int layer = shipControlsArray.Length + 9;

		foreach (GameObject virtualPlayerCam in virtualPlayerCams)
		{
			virtualPlayerCam.layer = layer;
		}

		int bitMask = (1 << layer)
		              | (1 << 0)
		              | (1 << 1)
		              | (1 << 2)
		              | (1 << 4)
		              | (1 << 5)
		              | (1 << 8);

		_thisCamera.cullingMask = bitMask;
		_thisCamera.gameObject.layer = layer;
	}
}
