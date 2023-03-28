using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardGroundDetector : MonoBehaviour
{
    [SerializeField] private Blob blob;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            blob.isGroundInFront = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            blob.isGroundInFront = false;
        }
    }
}
