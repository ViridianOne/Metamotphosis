using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecrobotActiveZone : MonoBehaviour
{
    private Mecrobot boss;
    [SerializeField] private GameObject bossGroup;
    [SerializeField] private GameObject bossRoomEnter;
    [SerializeField] private Collider2D enterDetector;
    [SerializeField] private Transform startPlayerPos;

    [SerializeField] private Location location;
    [SerializeField] private int roomNumber;
    [SerializeField] private Vector3Int positionOnMap; 

    private void Start()
    {
        boss = bossGroup.GetComponentInChildren<Mecrobot>();
        bossGroup.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LevelManager.instance.isBossDefeated)
        {
            enterDetector.enabled = false;
            return;
        }
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (!boss.IsFightStarted)
            {
                bossGroup.SetActive(true);
                enterDetector.enabled = true;
                boss.RestoreInitialStates();
                LevelManager.instance.SetMapInfo(location, roomNumber, positionOnMap);
                // Player.instance.respawnPoint = startPlayerPos;
                Physics2D.IgnoreLayerCollision(9, 13, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (boss.IsPlayerDefeated || Player.instance.isInBossRoom)
            {
                boss.DamagePlayer();
                bossGroup.SetActive(false);
            }
            if (boss.IsPlayerDefeated || boss.isBossDefeated)
            {
                bossRoomEnter.SetActive(false);
                enterDetector.enabled = false;
                Physics2D.IgnoreLayerCollision(9, 13, false);
            }
        }
    }
}
