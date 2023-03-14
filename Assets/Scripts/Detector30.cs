using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector30 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Magnet"))
        {
            //Player.instance.isOn30 = true;
            Player.instance.isOn60 = !Player.instance.isOn60;
            Player.instance.isOn30 = !Player.instance.isOn60;
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Magnet"))
    //    {
    //        Player.instance.isOn30 = false;
    //    }
    //}
}
