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
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            holdLeftButton();
        }
        //else if (Input.GetKeyUp(KeyCode.DownArrow))
        //{
        //    if (state == MecroStates.form206 && controller.playerState != state
        //        && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        //    {
        //        buttonImage.sprite = clickedSprite;
        //        controller.playerState = state;
        //    }
        //    else if (state != MecroStates.form206 && controller.playerState != state
        //        && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        //    {
        //        buttonImage.sprite = normalSprite;
        //    }
        //}
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            holdTopButton();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            holdRightButton();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            holdBottomButton();
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            releaseLeftButton();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            releaseTopButton();
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            releaseRightButton();
        }
        else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            releaseBottomButton();
        }
        else if (Input.GetKeyUp(KeyCode.I))
        {
            releaseIButton();
        }
    }
    private void releaseLeftButton()
    {
        if (state == MecroStates.form296
               && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = clickedSprite;
            controller.playerState = state;
        }
        else if (state != MecroStates.form296 
            && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = normalSprite;
        }
    }
    private void releaseTopButton()
    {
        if (state == MecroStates.form161 
            && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = clickedSprite;
            controller.playerState = state;
        }
        else if (state != MecroStates.form161
            && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = normalSprite;
        }
    }

    private void releaseRightButton()
    {
        if (state == MecroStates.form71
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = clickedSprite;
            controller.playerState = state;
        }
        else if (state != MecroStates.form71
            && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = normalSprite;
        }
    }

    private void releaseBottomButton()
    {
        if (state == MecroStates.form206
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = clickedSprite;
            controller.playerState = state;
        }
        else if (state != MecroStates.form206
            && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = normalSprite;
        }
    }

    private void releaseIButton()
    {
        if(state == MecroStates.form341
            && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            controller.playerState = state;
        }
    }

    private void holdLeftButton()
    {
        if (state == MecroStates.form296
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = selectedSprite;
        }
    }
    private void holdTopButton()
    {
        if (state == MecroStates.form161
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = selectedSprite;
        }
    }

    private void holdRightButton()
    {
        if (state == MecroStates.form71
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = selectedSprite;
        }
    }

    private void holdBottomButton()
    {
        if (state == MecroStates.form206
                && MecroSelectManager.instance.isMecroUnlocked[(int)state])
        {
            buttonImage.sprite = selectedSprite;
        }
    }
}
