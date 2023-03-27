using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockLight : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    public CheckArea checkAreaScript;
    public float timeRemainingBeforeRespawning = 0;
    public float speedRespawn;
    public bool lightOn = false;
    public MecroSelectManager index;
    void Start()
    {
        nextPos = startPos.position;
    }
    void Update()
    {
        if (index.instantiatedMecros[index.GetIndex()].isAbilityActivated)
        {
            lightOn = index.instantiatedMecros[index.GetIndex()].isAbilityActivated;
        }
        else
        {
            lightOn = false;
        }
        if (transform.position == pos1.position && lightOn)
        {
            nextPos = pos2.position;
            timeRemainingBeforeRespawning = 0;
        }
        if (transform.position == pos2.position && lightOn == true && timeRemainingBeforeRespawning==0)
        {
            timeRemainingBeforeRespawning = 5;
        }
        if (checkAreaScript.StartMoving() == true && lightOn)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
        if (timeRemainingBeforeRespawning > 0)
        {
            //nextPos = pos1.position;
            //transform.position = Vector3.MoveTowards(transform.position, nextPos, speedRespawn * Time.deltaTime);
            timeRemainingBeforeRespawning -= Time.deltaTime;
            if (timeRemainingBeforeRespawning < 3 && lightOn)
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