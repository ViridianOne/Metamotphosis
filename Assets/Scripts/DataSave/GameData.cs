using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Vector3 lastCheckpoint;
    public bool[] mecroFormsAvailabilty;
    public bool isBossDefeated;
    public int lastLocation;
    public int collectedDisksCount;
    public SerializableDictionary<string, bool> collectedDisks;
    public SerializableDictionary<int, bool> completedLocations;
    public bool[] completedAchievements;
    public float elipsedTime;
    public int playerLossesCount;

    public GameData(Vector3 checkpoint, bool[] mecroFormsAvailabilty, bool isBossDefeated, int lastLocation, 
        int collectedDisksCount, SerializableDictionary<string, bool> collectedDisks,
        SerializableDictionary<int, bool> completedLocations, bool[] completedAchievements, float elipsedTime, int playerLossesCount)
    {
        this.lastCheckpoint = checkpoint;
        this.mecroFormsAvailabilty = mecroFormsAvailabilty;
        this.isBossDefeated = isBossDefeated;
        this.lastLocation = lastLocation;
        this.collectedDisksCount = collectedDisksCount;
        this.collectedDisks = collectedDisks;
        this.completedLocations = completedLocations;
        this.completedAchievements = completedAchievements;
        this.elipsedTime = elipsedTime;
        this.playerLossesCount = playerLossesCount;
    }
}
