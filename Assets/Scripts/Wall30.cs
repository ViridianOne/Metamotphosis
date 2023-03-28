using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall30 : MonoBehaviour
{
    private bool turnSwitch = false;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Magnet"))
        {
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0)
            {
                if (!turnSwitch)
                {
                    Player.instance.isOn60 = !Player.instance.isOn60;
                    Player.instance.isOn30 = !Player.instance.isOn60;
                    Player.instance.isVertical = transform.localRotation.x == 0 ? Player.instance.isVertical : !Player.instance.isVertical;
                    Player.instance.ceilCoef = !Player.instance.isVertical ? 1 : -1;
                    Player.instance.transform.position += new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f);
                    turnSwitch = true;
                }
            }
            else
            {
                turnSwitch = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Magnet"))
        {
            turnSwitch = false;
        }
    }
}

