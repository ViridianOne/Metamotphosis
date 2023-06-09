using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_menu : MonoBehaviour
{
    private bool isLoadingScene = false;
    [SerializeField] private string[] locations;
    [SerializeField] private AudioSource music;
    [SerializeField] private TextMeshProUGUI tmp;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isLoadingScene)
        {
            isLoadingScene = true;
            tmp.text = "Loading...";
            music.Stop();

            var lastLocation = DataManager.instance.GetLocation();
            if (lastLocation == null)
            {
                DataManager.instance.NewGame();
                lastLocation = (int)Location.location161;
            }

            SceneManager.LoadSceneAsync(locations[(int)lastLocation]);
        }
    }


    //То, что ниже, использовать не нужно
    public void OpenLocation161()
    {
        music.Stop();
        SceneManager.LoadScene("location161");
    }

    public void OpenLocation71()
    {
        music.Stop();
        SceneManager.LoadScene("location71");
    }

    public void OpenLocation341()
    {
        music.Stop();
        SceneManager.LoadScene("location341");
    }
}
