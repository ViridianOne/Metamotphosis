using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platfrom341Detector : MonoBehaviour
{
    public bool isRight;
    public int coefficient = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ledge Grabbing")
        {
            
                
        }
    }
}
