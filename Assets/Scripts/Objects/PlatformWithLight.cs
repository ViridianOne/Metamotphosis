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
        if (index.GetIndex() == 0)
        {
            if (index.instantiatedMecros[index.GetIndex()].isAbilityActivated)
            {
                lightsOn = index.instantiatedMecros[index.GetIndex()].isAbilityActivated;
                //anim.SetBool("isSleeping", false);
                //anim.SetTrigger("impulse");
                nextPos = transform.position;
            }
            else
            {
                lightsOn = false;
                //anim.SetBool("impulse", false);
                //anim.SetBool("isSleeping", true);
                nextPos = transform.position;
            }

            if (checkAreaScript.StartMoving() && lightsOn)
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
            //anim.SetBool("isSleeping",true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //anim.SetBool("isPlayerOnPlatform", true);
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
            //anim.SetBool("isPlayerOnPlatform", false);
        }
    }
}

