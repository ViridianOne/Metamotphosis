using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ExtrasController : MonoBehaviour
{
    public static ExtrasController instance;

    public TextMeshProUGUI Disks;
    public TextMeshProUGUI Losses;
    public TextMeshProUGUI AchievementsCount;
    public TextMeshProUGUI Time;

    public CustomAchievement[] AchievementsList;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        LevelManager.instance.UpdateAchievements(AchievementsList);
        LevelManager.instance.AchievementsList[3].SetCompleted();
    }

    private void Awake()
    {
        /*
        Disks.text = "<color=#E50049>Disks: </color>" + LevelManager.instance.disksCount + " / " + LevelManager.instance.maxDisksAmount;
        Losses.text = "<color=#E50049>Losses: </color>" + LevelManager.instance.lossesCount;
        AchievementsCount.text = "<color=#E50049>Achievements: </color>" + LevelManager.instance.AchievementsCount + " / " + LevelManager.instance.maxAchievementsAmount;
        Time.text = "<color=#E50049>Time: </color>" + LevelManager.instance.timePlaying.ToString("mm':'ss':'ff");
        */
    }

    // Update is called once per frame
    void Update()
    {
        Disks.text = "<color=#E50049>Disks: </color>" + LevelManager.instance.disksCount + " / " + LevelManager.instance.maxDisksAmount;
        Losses.text = "<color=#E50049>Losses: </color>" + LevelManager.instance.lossesCount;
        AchievementsCount.text = "<color=#E50049>Achievements: </color>" + LevelManager.instance.AchievementsCount + " / " + LevelManager.instance.maxAchievementsAmount;
        Time.text = "<color=#E50049>Time: </color>" + LevelManager.instance.timePlaying.ToString("mm':'ss':'ff");
    }

}
