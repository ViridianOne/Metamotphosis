using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Pause_menu;

public class DeleteProgressMenu : MonoBehaviour
{
    public Pause_menu menu;
    public void DeleteSaves()
    {
        DataManager.instance.NewGame();
        menu.Resume();
    }

    public void CancelDeleting()
    {
        menu.Resume();
    }
}
