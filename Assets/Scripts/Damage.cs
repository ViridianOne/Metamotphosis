using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public bool isHole;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && MecroSelectManager.instance.GetIndex() != (int)MecroStates.form26 
            || isHole && other.CompareTag("Shield"))
        {
            Player.instance.DamagePlayer();
        }
    }
}
