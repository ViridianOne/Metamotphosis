using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SocialPlatforms.Impl;

public class LevelManager : MonoBehaviour, IDataPersistance
{
    public static LevelManager instance;

    private bool hasLocationSet = false;
    [SerializeField] private Location startLocation;

    public Location currentLocation { get; private set; } = Location.None;
    public int currentRoomNumber { get; private set; }
    public Vector3Int currentPositionOnMap { get; private set; }
    [SerializeField] private Light2D globalLight;
    public bool isDarknessOn { get; private set; }

    public int disksCount { get; private set; }
    public int maxDisksAmount;

    public bool containsBoss = false;
    [HideInInspector] public bool isBossDefeated = false;
    [HideInInspector] public SerializableDictionary<string, bool> collectedDisks;
    [HideInInspector] public bool isCompleted;

    public int AchievementsCount { get; set; }
    public int maxAchievementsAmount;
    [SerializeField] public CustomAchievement[] AchievementsList { get; private set; }

    public int lossesCount { get; private set; }
    public TimeSpan timePlaying { get; private set; }
    

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentLocation = Location.location161;
        currentRoomNumber = 1;
        currentPositionOnMap = new Vector3Int(12, -6, 0);
    }

    public void SetMapInfo(Location location, int roomNumber, Vector3Int positionOnMap)
    {
        hasLocationSet = true;
        currentLocation = location;
        currentRoomNumber = roomNumber;
        currentPositionOnMap = positionOnMap;

        if (location == Location.location26 || location == Location.location116 || location == Location.locztion296)
        {
            if (roomNumber == 15)
                isCompleted = true;
        }
        else
        {
            if (roomNumber == 14)
                isCompleted = true;
        }
    }

    public void SetGlobalLightItensity(float intensity)
    {
        globalLight.intensity = intensity;
        isDarknessOn = intensity == 0;
    }

    public void CollectDisk()
    {
        disksCount++;
    }

    public void LoadData(GameData data)
    {
        if (hasLocationSet && (int)currentLocation != data.lastLocation
            || !hasLocationSet && (int)startLocation != data.lastLocation)
        {
            MecroSelectManager.instance.respawnPoint.position = data.lastCheckpoint = Checkpoints.instance.first.position;
        }
        if (containsBoss)
            isBossDefeated = data.isBossDefeated;
        disksCount = data.collectedDisksCount;
        collectedDisks = data.collectedDisks;
        isCompleted = data.completedLocations[(int)(hasLocationSet ? currentLocation : startLocation)];
    }

    public void SaveData(ref GameData data)
    {
        if (containsBoss)
            data.isBossDefeated = isBossDefeated;
        data.lastLocation = (int)currentLocation;
        data.collectedDisksCount = disksCount;
        data.collectedDisks = collectedDisks;
        data.completedLocations[(int)currentLocation] = isCompleted;
    public void CountLosses()
    {
        lossesCount++;
    }

    public void SetTime(TimeSpan time)
    {
        timePlaying = time;
    }

    public void UpdateAchievements(CustomAchievement[] achievementsList)
    {
        AchievementsList = achievementsList;
    }
}
