using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPointObject;
    public GameObject activeCheck;
    public GameObject notActiveCheck;
    public bool currentCheck = false;
    public int index;
    [SerializeField]private int currentIndex;
    [SerializeField]private int previousIndex;
    [SerializeField] Light2D objectLight;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !currentCheck)
        {
            MecroSelectManager.instance.respawnPoint.position = other.transform.position;
            AudioManager.instance.Play(0);
            Checkpoints.instance.currentCheckpoint = index;
            Checkpoints.instance.previousCheckpoint = index - 1;
            currentCheck = true;
            //DataManager.instance.SaveGame();
        }
    }
    public void Update()
    {
        if (index == Checkpoints.instance.currentCheckpoint)
        {
            activeCheck.SetActive(true);
            notActiveCheck.SetActive(false);
        }
        else
        {
            activeCheck.SetActive(false);
            notActiveCheck.SetActive(true);
            currentCheck = false;
        }
        objectLight.intensity = LevelManager.instance.isDarknessOn ? 1 : 0;
    }

}
