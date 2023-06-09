using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public static Checkpoints instance;
    public int currentCheckpoint;
    public int previousCheckpoint;
    public Transform first;


    private void Awake()
    {
        instance = this;
    }
}
