using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Form_switch_controller;

public class Pause_menu : MonoBehaviour
{
    public enum ActiveButtonState
    {
        None,
        Continue,
        Settings,
        Help,
        Map,
        Extras,
        Authors,
        Delete,
        Quit
    }

    public ActiveButtonState activeButtonState;

    public bool isPaused = false;
    private GameObject activePage = null;

    public GameObject formSwitchController;

    public GameObject pauseMenuButtons;
    public GameObject settingsPage;
    public GameObject helpPage;
    public GameObject mapPage;
    public GameObject extrasPage;
    public GameObject authorsPage;
    public GameObject deletePage;
    public GameObject quitPage;

    public GameObject mapContainer;

    public GameObject pauseBG;
    public DialogueSystem dialogues;

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if(activePage != null)
        {
            activePage.SetActive(false);
            activeButtonState = ActiveButtonState.None;
            activePage = null;
            mapContainer.SetActive(false);
            
        }
        pauseMenuButtons.SetActive(false);
        pauseBG.SetActive(false);
        if (!dialogues.dialogueStarted)
            formSwitchController.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        pauseBG.SetActive(true);
        pauseMenuButtons.SetActive(true);
        formSwitchController.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings()
    {
        if (activeButtonState == ActiveButtonState.Settings)
        {
            if (activePage == settingsPage)
            {
                activePage.SetActive(false);
                activePage = null;
                return;
            }
            else if (activePage != null)
            {
                activePage.SetActive(false);
            }
            settingsPage.SetActive(true);
            activePage = settingsPage;
        }
        else
            return;
    }

    public void OpenHelp()
    {
        if (activeButtonState == ActiveButtonState.Help)
        {
            if (activePage == helpPage)
            {
                activePage.SetActive(false);
                activePage = null;
                return;
            }
            else if (activePage != null)
            {
                activePage.SetActive(false);
            }
            helpPage.SetActive(true);
            activePage = helpPage;
            print('s');
        }
        else
            return;
    }

    public void OpenMap()
    {
        if (activeButtonState == ActiveButtonState.Map)
        {
            if (activePage == mapPage)
            {
                activePage.SetActive(false);
                activePage = null;
                mapContainer.SetActive(false);
                return;
            }
            else if (activePage != null)
            {
                activePage.SetActive(false);
            }
            mapContainer.SetActive(true);
            mapPage.SetActive(true);
            activePage = mapPage;
        }
        else
            return;
    }

    public void OpenExtras()
    {
        if (activeButtonState == ActiveButtonState.Extras)
        {
            //if (activePage == extrasPage)
            //{
            //    activePage.SetActive(false);
            //    activePage = null;
            //    return;
            //}
            //else if (activePage != null)
            //{
            //    activePage.SetActive(false);
            //}
            //extrasPage.SetActive(true);
            //activePage = extrasPage;
        }
        else
            return;
    }

    public void OpenAuthors()
    {
        if (activeButtonState == ActiveButtonState.Authors)
        {
            //if (activePage == authorsPage)
            //{
            //    activePage.SetActive(false);
            //    activePage = null;
            //    return;
            //}
            //else if (activePage != null)
            //{
            //    activePage.SetActive(false);
            //}
            //authorsPage.SetActive(true);
            //activePage = authorsPage;
        }
        else
            return;
    }

    public void DeleteProgress()
    {
        if (activeButtonState == ActiveButtonState.Delete)
        {
            if (activePage == deletePage)
            {
                activePage.SetActive(false);
                activePage = null;
                return;
            }
            else if (activePage != null)
            {
                activePage.SetActive(false);
            }
            deletePage.SetActive(true);
            activePage = deletePage;
        }
        else
            return;
    }

    public void Quit()
    {
        if (activeButtonState == ActiveButtonState.Quit)
        {
            //if (activePage == quitPage)
            //{
            //    activePage.SetActive(false);
            //    activePage = null;
            //    return;
            //}
            //else if (activePage != null)
            //{
            //    activePage.SetActive(false);
            //}
            //quitPage.SetActive(true);
            //activePage = quitPage;
            Application.Quit();
        }
        else
            return;
        //Application.Quit();
    }
}
