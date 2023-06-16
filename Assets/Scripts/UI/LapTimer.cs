using System;
using TMPro;
using UnityEngine;

public class LapTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text bestTime;
    private float _lapTime;
    private float _bestTime;
    private LapCheckpoints _lapCp;
    private bool _startLap;
    

    private void Awake()
    {
        _lapCp = GetComponent<LapCheckpoints>();
    }

    private void Update()
    {
        if (_startLap)
        { 
            _lapTime += Time.deltaTime;
            timer.text = ShowTimer();
            Debug.Log(name);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        _startLap = true;
        if (_lapCp.nextCpNumber != 0) return;
        if (_lapTime < _bestTime || _bestTime == 0)
        {
            _bestTime = _lapTime; 
            bestTime.text = ShowTimer();
        }
        _startLap = false;
        _lapTime = 0;
    }

    string ShowTimer()
    {
        int intTime = (int)_lapTime;
        int seconds = intTime % 60;
        int minutes = intTime / 60;
        float fraction = _lapTime * 1000;
        fraction = (fraction % 1000);
        string timeText = $"{minutes:00}:{seconds:00}:{fraction:000}";
        return timeText;
    }
}
