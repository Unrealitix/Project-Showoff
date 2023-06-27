using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Checkpoints
{
	public class CheckpointTracker : MonoBehaviour
	{
		private static List<CheckpointTracker> _players;

		public int NextCpNumber { get; private set; }
		public bool HasPassedThroughACheckpoint => _checkpointAccumulator > 0;

		[SerializeField] private TMP_Text wrongWay;

		public UnityEvent hitCheckpoint;

		//Objects for the laps UI
		[SerializeField] private TMP_Text currentPos;
		[SerializeField] private TMP_Text maxPos;

		private int _checkpointAccumulator;
		private float _distanceToNext;

		private void Awake()
		{
			NextCpNumber = 0;
			_players ??= new List<CheckpointTracker>();
			_players.Add(this);
		}

		private void Update()
		{
			Transform t = transform;
			Vector3 position = t.position;
			Vector3 nextPos = CheckpointManager.Instance.cpList[NextCpNumber].transform.position;
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
			if (CheckpointManager.Instance.cpList.IndexOf(checkpoint) == NextCpNumber)
			{
				_checkpointAccumulator++;
				NextCpNumber = (NextCpNumber + 1) % CheckpointManager.Instance.cpList.Count;
				hitCheckpoint.Invoke();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out Checkpoint checkpoint))
			{
				PassedThroughCp(checkpoint);
			}
		}
	}
}
