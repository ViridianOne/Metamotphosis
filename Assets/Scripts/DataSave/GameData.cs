using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 lastCheckpoint;
    public bool[] mecroFromsAvailabilty;

    public GameData(Vector3 checkpoint, bool[] mecroFromsAvailabilty)
    {
        this.lastCheckpoint = checkpoint;
        this.mecroFromsAvailabilty = mecroFromsAvailabilty;
    }
}
