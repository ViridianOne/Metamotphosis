using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAchievement : MonoBehaviour
{
    public GameObject sprite;
    public string id { get; set; }

    public bool completed { get; private set; }

    public CustomAchievement(string id, double percent)
    {
        this.id = id;
        completed = false;
    }

    public void SetCompleted()
    {
        if (completed)
            return;
        else
        {
            completed = true;
            LevelManager.instance.AchievementsCount++;
            sprite.SetActive(false);
        }
    }

}
