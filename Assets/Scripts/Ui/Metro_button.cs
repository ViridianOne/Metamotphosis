using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Metro_buttons_contoller;
using UnityEngine.SceneManagement;

public class Metro_button : MonoBehaviour
{
    [Header("Metro System")]
    public MapController mapController;
    public Metro_buttons_contoller controller;

    [Header("Map and Metro Station")]
    public Location stationLocation;
    public int stationNumber;
    public Vector3Int stationPosition;
    public Color stationColor;
    public string locationScene;
    public Vector3 metroStationPos;

    public void PointerEnter()
    {
        mapController.HighlightStation(stationPosition, stationLocation);
    }

    public void PointerLeave()
    {
        if(controller.clicked_button != this)
            mapController.UnhighlightStation(stationPosition);
    }

    public void PointerClick()
    {
        if (controller.clicked_button != this)
        {
            mapController.MovePlayer(stationLocation, stationNumber, stationPosition);
            controller.clicked_button = this;
            controller.isClicked = true;
            Metro_system.instance.TransportPlayer(locationScene);
        }
        else
            return;

    }
}
