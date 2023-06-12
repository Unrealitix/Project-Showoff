using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class music : MonoBehaviour
{
    [SerializeField] private EventReference musicc;
    private EventInstance _music;
    private bool once = true;
    // Start is called before the first frame update
    void Start()
    {
        _music = RuntimeManager.CreateInstance(musicc);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && once) {
            _music.start(); 
            once = false; 
            Debug.Log("lol");
        }
    }
}
