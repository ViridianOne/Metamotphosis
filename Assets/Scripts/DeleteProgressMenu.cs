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
        //DataManager.instance.NewGame();
        Checkpoints.instance.currentCheckpoint = 0;
        Player.instance.respawnPoint.position = Checkpoints.instance.first.position;
        Player.instance.transform.position = Checkpoints.instance.first.position;
        menu.Resume();
    }

    public void CancelDeleting()
    {
        menu.Resume();
    }
}
