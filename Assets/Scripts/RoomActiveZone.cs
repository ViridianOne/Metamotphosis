using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomActiveZone : MonoBehaviour
{
    private static readonly List<RoomActiveZone> currentRooms = new (4);

    public static void RecoverEnemies()
    {
        foreach (var room in currentRooms)
        {
            if (room.roomEnemiesManager != null)
                room.roomEnemiesManager.RecoverEnemies();
        }
    }

    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private RoomObjectsManager roomObjectsManager;
    [SerializeField] private RoomEnemiesManager roomEnemiesManager;
    private bool isInRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInRoom && other.CompareTag("Player") && !other.isTrigger)
        {
            isInRoom = true;
            currentRooms.Add(this);
            virtualCamera.SetActive(true);
            if (roomObjectsManager != null)
                roomObjectsManager.PlaceObjects();
            if (roomEnemiesManager != null)
                roomEnemiesManager.ActivateEnemies();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!MecroSelectManager.instance.isChanged && other.CompareTag("Player") && !other.isTrigger)
        {
            currentRooms.Remove(this);
            virtualCamera.SetActive(false);
            if (roomObjectsManager != null)
                roomObjectsManager.RecycleObjects();
            if (roomEnemiesManager != null)
                roomEnemiesManager.DeactivateEnemies();
            isInRoom = false;
        }
    }
}
