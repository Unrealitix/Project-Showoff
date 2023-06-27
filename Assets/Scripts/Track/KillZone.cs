using UnityEngine;

namespace Track
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
	public class KillZone : MonoBehaviour
	{
		[SerializeField] private Material editorMaterial;
		[SerializeField] private Material gameMaterial;

		private void Awake()
		{
			GetComponent<MeshRenderer>().material = Application.isPlaying ? gameMaterial : editorMaterial;
			GetComponent<Collider>().isTrigger = true;
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
	}
}
