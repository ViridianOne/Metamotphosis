using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActiveZone : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = gameObject.GetComponentInChildren<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            enemy.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            enemy.gameObject.SetActive(false);
        }
    }
}
