using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;

    void Start()
    {
        nextPos = startPos.position;
        
    }

    // Update is called once per frame
    void Update()
    {        
        if (transform.position == pos1.position )
        {
            nextPos = pos2.position;
        }
        if (transform.position == pos2.position)
        {
            nextPos = pos1.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //if (transform.childCount == 0)
            //{
            //    collision.collider.transform.SetParent(transform);
            //}
            //else
            //{
            //    transform.DetachChildren();
            //    collision.collider.transform.SetParent(transform);
            //}
            if (nextPos == pos1.position && pos1.position != pos2.position)
            {
                Player.instance.isOnMovingPlatform = true;
                Player.instance.movingPlatDif = new Vector2(0, 0.005f);
            }
            collision.collider.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && (collision.gameObject.activeInHierarchy || MecroSelectManager.instance.isChanged))
        {
            //Player.instance.isOnMovingPlatform = false;
            //Player.instance.movingPlatDif = Vector2.zero;
            collision.collider.transform.SetParent(null);
        }
    }
}
