using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector30 : MonoBehaviour
{
    private bool turnSwitch = false;
    [SerializeField] private bool isConvex;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.CompareTag("Magnet"))
    //    {
    //        //Player.instance.isOn30 = true;
    //        Player.instance.isOn60 = !Player.instance.isOn60;
    //        Player.instance.isOn30 = !Player.instance.isOn60;
    //        Player.instance.isVertical = transform.localRotation.x == 0 ? Player.instance.isVertical : !Player.instance.isVertical;
    //        Player.instance.ceilCoef = !Player.instance.isVertical ? 1 : -1;
    //    }
    //}


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            //Player.instance.isOn30 = true;
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
            {
                if (!turnSwitch)
                {
                    Player.instance.isOn60 = !Player.instance.isOn60;
                    Player.instance.isOn30 = !Player.instance.isOn60;
                    Player.instance.isVertical = transform.localRotation.x == 0 ? Player.instance.isVertical : !Player.instance.isVertical;
                    Player.instance.ceilCoef = !Player.instance.isVertical ? 1 : -1;
                    turnSwitch = true;
                }
            }
            else
            {
                turnSwitch = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            turnSwitch = false;
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
