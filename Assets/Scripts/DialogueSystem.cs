using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class DialogueSystem : MonoBehaviour
{
    public string[] sentences;
    public bool dialogueStarted;

    public GameObject dialogueWindow;

    public TMP_Text dialogueText;

    public float dialogueSpeed;

    private int sentenceIndex;
    private int charIndex;
    private bool waitForNext;

    //private float temp;//мне очень не нравится что оно здесь есть, но мне нужно что - то в этой области памяти для запоминания изначальной скорости

    private void Awake()
    {
        ToggleWindow(false);
    }

    private void ToggleWindow(bool state)
    {
        dialogueWindow.SetActive(state);
    }

    public void StartDialogue()
    {
        if (dialogueStarted)
            return;

        dialogueStarted = true;
        ToggleWindow(true);
        GetDialogue(0);
    }

    private void GetDialogue(int index)
    {
        sentenceIndex = index;
        charIndex = 0;
        dialogueText.text = string.Empty;
        StartCoroutine(DialogueWriting());
    }

    public void EndDialogue()
    {
        ToggleWindow(false);
    }

    IEnumerator DialogueWriting()
    {
        string currentSentence = sentences[sentenceIndex];
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
}
