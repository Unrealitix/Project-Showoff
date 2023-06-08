using UnityEngine;

public class CameraInitializer : MonoBehaviour
{
	[SerializeField] private Camera cam;
	[SerializeField] private GameObject virtualPlayerCam;

	private void Start()
	{
		ShipControls[] shipControlsArray = FindObjectsOfType<ShipControls>();
		int layer = shipControlsArray.Length + 9;

		virtualPlayerCam.layer = layer;

		int bitMask = (1 << layer)
		              | (1 << 0)
		              | (1 << 1)
		              | (1 << 2)
		              | (1 << 4)
		              | (1 << 5)
		              | (1 << 8);

		cam.cullingMask = bitMask;
		cam.gameObject.layer = layer;
	}
}
