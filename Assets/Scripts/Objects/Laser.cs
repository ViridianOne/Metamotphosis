using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Vector2 activeZonePos, activeZoneSize;
    [SerializeField] private LayerMask playerMask;
    private bool isActiveZone;
    public Transform pos1, pos2;
    public float speed;
    public Transform startPos;
    private Vector3 nextPos;
    void Update()
    {
        isActiveZone = Physics2D.OverlapBox(activeZonePos, activeZoneSize, 0, playerMask);
        if (isActiveZone)
        {
            if (transform.position == pos1.position || nextPos == pos2.position)
            {
                nextPos = pos2.position;
            }
            if (transform.position == pos2.position || nextPos == pos1.position)
            {
                nextPos = pos1.position;
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos1.position, pos2.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(activeZonePos, activeZoneSize);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && MecroSelectManager.instance.GetIndex() != 3)
        {
            Player.instance.DamagePlayer();
        }
    }
}
