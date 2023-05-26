using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector30 : MonoBehaviour
{
    private bool turnSwitch = false;
    [SerializeField] private bool isConvex;
    [SerializeField] private float gravityChangeCoef;
    [SerializeField] private bool isOnFirstOrThirdQuater;
    private Vector2 preivousTurn, currentTurn = Vector2.zero;
    [SerializeField] private Vector2 possibleClockwiseDirection;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            if (Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0 && !turnSwitch)
            {
                currentTurn = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (currentTurn != preivousTurn && 
                    (currentTurn == possibleClockwiseDirection || currentTurn == possibleClockwiseDirection * -1))
                {
                    Player.instance.isOn60 = !Player.instance.isOn60;
                    Player.instance.isOn30 = !Player.instance.isOn60;
                    Player.instance.isVertical = !isOnFirstOrThirdQuater ? Player.instance.isVertical : !Player.instance.isVertical;
                    Player.instance.ceilCoef = !Player.instance.isVertical ? 1 : -1;
                    if (isConvex)
                    {
                        Player.instance.transform.position += new Vector3(currentTurn.x * gravityChangeCoef, currentTurn.y * gravityChangeCoef, 0f);
                    }
                    preivousTurn = currentTurn;
                    turnSwitch = true;
                }
            }
        }
    }

    private void Update()
    {
        if (!Player.instance.isActive)
            preivousTurn = Vector2.zero;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Magnet"))
        {
            turnSwitch = false;
        }
    }
}
