using System.Collections.Generic;
using Physics;
using TMPro;
using Track;
using UnityEngine;

namespace Checkpoints
{
	public class CheckpointTracker : MonoBehaviour
	{
		private static List<CheckpointTracker> _players;

		[HideInInspector] public int nextCpNumber;
		[SerializeField] private TMP_Text wrongWay;

		//Objects for the laps UI
		[SerializeField] private TMP_Text currentPos;
		[SerializeField] private TMP_Text maxPos;

		private int _checkpointAccumulator;
		private float _distanceToNext;

		private void Awake()
		{
			nextCpNumber = 0;
			_players ??= new List<CheckpointTracker>();
			_players.Add(this);
		}

		private void Update()
		{
			Transform t = transform;
			Vector3 position = t.position;
			Vector3 nextPos = CheckpointManager.Instance.cpList[nextCpNumber].transform.position;
			Vector3 nextDir = nextPos - position;

			// Wrong Way UI
			float dot = Vector3.Dot(t.forward, nextDir);
			wrongWay.gameObject.SetActive(dot > 0);

			// Position UI
			_distanceToNext = Vector3.Distance(position, nextPos);

			if (_players.Count > 1)
			{
				_players.Sort((a, b) =>
				{
					int initialCheck = b._checkpointAccumulator.CompareTo(a._checkpointAccumulator);
					if (initialCheck != 0) return initialCheck;
					return a._distanceToNext.CompareTo(b._distanceToNext);
				});
				currentPos.text = $"{_players.IndexOf(this) + 1}";
			}
			maxPos.text = $"/{_players.Count}";
		}

		private void PassedThroughCp(Checkpoint checkpoint)
		{
			if (CheckpointManager.Instance.cpList.IndexOf(checkpoint) == nextCpNumber)
			{
				_checkpointAccumulator++;
				nextCpNumber = (nextCpNumber + 1) % CheckpointManager.Instance.cpList.Count;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Checkpoint checkpoint))
			{
				PassedThroughCp(checkpoint);
			}
			else if (other.TryGetComponent(out KillZone _))
			{
				ShipControls ship = GetComponent<ShipControls>();
				Checkpoint cp = CheckpointManager.Instance.cpList[(nextCpNumber - 1 + CheckpointManager.Instance.cpList.Count) % CheckpointManager.Instance.cpList.Count];
				Debug.Log(cp.name);
				ship.Respawn(cp);
			}
		}
	}
}
