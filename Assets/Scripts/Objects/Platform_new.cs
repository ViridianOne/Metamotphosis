using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_new : MonoBehaviour
{
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    [SerializeField] private Animator anim;
    public bool isSleeping;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        nextPos = startPos.position;
        anim.SetFloat("sleepingCoef", 0);
        anim.SetBool("isSleeping", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == pos1.position)
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
            anim.SetTrigger("impulse");
            anim.SetBool("isPlayerOnPlatform", true);
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
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && (collision.gameObject.activeInHierarchy || MecroSelectManager.instance.isChanged))
        {
            anim.SetBool("isPlayerOnPlatform", false);
            collision.collider.transform.SetParent(null);
        }
    }
}
