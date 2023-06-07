using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static MapController;

public class Metro_system : MonoBehaviour
{
    public static Metro_system instance;

    [HideInInspector] public bool isOpen = false;
    [HideInInspector] public bool isAbleToOpen;
    [HideInInspector] public bool hasPlayerChoosen;

    public string currentScene;

    [SerializeField] private GameObject mapController;
    [SerializeField] public Pause_menu menu;

    public GameObject metroMenu;
    public GameObject formSwitchController;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAbleToOpen)
        {
            if (Input.GetKeyDown(KeyCode.M) && !menu.isPaused && !menu.dialogues.dialogueStarted && !hasPlayerChoosen)
            {
                ToggleActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Escape) && !menu.isPaused && !menu.dialogues.dialogueStarted)
            {
                ToggleActive(false);
            }
        }
    }

    public void ToggleActive(bool isActive)
    {
        metroMenu.SetActive(isActive);
        mapController.SetActive(isActive);
        formSwitchController.SetActive(!isActive);
        Time.timeScale = isActive ? 0 : 1;
        isOpen = isActive;
    }

    public void TransportPlayer(string locationScene)
    {
        hasPlayerChoosen = true;
        ToggleActive(false);
        if (locationScene != currentScene)
        {
            hasPlayerChoosen = false;
            SceneManager.LoadScene(locationScene);
        }
    }
}
