using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroStation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Location location; 
    [SerializeField] private Location prevoiusLocation = Location.None;
    [SerializeField] private GameObject tip;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        animator.SetFloat("location", (float)location);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(prevoiusLocation == Location.None 
                || (prevoiusLocation != Location.None && LevelManager.instance.completedLocations[(int)prevoiusLocation]))
                ToggleMetroStationActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !Metro_system.instance.hasPlayerChoosen)
        {
            if (prevoiusLocation == Location.None
                || (prevoiusLocation != Location.None && LevelManager.instance.completedLocations[(int)prevoiusLocation]))
                ToggleMetroStationActive(false);
        }
    }

    private void ToggleMetroStationActive(bool isOpen)
    {
        animator.SetBool("isOpening", isOpen);
        animator.speed = 1.25f;
        Metro_system.instance.isAbleToOpen = isOpen;
        tip.SetActive(isOpen);
    }
}
