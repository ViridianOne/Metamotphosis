using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using static Form_switch_controller;
using UnityEngine.Playables;

public class DialogueSystem : MonoBehaviour
{
    [HideInInspector] public NpcPhrase[] sentences;

    public GameObject dialogue;

    public GameObject formSwitchController;

    public bool dialogueStarted;

    public Image dialogueWindow;

    public TMP_Text dialogueText;

    public float dialogueSpeed;

    private int sentenceIndex;
    private int charIndex;
    private bool waitForNext;
    private bool isStarted = false;

    public bool isFinal = false;
    public float yPos = -313f;
    [SerializeField] private GameObject victoryScreen;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private RectTransform animationPos;
    [SerializeField] private float guardAnimationXPos, mecroAnimationXPos;

    private void Start()
    {
        foreach( var item in sentences)
        {
            if(item.scene != null)
            {
                item.scene.played += CutsceneStarted;
                item.scene.stopped += CutsceneStopped;
            }
        }
    }

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
        animationPos.localPosition = new Vector3(sentences[sentenceIndex].character == MecroStates.none 
            ? mecroAnimationXPos : guardAnimationXPos, animationPos.localPosition.y, 0);
        animator.SetFloat("characterNumber", (float)sentences[sentenceIndex].character);
        animator.SetFloat("actionNumber", (float)sentences[sentenceIndex].action);
        animator.SetBool("isTalking", true);
        animator.SetTrigger("action");
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
        {
            if (sentences[sentenceIndex].scene != null)
            {
                yield return new WaitForSeconds(0.6f);
                sentences[sentenceIndex].scene.Play();
            }
            animator.SetBool("isTalking", false);
            waitForNext = true;
        }
            
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
            {
                if (isStarted == false)
                    GetDialogue(sentenceIndex);

                
            }               
            else
                EndDialogue();
        }
        if (waitForNext == false && Input.anyKey && charIndex > 4)
        {
            dialogueSpeed = 0;
        }

        
    }

    private void CutsceneStopped(PlayableDirector obj)
    {
        isStarted = false;
        ToggleWindow(true);
    }

    private void CutsceneStarted(PlayableDirector obj)
    {
        ToggleWindow(false);
        isStarted = true;
    }


    [Serializable]
    public struct NpcPhrase
    {
        public string text;
        public Sprite fraseBackground;
        public PlayableDirector scene;
        public CharacterAction action;
        public MecroStates character; //Mecro is a 'none' form
    }
}
