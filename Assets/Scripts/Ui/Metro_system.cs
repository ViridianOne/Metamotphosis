using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MapController;

public class Metro_system : MonoBehaviour
{
    public bool isOpen = false;

    [SerializeField] private GameObject mapController;
    [SerializeField] public Pause_menu menu;

    public GameObject metroMenu;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (menu.isPaused)
            {
                return;
            }
            else
            {
                if (isOpen)
                {
                    isOpen = false;
                    Close();
                }
                else
                {
                    isOpen = true;
                    Open();
                }
            }
        }
    }

    private void Open()
    {
        metroMenu.SetActive(true);
        mapController.SetActive(true);
    }

    private void Close()
    {
        metroMenu.SetActive(false);
        mapController.SetActive(false);
    }
}
