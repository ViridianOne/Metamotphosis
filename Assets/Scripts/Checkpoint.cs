using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //public GameObject respawnPointObject;
    public Transform respawnPointObject;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            respawnPointObject.position = other.transform.position;
            //respawnPoint = other.transform;//transform.position;
            /*Mecro161.instance.respawnPoint = gameObject.transform;
            Mecro296.instance.respawnPoint = gameObject.transform;
            Mecro71.instance.respawnPoint = gameObject.transform;
            var respawnPoint = gameObject.transform;
            MecroSelectManager.instance.respawnPoint = respawnPoint;
            //Player.instance.respawnPoint = respawnPoint;
            Mecro161.instance.respawnPoint = respawnPoint;
            Mecro296.instance.respawnPoint = respawnPoint;
            Mecro71.instance.respawnPoint = respawnPoint;*/
        }
    }
}
