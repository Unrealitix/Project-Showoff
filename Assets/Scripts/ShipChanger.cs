using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]
public class ShipChanger : MonoBehaviour
{
	private static int _player;

	[SerializeField] private Mesh player2Mesh;

	private void Awake()
	{
		_player++;
		if (_player == 2)
		{
			GetComponent<MeshFilter>().sharedMesh = player2Mesh;
			GetComponent<MeshCollider>().sharedMesh = player2Mesh;
		}
	}
}
