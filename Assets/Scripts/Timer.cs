using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer instance;
    private bool isStarted;
    private float elapsedTime;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isStarted = false;

    }

    public void StartTimer()
    {
        isStarted = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void EndTimer()
    {
        isStarted = false;
    }

    private IEnumerator UpdateTimer()
    {
        while(isStarted)
        {
            elapsedTime += Time.deltaTime;
            LevelManager.instance.SetTime(TimeSpan.FromSeconds(elapsedTime));
            yield return null;
        }
    }
}
