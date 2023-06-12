using Physics;
using UnityEngine;

namespace Camera
{
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class SplitScreenInitializer : MonoBehaviour
	{
		[SerializeField] private GameObject[] virtualPlayerCams;

		private UnityEngine.Camera _thisCamera;

		private void Start()
		{
			_thisCamera = GetComponent<UnityEngine.Camera>();

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
}
