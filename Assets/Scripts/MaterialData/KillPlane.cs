using Physics;
using UnityEngine;

namespace MaterialData
{
	public class KillPlane : MonoBehaviour
	{
		[SerializeField] private Material killPlaneMaterial;
		private static readonly int Ship1ID = Shader.PropertyToID("_ShipPos1");
		private static readonly int Ship2ID = Shader.PropertyToID("_ShipPos2");

		private int _myShipID;

		private void Awake()
		{
			if (FindObjectsOfType<ShipControls>().Length == 1)
			{
				_myShipID = Ship1ID;
				killPlaneMaterial.SetVector(Ship2ID, Vector4.zero);
			}
			else
			{
				_myShipID = Ship2ID;
			}
		}

		private void Update()
		{
			killPlaneMaterial.SetVector(_myShipID, transform.position);
		}

		private void OnDisable()
		{
			killPlaneMaterial.SetVector(Ship1ID, Vector4.zero);
			killPlaneMaterial.SetVector(Ship2ID, Vector4.zero);
		}
	}
}
