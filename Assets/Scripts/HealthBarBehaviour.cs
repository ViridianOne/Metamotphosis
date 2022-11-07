using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject sliderBox;
    [SerializeField] private Slider slider;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        sliderBox.SetActive(false);
        slider.value = 100;
    }

    private void Update()
    {
        sliderBox.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }

    public void AddValueToJumpSlider(float value)
    {
        slider.value -= value;
    }

    public void ToggleJumpSlider(bool type)
    {
        sliderBox.SetActive(type);
    }
}
