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



    public bool containsBoss = false;
    [HideInInspector] public bool isBossDefeated = false;
    [HideInInspector] public SerializableDictionary<string, bool> collectedDisks;
    [HideInInspector] public bool isCompleted;
    [HideInInspector] public SerializableDictionary<int, bool> completedLocations;

    [Header("Statistics")]
    public int maxDisksAmount;
    public int disksCount { get; private set; }
    public int AchievementsCount { get; set; }
    public int maxAchievementsAmount;
    [HideInInspector] public bool[] completedAchievements = { false, false, false, false, false, false, false, false };
    public int lossesCount { get; private set; }
    public TimeSpan timePlaying { get; private set; }
    [SerializeField] private ExtrasController extraMenu;

    [Header("Achievements Data")]
    private int lightUsesCount;
    private float location71Timer;
    private TimeSpan location71Time;
    private int location206LosesCount;
    private int chemicalTouchesCount;
    private int lossesOnTheCeil;
    private float location251Timer;
    private TimeSpan location251Time;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentLocation = Location.location161;
        currentRoomNumber = 1;
        currentPositionOnMap = new Vector3Int(12, -6, 0);
        Timer.instance.StartTimer();
    }

    private void Update()
    {
        UpdateLightningStats();
        UpdateFlashShotStats();
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
        else if(location == Location.centralLocation)
        {
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
        UpdateWitnessStats();
        UpdateJournalistStats();
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
        completedAchievements = data.completedAchievements;
        completedLocations = data.completedLocations;
        Timer.instance.elapsedTime = data.elipsedTime;
        lossesCount = data.playerLossesCount;
        UpdateAchievementsCount();
    }

    public void SaveData(ref GameData data)
    {
        if (containsBoss)
            data.isBossDefeated = isBossDefeated;
        data.lastLocation = (int)currentLocation;
        data.collectedDisksCount = disksCount;
        data.collectedDisks = collectedDisks;
        data.completedLocations[(int)currentLocation] = isCompleted;
        data.completedAchievements = completedAchievements;
        data.elipsedTime = Timer.instance.elapsedTime;
        data.playerLossesCount = lossesCount;
    }
    public void CountLosses()
    {
        lossesCount++;
    }

    public void SetTime(TimeSpan time)
    {
        timePlaying = time;
    }

    public void UpdateDarkVisionStats()
    {
        if (currentLocation == Location.location161)
        {
            if (!isCompleted)
                lightUsesCount++;
            else
            {
                if (lightUsesCount == 0)
                    UpdateAchievement(0);
            }
        }
    }

    public void UpdateWitnessStats()
    {
        if (disksCount >= 5)
            UpdateAchievement(1);
    }

    public void UpdateLightningStats()
    {
        if (currentLocation == Location.location71 && Time.timeScale != 0)
        {
            if (!isCompleted)
            {
                location71Timer += Time.deltaTime;
                location71Time = TimeSpan.FromSeconds(location71Timer);
            }
            else
            {
                if (location71Time.TotalMinutes <= 20)
                    UpdateAchievement(2);
            }
        }
    }

    public void UpdateSafeAndSoundStats()
    {
        if (currentLocation == Location.location206)
        {
            if (!isCompleted)
                location206LosesCount++;
            else
            {
                if (location206LosesCount < 11)
                    UpdateAchievement(3);
            }
        }
    }

    public void UpdateAntidoteStats()
    {
        if (currentLocation == Location.location341)
        {
            if (!isCompleted)
                chemicalTouchesCount++;
            else
            {
                if (chemicalTouchesCount == 0)
                    UpdateAchievement(4);
            }
        }
    }

    public void UpdateLifeAtTheCeilStats()
    {
        if (currentLocation == Location.location116)
        {
            if (!isCompleted)
                lossesOnTheCeil++;
            else
            {
                if (lossesOnTheCeil == 0)
                    UpdateAchievement(5);
            }
        }
    }

    public void UpdateFlashShotStats()
    {
        if (currentLocation == Location.location251 && Time.timeScale != 0)
        {
            if (!isCompleted)
            {
                location251Timer += Time.deltaTime;
                location251Time = TimeSpan.FromSeconds(location251Timer);
            }
            else
            {
                if (location251Time.TotalMinutes <= 20)
                    UpdateAchievement(6);
            }
        }
    }

    public void UpdateJournalistStats()
    {
        if (disksCount >= 20)
            UpdateAchievement(7);
    }

    public void UpdateAchievement(int id)
    {
        completedAchievements[id] = true;
        extraMenu.AchievementsList[id].SetCompleted();
        UpdateAchievementsCount();
    }

    private void UpdateAchievementsCount()
    {
        AchievementsCount = 0;
        foreach (var achievement in completedAchievements)
            if (achievement)
                AchievementsCount++;
    }
}
