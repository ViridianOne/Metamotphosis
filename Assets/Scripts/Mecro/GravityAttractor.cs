using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    [SerializeField] private float gravity;
    private bool isActive;
    void Start()
    {
        
    }

    void Update()
    {
        if (isActive)
            Attract();
    }

    public void Attract()
    {
        var gravityUp = (Player.instance.transform.position - transform.position).normalized;

        Player.instance.AddGravityForce(gravityUp * gravity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Magnet")
        {
            isActive = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Magnet")
        {
            isActive = false;
        }
    }
}
