using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private GameData data;
    [SerializeField] private Transform firstCheckpoint;
    private bool[] mecroFormsAvailability = {true, false, false, false};

    private List<IDataPersistance> dataPersistances;

    [SerializeField] private string fileName;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistances = FindAllDataPersistances();
        LoadGame();
    }

    public void NewGame()
    {
        data = new GameData(firstCheckpoint.position, mecroFormsAvailability);
        dataHandler.Save(data);
        LoadGame();
    }

    public void LoadGame()
    {
        data = dataHandler.Load();

        if(data == null)
        {
            NewGame();
        }

        foreach(var persistance in dataPersistances)
        {
            persistance.LoadData(data);
        }
    }

    public void SaveGame()
    {
        foreach (var persistance in dataPersistances)
        {
            persistance.SaveData(ref data);
        }

        dataHandler.Save(data);
    }

    private List<IDataPersistance> FindAllDataPersistances()
    {
        IEnumerable<IDataPersistance> newDataPersistances = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistance>();
        return new List<IDataPersistance>(newDataPersistances);
    } 
}
