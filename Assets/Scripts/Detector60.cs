using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector60 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            Player.instance.isOn60 = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            Player.instance.isOn60 = false;
        }
    }
}
