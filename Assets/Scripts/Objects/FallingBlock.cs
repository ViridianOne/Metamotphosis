using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    public CheckArea checkAreaScript;
    public float timeRemainingBeforeRespawning = 0;
    public float speedRespawn;
    void Start()
    {
        nextPos = startPos.position;
    }
    void Update()
    {
        if (transform.position == pos1.position)
        {
            nextPos = pos2.position;
            timeRemainingBeforeRespawning = 0;
        }
        /*if (transform.position == pos2.position)
        {
            nextPos = pos1.position;
        }*/
        if (checkAreaScript.StartMoving() == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
        if (timeRemainingBeforeRespawning>0)
        {
            //nextPos = pos1.position;
            //transform.position = Vector3.MoveTowards(transform.position, nextPos, speedRespawn * Time.deltaTime);
            timeRemainingBeforeRespawning -= Time.deltaTime;
            if (timeRemainingBeforeRespawning<3)
            {
                nextPos = pos1.position;
                transform.position = Vector3.MoveTowards(transform.position, nextPos, speedRespawn * Time.deltaTime);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Player.instance.DamagePlayer();
            timeRemainingBeforeRespawning = 5;
        }
    }
}
