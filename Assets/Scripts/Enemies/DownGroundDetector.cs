using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownGroundDetector : MonoBehaviour
{
    [SerializeField] private Blob blob;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground"))
        {
            blob.isOnGround = true;
            print(collision.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            blob.isOnGround = false;
        }
    }
}
