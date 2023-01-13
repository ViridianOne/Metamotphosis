using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using static Form_switch_controller;

public class DialogueSystem : MonoBehaviour
{
    [HideInInspector] public NpcFrase[] sentences;

    public GameObject dialogue;

    public GameObject formSwitchController;

    public bool dialogueStarted;

    public Image dialogueWindow;

    public TMP_Text dialogueText;

    public float dialogueSpeed;

    private int sentenceIndex;
    private int charIndex;
    private bool waitForNext;

    public bool isFinal = false;
    public float yPos = -313f;
    [SerializeField] private GameObject victoryScreen;

    //private void Awake()
    //{
    //    ToggleWindow(false);
    //}

    private void ToggleWindow(bool state)
    {
        dialogue.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, yPos);
        dialogue.SetActive(state);
    }

    public void StartDialogue()
    {
        if (dialogueStarted)
            return;

        ToggleWindow(true);
        dialogueStarted = true;
        formSwitchController.SetActive(false);
        dialogueWindow.sprite = sentences[0].fraseBackground;
        Player.instance.ToggleActive(false);
        GetDialogue(0);
    }

    private void GetDialogue(int index)
    {
        formSwitchController.SetActive(false);
        sentenceIndex = index;
        charIndex = 0;
        dialogueWindow.sprite = sentences[index].fraseBackground;
        dialogueText.text = string.Empty;
        AudioManager.instance.Play(15);
        StartCoroutine(DialogueWriting());
    }

    public void EndDialogue()
    {
        Player.instance.ToggleActive(true);
        ToggleWindow(false);
        formSwitchController.SetActive(true);
        dialogueStarted = false;
        if (isFinal)
        {
            victoryScreen.SetActive(true); 
        }
    }

    IEnumerator DialogueWriting()
    {
        string currentSentence = sentences[sentenceIndex].text;
        dialogueText.text += currentSentence[charIndex];
        charIndex++;

        if (charIndex < currentSentence.Length)
        {
            yield return new WaitForSeconds(dialogueSpeed);
            StartCoroutine(DialogueWriting());
        }
        else
            waitForNext = true;
    }

    void Update()
    {
        if (!dialogueStarted)
            return;

        if (waitForNext == true && Input.GetKeyDown(KeyCode.E))
        {
            waitForNext = false;
            dialogueSpeed = 0.2f;
            sentenceIndex++;

            if (sentenceIndex < sentences.Length)
                GetDialogue(sentenceIndex);
            else
                EndDialogue();
        }
        if (waitForNext == false && Input.anyKey && charIndex > 4)
        {
            dialogueSpeed = 0;
        }
    }

    [Serializable]
    public struct NpcFrase
    {
        public string text;
        public Sprite fraseBackground;
    }
}
