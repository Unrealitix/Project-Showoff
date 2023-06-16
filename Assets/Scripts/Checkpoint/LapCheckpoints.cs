using UnityEngine;

public class LapCheckpoints : MonoBehaviour
{
    [HideInInspector] public int nextCpNumber;
    [HideInInspector] public bool startLap;

    private void Awake()
    {
        nextCpNumber = 0;
    }

    private void PassedThroughCp(Checkpoint checkpoint)
    {
        if(CheckpointManager.Instance.cpList.IndexOf(checkpoint) == nextCpNumber)
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
        if (other.TryGetComponent<Checkpoint>(out Checkpoint checkpoint))
        {
            this.PassedThroughCp(checkpoint);
        }
    }
}
