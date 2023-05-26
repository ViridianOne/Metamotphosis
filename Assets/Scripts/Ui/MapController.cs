using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public enum Location
{
    location26,
    location71,
    location116,
    location161,
    location206,
    location251,
    locztion296,
    location341,
    locationCenter,
    None,
}

public class MapController : MonoBehaviour
{
    public static MapController instance;

    private Location PlayerLocation;
    private Vector3Int PlayerPosition;
    [SerializeField] Tilemap Rooms;
    [SerializeField] Tilemap Sectors;
    [SerializeField] Tilemap SpecialRooms;
    [SerializeField] Tilemap Player;

    [SerializeField] Sprite[] PlayerSprites;
    [SerializeField] Sprite[] RoomsSprites;

    List<List<Vector3Int>> SectorsCoordinates;

    [SerializeField] TextMeshProUGUI SectionText;
    [SerializeField] TextMeshProUGUI RoomText;

    private void Awake()
    {
        instance = this;
        PlayerLocation = Location.None;
        SectorsCoordinates = GetSectorsCoordinates();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetBordersColors();
        /*PlayerLocation = Location.None;
        SectorsCoordinates = GetSectorsCoordinates();
        ResetBordersColors();*/
        //MovePlayer(Location.location161, "161-5", new Vector3Int(11, -8, 0));
        //MovePlayer(Location.location161, "161-2", new Vector3Int(11, -7, 0));
        //MovePlayer(Location.location341, "341-9", new Vector3Int(8, 5, 0));
    }


    /// <summary>
    /// Устанавливает позицию и локацию игрока. 
    /// </summary>
    /// <param name="newLocation">Новая локация игрока</param>
    /// <param name="position">Новые координаты игрока на карте</param>
    public void MovePlayer(Location newLocation, int roomNumber, Vector3Int position)
    {
        if (PlayerLocation == Location.None)
        {
            Highlight_Sector(newLocation);

            PlayerLocation = newLocation;
            PlayerPosition = position;

            Player.SetTile(position, new Tile());
            Player.GetTile<Tile>(position).sprite = PlayerSprites[(int)PlayerLocation];
            Player.RefreshTile(position);
        }
        else
        {
            if (newLocation != PlayerLocation)
            {
                Highlight_Sector(newLocation);
                PlayerLocation = newLocation;

            }
            Player.SetTile(position, new Tile());
            Player.GetTile<Tile>(position).sprite = PlayerSprites[(int)PlayerLocation];
            Player.RefreshTile(position);

            Player.SetTile(PlayerPosition, new Tile());
            Player.RefreshTile(PlayerPosition);

            PlayerPosition = position;
        }
        SetLocationText(newLocation, roomNumber);
    }


    private void SetLocationText(Location location, int roomNumber)
    {
        switch (location)
        {
            case Location.location26:
                SectionText.text = "Location: " + "<color=#E56300>26</color>";
                RoomText.text = "Room: " + "<color=#E56300>26-" + roomNumber + "</color>";
                break;

            case Location.location71:
                SectionText.text = "Location: " + "<color=#BBE500>71</color>";
                RoomText.text = "Room: " + "<color=#BBE500>71-" + roomNumber + "</color>";
                break;

            case Location.location116:
                SectionText.text = "Location: " + "<color=#0FE500>116</color>";
                RoomText.text = "Room: " + "<color=#0FE500>116-" + roomNumber + "</color>";
                break;

            case Location.location161:
                SectionText.text = "Location: " + "<color=#00E59C>161</color>";
                RoomText.text = "Room: " + "<color=#00E59C>161-" + roomNumber + "</color>";
                break;

            case Location.location206:
                SectionText.text = "Location: " + "<color=#0082E5>206</color>";
                RoomText.text = "Room: " + "<color=#0082E5>206-" + roomNumber + "</color>";
                break;

            case Location.location251:
                SectionText.text = "Location: " + "<color=#2A00E5>251</color>";
                RoomText.text = "Room: " + "<color=#2A00E5>251-" + roomNumber + "</color>";
                break;

            case Location.locztion296:
                SectionText.text = "Location: " + "<color=#D600E5>296</color>";
                RoomText.text = "Room: " + "<color=#D600E5>296-" + roomNumber + "</color>";
                break;

            case Location.location341:
                SectionText.text = "Location: " + "<color=#E50049>341</color>";
                RoomText.text = "Room: " + "<color=#E50049>341-" + roomNumber + "</color>";
                break;

            case Location.locationCenter: //цвета и номера для локаций центра нет в фигме
                SectionText.text = "Location: " + "<color=#2A00E5>251</color>";
                RoomText.text = "Room: " + "<color=#2A00E5>" + roomNumber + "</color>";
                break;
        }
        
    }


    private void ResetBordersColors()
    {
        foreach (var sector in SectorsCoordinates)
        {
            foreach (var coordinate in sector)
            {
                Sectors.SetColor(coordinate, new Color(0, 0, 0, 0.5f));
                Rooms.GetTile<Tile>(coordinate).sprite = RoomsSprites[0];
                Rooms.RefreshTile(coordinate);
                Sectors.RefreshTile(coordinate);
            }
        }
    }


