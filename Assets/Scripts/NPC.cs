using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private DialogueSystem.NpcFrase[] npcSentences;
    [SerializeField] private GameObject pressButtonMessage;
    private DialogueSystem.NpcFrase[] empty;
    private bool canTalk;
    private bool wasDialogue;

    [SerializeField] private MecroStates mecroToUnlock;

    void Start()
    {
        pressButtonMessage.SetActive(false);
        wasDialogue = false;
    }

    private void Update()
    {
        if (canTalk)
        {
            if (wasDialogue && Input.GetKeyDown(KeyCode.E)) 
            {
                pressButtonMessage.SetActive(false);
                dialogueSystem.StartDialogue();
                canTalk = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            dialogueSystem.sentences = npcSentences;
            canTalk = wasDialogue;

            if (wasDialogue)
            {
                pressButtonMessage.SetActive(true);
            }
            else
            {
                dialogueSystem.StartDialogue();
                wasDialogue = true;
                if (mecroToUnlock != MecroStates.none)
                {
                    MecroSelectManager.instance.isMecroUnlocked[(int)mecroToUnlock] = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            canTalk = false;
            pressButtonMessage.SetActive(false);

            if (dialogueSystem.dialogueStarted)
            {
                dialogueSystem.EndDialogue();
            }
            dialogueSystem.sentences = empty;
        }
    }
}
