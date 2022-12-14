using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Form_switch_controller;

public class FormSwitchIconChanger : MonoBehaviour
{
    public Form_switch_controller controller;
    public State state;

    public Image buttonImage;
    public Sprite normalSprite, selectedSprite, clickedSprite;

    void Start()
    {
        if (controller.playerState == state)
            buttonImage.sprite = clickedSprite;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (state == State.form161 && controller.playerState != state)
            {
                buttonImage.sprite = clickedSprite;
                controller.playerState = state;
            }
            else if (state != State.form161 && controller.playerState != state)
            {
                buttonImage.sprite = normalSprite;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (state == State.form296 && controller.playerState != state)
            {
                buttonImage.sprite = clickedSprite;
                controller.playerState = state;
            }
            else if (state != State.form296 && controller.playerState != state)
            {
                buttonImage.sprite = normalSprite;
            }
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (state == State.form71 && controller.playerState != state)
            {
                buttonImage.sprite = clickedSprite;
                controller.playerState = state;
            }
            else if (state != State.form71 && controller.playerState != state)
            {
                buttonImage.sprite = normalSprite;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (state == State.form161 && controller.playerState != state)
            {
                buttonImage.sprite = selectedSprite;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (state == State.form296 && controller.playerState != state)
            {
                buttonImage.sprite = selectedSprite;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (state == State.form71 && controller.playerState != state)
            {
                buttonImage.sprite = selectedSprite;
            }
        }
    }
}
