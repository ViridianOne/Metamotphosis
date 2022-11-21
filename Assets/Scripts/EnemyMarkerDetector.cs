using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMarkerDetector : MonoBehaviour
{
    [SerializeField] private GameObject enemyInstance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy Marker")
        {
            enemyInstance.GetComponent<Enemy1>().ChangeMoveDirection();
        }
    }
}
