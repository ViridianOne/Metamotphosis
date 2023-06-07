using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_menu : MonoBehaviour
{
    [SerializeField] private string location161, location71, location341;
    [SerializeField] private AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Return))
        {
            music.Stop();
            SceneManager.LoadScene(nextScene);
        }*/
    }

    public void OpenLocation161()
    {
        music.Stop();
        SceneManager.LoadScene(location161);
    }

    public void OpenLocation71()
    {
        music.Stop();
        SceneManager.LoadScene(location71);
    }

    public void OpenLocation341()
    {
        music.Stop();
        SceneManager.LoadScene(location341);
    }
}
