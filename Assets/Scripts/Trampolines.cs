using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolines : MonoBehaviour
{
    public float JumpForceTrampoline = 20f;
    public MecroSelectManager indexTrampoline;
    public bool lightOn = false;
    protected Animator anim;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && (indexTrampoline.GetIndex() == 0 || indexTrampoline.GetIndex()==1))
        {
            if (lightOn == true)
            { 
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * JumpForceTrampoline, ForceMode2D.Impulse); 
            }
        }
    }

    private void Update()
    {
        anim = GetComponent<Animator>();
        if (Input.GetButtonDown("Fire1") && indexTrampoline.GetIndex()==0)
        {
            lightOn = !lightOn;
            if (lightOn == true)
            {
                anim.SetBool("isOn", true);
            }
            if (lightOn == false)
            {
                anim.SetBool("isOn", false);
            }
        }
    }
}
