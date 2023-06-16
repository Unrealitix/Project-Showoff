using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager _instance;
    public static CheckpointManager Instance => _instance;

    public List<Checkpoint> cpList;

    private void Awake()
    {
        _instance = this;

        cpList = new List<Checkpoint>();
        foreach (Transform cpTransform in this.transform)
        {
            Checkpoint cp = cpTransform.GetComponentInChildren<Checkpoint>();
            Debug.Log(cp);
            cpList.Add(cp);
        }
    }
}
