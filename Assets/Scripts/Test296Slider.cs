using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test296Slider : MonoBehaviour
{
    int progress = 0;
    public Slider slider;

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            slider.value--;
        }
    }

}
