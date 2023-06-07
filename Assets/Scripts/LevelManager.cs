using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Location currentLocation { get; private set; }
    public int currentRoomNumber { get; private set; }
    public Vector3Int currentPositionOnMap { get; private set; }
    [SerializeField] private Light2D globalLight;
    public bool isDarknessOn { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentLocation = Location.location161;
        currentRoomNumber = 1;
        currentPositionOnMap = new Vector3Int(12, -6, 0);
    }

    public void SetMapInfo(Location location, int roomNumber, Vector3Int positionOnMap)
    {
        currentLocation = location;
        currentRoomNumber = roomNumber;
        currentPositionOnMap = positionOnMap;
    }

    public void SetGlobalLightItensity(float intensity)
    {
        globalLight.intensity = intensity;
        isDarknessOn = intensity == 0;
    }
}