    private void Highlight_Sector(Location newLocation)
    {
        if (PlayerLocation == newLocation)
            return;
        foreach (var tileCoordinate in SectorsCoordinates[(int)newLocation])
        {
            Sectors.SetColor(tileCoordinate, new Color(1, 1, 1, 1));
            Rooms.GetTile<Tile>(tileCoordinate).sprite = RoomsSprites[1];
            SpecialRooms.SetColor(tileCoordinate, new Color(1, 1, 1, 1));

            Sectors.RefreshTile(tileCoordinate);
            Rooms.RefreshTile(tileCoordinate);
            SpecialRooms.RefreshTile(tileCoordinate);
            //Rooms
            //Sectors
            //SpecialRooms
        }
        if (PlayerLocation != Location.None)
            foreach(var tileCoordinate in SectorsCoordinates[(int)PlayerLocation])
            {
                Sectors.SetColor(tileCoordinate, new Color(0, 0, 0, 0.5f));
                Rooms.GetTile<Tile>(tileCoordinate).sprite = RoomsSprites[0];
                SpecialRooms.SetColor(tileCoordinate, new Color(0, 0, 0, 0.5f));

                Sectors.RefreshTile(tileCoordinate);
                Rooms.RefreshTile(tileCoordinate);
                SpecialRooms.RefreshTile(tileCoordinate);
            }
    }


    private static List<List<Vector3Int>> GetSectorsCoordinates()
    {
        var result = new List<List<Vector3Int>>();

        var firstSector = new List<Vector3Int>();
        firstSector.AddRange(GetSectorColumn(11, 3, 9));
        firstSector.AddRange(GetSectorColumn(12, 2, 9));
        firstSector.AddRange(GetSectorColumn(13, 4, 9));
        firstSector.AddRange(GetSectorColumn(14, 6, 9));
        firstSector.AddRange(GetSectorColumn(15, 7, 9));
        firstSector.AddRange(GetSectorColumn(16, 8, 9));
        result.Add(firstSector);

        var secondSector = new List<Vector3Int>();
        secondSector.AddRange(GetSectorColumn(13, 1, 3));
        secondSector.AddRange(GetSectorColumn(14, 1, 5));
        secondSector.AddRange(GetSectorColumn(15, 1, 6));
        secondSector.AddRange(GetSectorColumn(16, 1, 7));
        secondSector.AddRange(GetSectorColumn(17, 1, 9));
        result.Add(secondSector);

        var thirdSector = new List<Vector3Int> ();
        thirdSector.AddRange(GetSectorColumn(13, -2, 0));
        thirdSector.AddRange(GetSectorColumn(14, -4, 0));
        thirdSector.AddRange(GetSectorColumn(15, -5, 0));
        thirdSector.AddRange(GetSectorColumn(16, -6, 0));
        thirdSector.AddRange(GetSectorColumn(17, -8, 0));
        result.Add(thirdSector);

        var fourthSector = new List<Vector3Int>();
        fourthSector.AddRange(GetSectorColumn(11, -8, -2));
        fourthSector.AddRange(GetSectorColumn(12, -8, -1));
        fourthSector.AddRange(GetSectorColumn(13, -8, -3));
        fourthSector.AddRange(GetSectorColumn(14, -8, -5));
        fourthSector.AddRange(GetSectorColumn(15, -8, -6));
        fourthSector.AddRange(GetSectorColumn(16, -8, -7));
        result.Add(fourthSector);

        var fifthSector = new List<Vector3Int>();
        fifthSector.AddRange(GetSectorColumn(5, -8, -7));
        fifthSector.AddRange(GetSectorColumn(6, -8, -6));
        fifthSector.AddRange(GetSectorColumn(7, -8, -5));
        fifthSector.AddRange(GetSectorColumn(8, -8, -3));
        fifthSector.AddRange(GetSectorColumn(9, -8, -1));
        fifthSector.AddRange(GetSectorColumn(10, -8, -2));
        result.Add(fifthSector);

        var sixthSector = new List<Vector3Int>();
        sixthSector.AddRange(GetSectorColumn(4, -8, 0));
        sixthSector.AddRange(GetSectorColumn(5, -6, 0));
        sixthSector.AddRange(GetSectorColumn(6, -5, 0));
        sixthSector.AddRange(GetSectorColumn(7, -4, 0));
        sixthSector.AddRange(GetSectorColumn(8, -2, 0));
        result.Add(sixthSector);

        var seventhSector = new List<Vector3Int>();
        seventhSector.AddRange(GetSectorColumn(4, 1, 9));
        seventhSector.AddRange(GetSectorColumn(5, 1, 7));
        seventhSector.AddRange(GetSectorColumn(6, 1, 6));
        seventhSector.AddRange(GetSectorColumn(7, 1, 5));
        seventhSector.AddRange(GetSectorColumn(8, 1, 3));
        result.Add(seventhSector);

        var eighthSector = new List<Vector3Int>();
        eighthSector.AddRange(GetSectorColumn(5, 8, 9));
        eighthSector.AddRange(GetSectorColumn(6, 7, 9));
        eighthSector.AddRange(GetSectorColumn(7, 6, 9));
        eighthSector.AddRange(GetSectorColumn(8, 4, 9));
        eighthSector.AddRange(GetSectorColumn(9, 2, 9));
        eighthSector.AddRange(GetSectorColumn(10, 3, 9));
        result.Add(eighthSector);

        var ninthSector = new List<Vector3Int>();
        ninthSector.AddRange(GetSectorColumn(9, 0, 1));
        ninthSector.AddRange(GetSectorColumn(10, -1, 2));
        ninthSector.AddRange(GetSectorColumn(11, -1, 2));
        ninthSector.AddRange(GetSectorColumn(12, 0, 1));
        result.Add(ninthSector);

        return result;
    }


    private static List<Vector3Int> GetSectorColumn(int x, int columnStart, int columnEnd)
    {
        var result = new List<Vector3Int>();
        for (int i = columnStart; i <= columnEnd; i++)
        {
            result.Add(new Vector3Int(x, i, 0));
        }
        return result;
    }
}
