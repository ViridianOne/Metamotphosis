using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformWithLight : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    protected Animator anim;
    public bool lightsOn = false;
    public CheckArea checkAreaScript;
    public MecroSelectManager index;
    public bool MovingDown = false;
    public bool MovingUp = false;

    void Start()
    {
        nextPos = startPos.position;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (/*checkAreaScript.StartMoving() == true &&*/ index.GetIndex() == 0)
        {
            /*if (Input.GetButtonDown("Fire1"))
            {
                lightsOn = !index.instantiatedMecros[index.GetIndex()].lightSwitcher;
                anim.SetBool("isOn", lightsOn);
                /*if (checkAreaScript.StartMoving() == true)
                {
                    if (transform.position == pos1.position || lightsOn == true)
                    {
                        nextPos = pos2.position;
                        if (transform.position == pos2.position)
                            nextPos = pos1.position;
                    }
                }
                if (lightsOn == false)
                {
                    nextPos = transform.position;
                }
                if (lightsOn == false)
                {
                    nextPos = transform.position;
                }
            }*/
            if (index.instantiatedMecros[index.GetIndex()].lightSwitcher)
            {
                lightsOn = index.instantiatedMecros[index.GetIndex()].lightSwitcher;
                anim.SetBool("isOn", lightsOn);
                nextPos = transform.position;
            }
            else
            {
                lightsOn = false;
                anim.SetBool("isOn", false);
                nextPos = transform.position;
            }

            if (checkAreaScript.StartMoving() == true && lightsOn == true)
            {
                if (transform.position == pos1.position || (lightsOn && MovingDown))
                {
                    MovingDown = true;
                    MovingUp = false;
                    nextPos = pos2.position;
                }
                if (transform.position == pos2.position || (lightsOn && MovingUp))
                {
                    MovingUp = true;
                    MovingDown = false;
                    nextPos = pos1.position;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
        else
        {
            anim.SetBool("isOn", false);
        }
        /*if (transform.position == pos1.position && Input.GetButtonDown("Fire1"))
        {
            nextPos = pos2.position;
            //anim.SetBool("isOn", true);
            lightsOn = !lightsOn;
            anim.SetBool("isOn", lightsOn);
        }*/
        /*if (transform.position == pos2.position && Input.GetButtonDown("Fire1"))
        {
            nextPos = pos1.position;
        }*/
        /*if (lightsOn == true && Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("isOn", false);
            lightsOn = false;
        }*/
        //transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.tag == "Player")
        {
            if (transform.childCount == 0)
            {
                collision.collider.transform.SetParent(transform);
            }
            else
            {
                transform.DetachChildren();
                collision.collider.transform.SetParent(transform);
            }
        }*/
        if (nextPos == pos1.position && pos1.position.y != pos2.position.y)
        {
            Player.instance.isOnMovingPlatform = true;
            Player.instance.movingPlatDif = new Vector2(0, 0.005f);
        }
        else if (nextPos == pos2.position && pos2.position.y != pos1.position.y)
        {
            Player.instance.isOnMovingPlatform = true;
            Player.instance.movingPlatDif = new Vector2(0, -0.005f);
        }
        collision.collider.transform.SetParent(transform);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && (collision.gameObject.activeInHierarchy || MecroSelectManager.instance.isChanged))
        {
            collision.collider.transform.SetParent(null);
        }
    }
}

