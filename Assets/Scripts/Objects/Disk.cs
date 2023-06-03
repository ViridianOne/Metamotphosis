using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disk : MonoBehaviour
{
    public DiskCollection diskCollection;
    public TextMesh textMesh;
    private float timer;
    private void Update()
    {
        if (timer>0f)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0f)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            diskCollection.DiskCollected();
            timer = 5f;
            if (textMesh.text != null)
            {
                textMesh.text += "Disks collected: " + diskCollection.getDiskCount().ToString();
            }
            gameObject.GetComponent<Collider2D>().enabled = !gameObject.GetComponent<Collider2D>().enabled;
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            //gameObject.SetActive(false);
        }
    }
}
