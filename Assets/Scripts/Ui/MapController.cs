using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    centralLocation,
    None,
}

public class MapController : MonoBehaviour
{
    public static MapController instance;

    public Location PlayerLocation;
    public Vector3Int PlayerPosition;
    [SerializeField] Tilemap Rooms;
    [SerializeField] Tilemap Sectors;
    [SerializeField] Tilemap SpecialRooms;
    [SerializeField] Tilemap Player;

    [SerializeField] Sprite[] PlayerSprites;
    [SerializeField] Sprite[] RoomsSprites;

    List<List<Vector3Int>> SectorsCoordinates;

    [SerializeField]
    Map_text_controller[] MapTexts;


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

            PlayerLocation = Location.None;
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
        foreach (var text in MapTexts)
        {
            text.SetLocationText(newLocation, roomNumber);
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
        if(PlayerLocation != newLocation && PlayerLocation != Location.None)
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

    public void HighlightStation(Vector3Int position, Location location)
    {
        Player.SetTile(position, new Tile());
        Player.GetTile<Tile>(position).sprite = PlayerSprites[(int)location];

        Player.RefreshTile(position);
    }

    public void UnhighlightStation(Vector3Int position)
    {
        Player.SetTile(position, new Tile());
        Player.RefreshTile(position);
    }
}
