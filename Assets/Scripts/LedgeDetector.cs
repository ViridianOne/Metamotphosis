using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    public bool isRight;
    private float coefficient;

    private void Start()
    {
        if (isRight)
            coefficient = -1;
        else
            coefficient = 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player.instance.GrabLedge(transform.position, isRight, coefficient);
        }
    }
}
