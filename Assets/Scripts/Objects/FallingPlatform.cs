using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    public bool playerTouchedPlatform = false;
    public float timeRemaining = 5;
    public SpriteRenderer platform;
    public PlatformEffector2D effector;
    public Collider2D collider2;
    public float timeRemainingBeforeRespawning = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTouchedPlatform)
        {
            if (timeRemaining>0)
            {
                timeRemaining -= Time.deltaTime;
            }
        }
        if (timeRemaining<=0 && timeRemainingBeforeRespawning == 0)
        {
            collider2.enabled = false;
            platform.enabled = false;
            effector.enabled = false;
            timeRemainingBeforeRespawning = 15;
            playerTouchedPlatform = false;
        }
        if (playerTouchedPlatform == false && timeRemaining<=0)
        {
            if (timeRemainingBeforeRespawning > 0)
            { 
                timeRemainingBeforeRespawning -= Time.deltaTime; 
            }
            if (timeRemainingBeforeRespawning < 10)
            {
                collider2.enabled = true;
                effector.enabled = true;
                platform.enabled = true;
            }
        }
        if (timeRemainingBeforeRespawning<0)
        {
            timeRemaining = 5;
            timeRemainingBeforeRespawning = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTouchedPlatform = true;
        }
    }
}
