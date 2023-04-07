using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public bool isHole;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.tag == "Player" && !MecroSelectManager.instance.instantiatedMecros[(int)MecroStates.form26]) 
            || (collision.tag == "Player" && isHole ))
        {
            Player.instance.DamagePlayer();
        }
    }
}
