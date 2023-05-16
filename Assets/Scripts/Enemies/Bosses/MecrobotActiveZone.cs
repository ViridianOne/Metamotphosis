using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MecrobotActiveZone : MonoBehaviour
{
    private Mecrobot boss;
    [SerializeField] private GameObject bossGroup;
    [SerializeField] private GameObject bossRoomEnter;
    [SerializeField] private Transform startPlayerPos;

    private void Start()
    {
        boss = bossGroup.GetComponentInChildren<Mecrobot>();
        bossGroup.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (!boss.IsFightStarted)
            {
                bossGroup.SetActive(true);
                bossRoomEnter.SetActive(true);
                Player.instance.SetPosition(startPlayerPos.position);
                boss.RestoreInitialStates();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            if (boss.IsPlayerDefeated)
            {
                bossRoomEnter.SetActive(false);
                bossGroup.SetActive(false);
            }
        }
    }
}
