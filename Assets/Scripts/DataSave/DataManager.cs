using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;

    private GameData data;
    private Vector2 startPosition = new (-5.25f, -4.25f);
    private readonly bool[] mecroFormsAvailability = { true, false, false, false, false, false, false, false };
    private readonly bool isBossDefeated = false;
    private readonly int initialLocation = (int)Location.location161;
    private readonly int initialCollectedDisksCount = 0;
    private readonly SerializableDictionary<string, bool> initialCollectedDisks = new();
    private readonly SerializableDictionary<int, bool> initialCompletedLocations = new ();

    private List<IDataPersistance> dataPersistances;

    [SerializeField] private string fileName;
    private FileDataHandler dataHandler;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        dataPersistances = FindAllDataPersistances();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        if (Checkpoints.instance)
            Checkpoints.instance.currentCheckpoint = 0;
        var locationCount = System.Enum.GetNames(typeof(Location)).Length - 2;
        for (var i = 0; i < locationCount; i++)
        {
            initialCompletedLocations[i] = false;
        }

        data = new GameData(startPosition, mecroFormsAvailability, isBossDefeated, 
            initialLocation, initialCollectedDisksCount, initialCollectedDisks, initialCompletedLocations);
        dataHandler.Save(data);

        LoadGame();
    }

    public void LoadGame()
    {
        data = dataHandler.Load();

        if(data == null)
        {
            if (initializeDataIfNull)
                NewGame();
            else
                return;
        }

        foreach(var persistance in dataPersistances)
        {
            persistance.LoadData(data);
        }
    }

    public void SaveGame()
    {
        if (data == null)
            return;

        foreach (var persistance in dataPersistances)
        {
            persistance.SaveData(ref data);
        }

        dataHandler.Save(data);
    }

    private List<IDataPersistance> FindAllDataPersistances()
    {
        var newDataPersistances = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistance>();
        return new List<IDataPersistance>(newDataPersistances);
    } 

    public int? GetLocation()
    {
        return data?.lastLocation;
    }
}
