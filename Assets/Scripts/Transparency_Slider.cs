using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Transparency_Changer : MonoBehaviour
{
    public Image image;
    public float alpha = 1f;

    void ChangeAlpha(float alphaVal)
    {
        Color oldColor = image.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        image.color = newColor;
    }

    public void ChangeAlphaOnValueChange(Slider slider)
    {
        ChangeAlpha( slider.value);
    }

}
