using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private int musicIndex;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !AudioManager.instance.sounds[musicIndex].source.isPlaying)
        {
            AudioManager.instance.Play(musicIndex);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && AudioManager.instance.sounds[musicIndex].source.isPlaying)
        {
            AudioManager.instance.Stop(musicIndex);
        }
    }
}
