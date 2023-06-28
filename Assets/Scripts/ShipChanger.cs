using Physics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider), typeof(ShipControls))]
public class ShipChanger : MonoBehaviour
{
	private static int _player;

	[SerializeField] private Mesh player2Mesh;
	[SerializeField] private float player2TurnSpeedMultiplier = 1.25f;

	private void Awake()
	{
		_player++;
		if (_player == 2)
		{
			GetComponent<MeshFilter>().sharedMesh = player2Mesh;
			// GetComponent<MeshCollider>().sharedMesh = player2Mesh;
			// GetComponent<ShipControls>().turnSpeed *= player2TurnSpeedMultiplier;
		}
	}

	private void OnDestroy()
	{
		_player--;
	}
}
