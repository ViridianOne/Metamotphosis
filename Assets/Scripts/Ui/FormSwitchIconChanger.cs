using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FormSwitchIconChanger : MonoBehaviour
{
    public static FormSwitchIconChanger instance;

    public Image BackgroundRing;
    private MecroStates CurrentState = MecroStates.form161;
    private string CurrentColorCode = "#13805D";
    [SerializeField] public FormSelectionIcon[] ButtonImages;
    [SerializeField] public List<MecroStates> PossibleStates = new();


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ChangeForm(CurrentColorCode, CurrentState);

        foreach (var buttonImage in ButtonImages)
        {
            if (PossibleStates.Contains(buttonImage.Name))
            {
                continue;
            }
            else
            {
                buttonImage.Icon.gameObject.SetActive(false);
            }

        }
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            BackgroundRing.gameObject.SetActive(true);
            var angle = GetAngle();
            SetFormProperties(angle);
            Time.timeScale = 0.25f;
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Time.timeScale = 1;
                ChangeForm(CurrentColorCode, CurrentState);
            }
            BackgroundRing.gameObject.SetActive(false);
        }

    }

    private void SetFormProperties(float angle)
    {
        if (41.5f <= angle && angle < 86.5f && PossibleStates.Contains(MecroStates.form26))
        {
            CurrentState = MecroStates.form26;
            CurrentColorCode = "#804213";
            HighlightFormIcon(CurrentState);
        }
        else if (-3.5f <= angle && angle < 41.5f && PossibleStates.Contains(MecroStates.form71))
        {
            CurrentState = MecroStates.form71;
            CurrentColorCode = "#6C8013";
            HighlightFormIcon(CurrentState);
        }
        else if (-48.5f <= angle && angle < -3.5f && PossibleStates.Contains(MecroStates.form116))
        {
            CurrentState = MecroStates.form116;
            CurrentColorCode = "#1A8013";
            HighlightFormIcon(CurrentState);
        }
        else if (-93.5f <= angle && angle < -48.5f && PossibleStates.Contains(MecroStates.form161))
        {
            CurrentState = MecroStates.form161;
            CurrentColorCode = "#13805D";
            HighlightFormIcon(CurrentState);
        }
        else if (-138.5f <= angle && angle < -93.5f && PossibleStates.Contains(MecroStates.form206))
        {
            CurrentState = MecroStates.form206;
            CurrentColorCode = "#135180";
            HighlightFormIcon(CurrentState);
        }
        else if (-183.5f <= angle && angle < -138.5f && PossibleStates.Contains(MecroStates.form251))
        {
            CurrentState = MecroStates.form251;
            CurrentColorCode = "#271380";
            HighlightFormIcon(CurrentState);
        }
        else if (131.5f <= angle && angle < 176.5f && PossibleStates.Contains(MecroStates.form296))
        {
            CurrentState = MecroStates.form296;
            CurrentColorCode = "#791380";
            HighlightFormIcon(CurrentState);
        }
        else if (83.5f <= angle && angle < 128.5f && PossibleStates.Contains(MecroStates.form341))
        {
            CurrentState = MecroStates.form341;
            CurrentColorCode = "#801336";
            HighlightFormIcon(CurrentState);
        }
    }

    private float GetAngle()
    {
        var mousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2f, Input.mousePosition.y - Screen.height / 2f);//я не нашел координат персонажа в свободном доступе, поэтому разместил меню выбора в центре экрана
        mousePosition.Normalize();
        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;
        return angle;
    }

    private void HighlightFormIcon(MecroStates form)
    {
        foreach (var buttonImage in ButtonImages)
        {
            if (buttonImage.Name == form)
            {
                buttonImage.Icon.sprite = buttonImage.HighlightedImage;
            }
            else
            {
                buttonImage.Icon.sprite = buttonImage.NormalImage;
            }

        }
    }

    private void ChangeForm(string color, MecroStates form)
    {
        Color newColor;
        ColorUtility.TryParseHtmlString(color, out newColor);
        BackgroundRing.color = newColor;
        foreach (var buttonImage in ButtonImages) 
        {
            if (buttonImage.Name == form && PossibleStates.Contains(form))
            {
                buttonImage.Icon.sprite = buttonImage.PressedImage;
                MecroSelectManager.instance.SelectMecro(form);
            }
        }
    }

    public void SetPossibleState(MecroStates mecro, bool isUnlocked)
    {
        if (isUnlocked)
        {
            if (PossibleStates.Contains(mecro))
                return;
            
            PossibleStates.Add(mecro);
            foreach (var buttonImage in ButtonImages)
            {
                if (mecro == buttonImage.Name)
                {
                    buttonImage.Icon.gameObject.SetActive(true);
                    return;
                }
            }            
        }
        else
        {
            PossibleStates.Remove(mecro);
        }
    }
}