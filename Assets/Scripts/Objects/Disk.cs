using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk : MonoBehaviour
{
    public DiskCollection diskCollection;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            diskCollection.DiskCollected();
            gameObject.SetActive(false);
        }
    }
}
