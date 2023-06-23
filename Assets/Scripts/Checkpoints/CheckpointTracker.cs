using Physics;
using TMPro;
using Track;
using UnityEngine;

namespace Checkpoints
{
	public class CheckpointTracker : MonoBehaviour
	{
		[HideInInspector] public int nextCpNumber;
		[SerializeField] private TMP_Text wrongWay;

		private void Awake()
		{
			nextCpNumber = 0;
		}

		private void Update()
		{
			Transform t = transform;
			Vector3 nextPos = CheckpointManager.Instance.cpList[nextCpNumber].transform.position;
			Vector3 nextDir = nextPos - t.position;
			float dot = Vector3.Dot(t.forward, nextDir);
			wrongWay.gameObject.SetActive(dot > 0);
			
			//For testing purposes
			// if (Input.GetKeyDown(KeyCode.R))
			// {
			// 	ShipControls ship = GetComponent<ShipControls>();
			// 	Checkpoint cp = CheckpointManager.Instance.cpList[nextCpNumber - 1];
			// 	Debug.Log(cp.name);
			// 	ship.Respawn(cp);
			// }
		}

		private void PassedThroughCp(Checkpoint checkpoint)
		{
			if (CheckpointManager.Instance.cpList.IndexOf(checkpoint) == nextCpNumber)
			{
				Debug.Log("Correct");
				nextCpNumber = (nextCpNumber + 1) % CheckpointManager.Instance.cpList.Count;
				Debug.Log(nextCpNumber);
			}
			else
			{
				Debug.Log("Wrong");
				Debug.Log(nextCpNumber);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Checkpoint checkpoint))
			{
				PassedThroughCp(checkpoint);
			}
			else if (other.TryGetComponent(out KillZone killZone))
			{
				ShipControls ship = GetComponent<ShipControls>();
				Checkpoint cp = CheckpointManager.Instance.cpList[nextCpNumber - 1];
				Debug.Log(cp.name);
				ship.Respawn(cp);
			}
		}
	}
}
