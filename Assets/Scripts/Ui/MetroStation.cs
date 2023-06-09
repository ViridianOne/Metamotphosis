using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetroStation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Location location;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        animator.SetFloat("location", (float)location);
    }

    private void Update()
    {
        if(Metro_system.instance.hasPlayerChoosen)
        {
            ToggleMetroStationActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            ToggleMetroStationActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !Metro_system.instance.hasPlayerChoosen)
        {
            ToggleMetroStationActive(false);
        }
    }

    private void ToggleMetroStationActive(bool isOpen)
    {
        animator.SetBool("isOpening", isOpen);
        animator.speed = 1.25f;
        Metro_system.instance.isAbleToOpen = isOpen;
    }
}
