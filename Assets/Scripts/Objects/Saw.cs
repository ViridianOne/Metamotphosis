using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    public AudioSource bladeSound;
    /*private void Update()
    {
        if (!bladeSound.isPlaying)
        {
            bladeSound.Play();
        }
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!bladeSound.isPlaying)
            {
                bladeSound.Play();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (bladeSound.isPlaying)
            {
                bladeSound.Stop();
            }
        }
    }
}
