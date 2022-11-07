using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_menu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;

    // Update is called once per frame
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
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    protected void OpenSettings()
    {
        DoNothing();
    }

    protected void OpenHelp()
    {
        DoNothing();
    }

    protected void OpenMap()
    {
        DoNothing();
    }

    protected void OpenExtras()
    {
        DoNothing();
    }

    protected void OpenAuthors()
    {
        DoNothing();
    }

    protected void DeleteProgress()
    {
        DoNothing();
    }

    protected void Quit()
    {
        DoNothing();
        Application.Quit();
    }

    private void DoNothing()
    {
        Debug.Log("Текст для проверки");
    }
}
