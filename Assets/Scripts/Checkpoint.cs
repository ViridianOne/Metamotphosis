using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //public GameObject respawnPointObject;
    public Transform respawnPointObject;
    public GameObject activeCheck;
    public GameObject notActiveCheck;
    public bool currentCheck = false;
    //public Sprite notActiveCheck;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            respawnPointObject.position = other.transform.position;
            currentCheck = true;
            AudioManager.instance.Play(0);
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentCheck = false;
    }
    public void Update()
    {
        if (currentCheck)
        {
            activeCheck.SetActive(true);
            notActiveCheck.SetActive(false);
        }
        else
        {
            activeCheck.SetActive(false);
            notActiveCheck.SetActive(true);
        }
    }
}
