using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private DialogueSystem.NpcFrase[] npcSentences;
    [SerializeField] private GameObject pressButtonMessage;
    private bool wasDialogue;
    private bool canTalk;

    void Start()
    {
        dialogueSystem.sentences = npcSentences;
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
            canTalk = wasDialogue;

            if (wasDialogue)
            {
                pressButtonMessage.SetActive(true);
            }
            else
            {
                dialogueSystem.StartDialogue();
                wasDialogue = true;
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
        }
    }
}
