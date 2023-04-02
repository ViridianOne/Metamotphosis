using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampolines : MonoBehaviour
{
    public float JumpForceTrampoline;
    public MecroSelectManager indexTrampoline;
    public bool lightOn = false;
    protected Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && indexTrampoline.GetIndex() != 2)
        {
            if (/*lightOn*/ true)
            { 
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * JumpForceTrampoline, ForceMode2D.Impulse);
                AudioManager.instance.Play(4);
            }
        }
    }

    private void Update()
    {
        if (/*Input.GetButtonDown("Fire1") && */indexTrampoline.GetIndex() == 0)
        {
            lightOn = indexTrampoline.instantiatedMecros[indexTrampoline.GetIndex()].isAbilityActivated;
            //anim.SetBool("isOn", lightOn);
        }
        else
        {
            lightOn = false;
            //anim.SetBool("isOn", lightOn);
        }
    }
}
