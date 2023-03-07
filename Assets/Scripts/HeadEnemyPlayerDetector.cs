using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadEnemyPlayerDetector : MonoBehaviour
{
    [SerializeField] private GameObject enemyInstance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form206].isAbilityActivated)
        {
            enemyInstance.GetComponent<Enemy1>().TakeDamage();
        }
    }
}
