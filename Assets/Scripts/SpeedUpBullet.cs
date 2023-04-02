using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            var rb = collision.GetComponent<Rigidbody2D>();
            if(rb != null)
            {
                print("speed up");
            }
        }
    }
}
