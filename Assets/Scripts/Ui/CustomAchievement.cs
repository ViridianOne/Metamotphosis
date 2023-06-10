using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAchievement : MonoBehaviour
{
    public GameObject sprite;
    public int id;

    public bool completed { get; private set; }

    public void SetCompleted()
    {
        if (completed)
            return;
        else
        {
            completed = true;
            sprite.SetActive(false);
        }
    }

}
