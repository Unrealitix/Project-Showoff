using System.Collections.Generic;
using UnityEngine;

namespace Checkpoints
{
	public class CheckpointManager : MonoBehaviour
	{
		public static CheckpointManager Instance { get; private set; }

		[HideInInspector] public List<Checkpoint> cpList;

		private void Awake()
		{
			Instance = this;

			cpList = new List<Checkpoint>();
			foreach (Transform cpTransform in transform)
			{
				Checkpoint cp = cpTransform.GetComponentInChildren<Checkpoint>();
				Debug.Log(cp);
				cpList.Add(cp);
			}
		}
	}
}
