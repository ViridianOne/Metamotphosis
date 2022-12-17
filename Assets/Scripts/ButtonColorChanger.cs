using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Pause_menu;

public class ButtonColorChanger : MonoBehaviour
{
    public Pause_menu pauseMenu;
    public ActiveButtonState buttonState;

    public Image buttonImage;
    public Sprite normalSprite, selectedSprite, clickedSprite;

    public TextMeshProUGUI buttonText;
    public Color normal, selected, clicked;

    public void PointerEnter()
    {
        if (IsActive())
            return;
        ChangeSprite();
    }

    public void PointerLeave()
    {
        if (IsActive())
            return;
        ReturnSprite();
    }

    public void PointerClick()
    {
        if (IsActive())
        {
            ReturnSprite();
            switch (pauseMenu.activeButtonState)
            {
                case ActiveButtonState.Settings:
                    pauseMenu.OpenSettings();
                    break;

                case ActiveButtonState.Help:
                    pauseMenu.OpenHelp();
                    break;

                case ActiveButtonState.Map:
                    pauseMenu.OpenMap();
                    break;

                case ActiveButtonState.Extras:
                    pauseMenu.OpenExtras();
                    break;

                case ActiveButtonState.Authors:
                    pauseMenu.OpenAuthors();
                    break;

                case ActiveButtonState.Delete:
                    pauseMenu.DeleteProgress();
                    break;

                case ActiveButtonState.Quit:
                    pauseMenu.Quit();
                    break;
            }
            pauseMenu.activeButtonState = ActiveButtonState.None;
        }
        else if (pauseMenu.activeButtonState != ActiveButtonState.None)
        {
            return;
        }
        else
        {
            pauseMenu.activeButtonState = buttonState;
            ChangeSprite();
            switch (pauseMenu.activeButtonState)
            {
                case ActiveButtonState.Continue:
                    pauseMenu.Resume();
                    pauseMenu.activeButtonState = ActiveButtonState.None;
                    break;

                case ActiveButtonState.Settings:
                    pauseMenu.OpenSettings();
                    break;

                case ActiveButtonState.Help:
                    pauseMenu.OpenHelp();
                    break;

                case ActiveButtonState.Map:
                    pauseMenu.OpenMap();
                    break;

                case ActiveButtonState.Extras:
                    pauseMenu.OpenExtras();
                    break;

                case ActiveButtonState.Authors:
                    pauseMenu.OpenAuthors();
                    break;

                case ActiveButtonState.Delete:
                    pauseMenu.DeleteProgress();
                    break;

                case ActiveButtonState.Quit:
                    pauseMenu.Quit();
                    break;
            }
        }
    }

    private bool IsActive()
    {
        return pauseMenu.activeButtonState == buttonState;
    }

    private void ChangeSprite()
    {

        buttonImage.sprite = selectedSprite;
        buttonText.color = selected;
    }

    private void ReturnSprite()
    {
        buttonImage.sprite = normalSprite;
        buttonText.color = normal;
    }

    private void Highlight()
    {
        buttonImage.sprite = clickedSprite;
        buttonText.color = clicked;
    }
}
