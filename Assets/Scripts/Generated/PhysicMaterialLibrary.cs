// This file and its accompanying .meta file were automatically generated.
// Any modifications you do to either of them will be overwritten.

using UnityEngine;

namespace Generated
{
	public class PhysicMaterialLibrary : ScriptableObject
	{
		private static PhysicMaterialLibrary _instance;
		private void Awake() => _instance = Resources.Load<PhysicMaterialLibrary>("PhysicMaterialLibrary");
		private void OnEnable() => Awake();

		public static PhysicMaterial Grass => _instance.propGrass;
		[SerializeField] private PhysicMaterial propGrass;

		public static PhysicMaterial Track => _instance.propTrack;
		[SerializeField] private PhysicMaterial propTrack;

		public static PhysicMaterial Water => _instance.propWater;
		[SerializeField] private PhysicMaterial propWater;
	}
}
