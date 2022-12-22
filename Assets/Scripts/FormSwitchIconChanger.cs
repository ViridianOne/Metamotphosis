using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormSwitchIconChanger : MonoBehaviour
{
    public Form_switch_controller controller;
    public MecroStates state;

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
            if (state == MecroStates.form161 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = clickedSprite;
                controller.playerState = state;
            }
            else if (state != MecroStates.form161 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = normalSprite;
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (state == MecroStates.form296 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = clickedSprite;
                controller.playerState = state;
            }
            else if (state != MecroStates.form296 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = normalSprite;
            }
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (state == MecroStates.form71 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = clickedSprite;
                controller.playerState = state;
            }
            else if (state != MecroStates.form71 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = normalSprite;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (state == MecroStates.form161 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = selectedSprite;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (state == MecroStates.form296 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = selectedSprite;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (state == MecroStates.form71 && controller.playerState != state 
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
            {
                buttonImage.sprite = selectedSprite;
            }
        }
    }
}
