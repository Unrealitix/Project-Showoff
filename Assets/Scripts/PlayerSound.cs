using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private EventReference engine;
    private EventInstance _engine;
    // Start is called before the first frame update
    void Start()
    {
        _engine = RuntimeManager.CreateInstance(engine);
        _engine.start();
    }

    void OnAcceleration(bool accelerate)
    {
        if (accelerate){
            _engine.setParameterByName("acceleration", 1);
        }
        else {
            _engine.setParameterByName("acceleration", 0);
        }
    }
}
