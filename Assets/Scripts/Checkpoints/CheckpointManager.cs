using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Checkpoints
{
	[ExecuteInEditMode]
	public class CheckpointManager : MonoBehaviour
	{
		public static CheckpointManager Instance { get; private set; }

		[HideInInspector] public List<Checkpoint> cpList;

		private void Awake()
		{
			Instance = this;

			SetupList();
		}

		[Button]
		private void SetupList()
		{
			cpList = new List<Checkpoint>();
			foreach (Transform cpTransform in transform)
			{
				Checkpoint cp = cpTransform.GetComponentInChildren<Checkpoint>();
				cpList.Add(cp);
			}
		}

		private void OnDrawGizmos()
		{
			if (cpList == null) return;

			//Draw line between checkpoints
			for (int i = 0; i < cpList.Count-1; i++)
			{
				Checkpoint cp = cpList[i];
				if (cp == null) continue;

				Vector3 currentPos = cp.transform.position;
				Vector3 nextPos = cpList[i + 1].transform.position;
				Gizmos.color = Color.green;
				Gizmos.DrawLine(currentPos, nextPos);
			}
		}
	}
}
