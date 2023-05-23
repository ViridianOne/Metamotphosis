using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcPlatformDetector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player.instance.isOnArcPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.instance.isOnArcPlatform = false;
        }
    }
}
