using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckArea : MonoBehaviour
{
    public bool playerInRange;
    public static CheckArea instance;
    // Start is called before the first frame update
    private void Start()
    {
        instance = this;
        playerInRange = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public bool StartMoving ()
    {
        return playerInRange;
    }
}
