using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    public bool isRight;
    public bool isOnPlatform;
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
        if(collision.tag == "Ledge Grabbing" && !Player.instance.isTouchingLedge)
        {
            if (isOnPlatform)
            {
                Player.instance.transform.SetParent(transform.parent);
                Player.instance.GrabLedge(transform.localPosition, isRight, coefficient, isOnPlatform);
            }
            else
                Player.instance.GrabLedge(transform.position, isRight, coefficient, isOnPlatform);
        }
    }
}
