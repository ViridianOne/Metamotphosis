using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public static VictoryScreen instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("Main menu");
            }
        }
    }
}
