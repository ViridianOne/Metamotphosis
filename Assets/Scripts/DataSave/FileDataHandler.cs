using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class FileDataHandler
{
    private readonly string dataPath = "";
    private readonly string dataFileName = "";

    public FileDataHandler(string dataPath, string dataFileName)
    {
        this.dataPath = dataPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        GameData loaded = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using(var stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(var reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loaded = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception error)
            {
                Debug.LogError("Error with loading data to file: " + fullPath + "\n" + error);
            }
        }
        return loaded;
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception error)
        {
            Debug.LogError("Error with saving data to file: " + fullPath + "\n" + error);
        }
    }
}
