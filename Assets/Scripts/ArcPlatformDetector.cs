using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcPlatformDetector : MonoBehaviour
{
    [SerializeField] private Vector2 zone30Pos, zone30Size, zone60Pos, zone60Size;
    private bool isOn30, isOn60;
    [SerializeField]private LayerMask magnetMask;

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
