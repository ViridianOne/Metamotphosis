using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Pause_menu;

public class DeleteProgressMenu : MonoBehaviour
{
    public Pause_menu menu;
    public void DeleteSaves()
    {
        DataManager.instance.NewGame();
        SceneManager.LoadSceneAsync("location161");
        menu.Resume();
    }

    public void CancelDeleting()
    {
        menu.Resume();
    }
}
