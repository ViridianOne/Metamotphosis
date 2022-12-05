using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadEnemyPlayerDetector : MonoBehaviour
{
    [SerializeField] private GameObject enemyInstance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            enemyInstance.GetComponent<Enemy1>().TakeDamage();
        }
    }
}
