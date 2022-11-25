using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    public Image buttonImage;
    public Sprite selectedSprite;
    public Sprite normalSprite;
    public Sprite clickedSprite;

    public TextMeshProUGUI buttonText;
    public Color normal, selected, clicked;

    public void ChangeSprite()
    {
        buttonImage.sprite = selectedSprite;
        buttonText.color = selected;
    }

    public void ReturnSprite()
    {
        buttonImage.sprite = normalSprite;
        buttonText.color = normal;
    }

    public void Highlight()
    {
        buttonImage.sprite = clickedSprite;
        buttonText.color = clicked;
    }
}
