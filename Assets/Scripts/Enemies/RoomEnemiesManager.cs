using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEnemiesManager : MonoBehaviour
{
    private Enemy[] enemies;

    private void Awake()
    {
        enemies = gameObject.GetComponentsInChildren<Enemy>();
        DeactivateEnemies();
    }

    public void ActivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            if (!enemy.isDefeated)
                enemy.SetActive(true);
        }
    }

    public void DeactivateEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }

    public void RecoverEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.Recover();
        }
    }
}
