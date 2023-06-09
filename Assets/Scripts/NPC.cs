using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private DialogueSystem.NpcPhrase[] npcSentences;
    [SerializeField] private GameObject pressButtonMessage;
    private DialogueSystem.NpcPhrase[] empty;
    private bool canTalk;
    private bool wasDialogue;

    [SerializeField] private MecroStates mecroToUnlock;
    [SerializeField] private bool isFinal = false;

    [SerializeField] MecroStates guardForm;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        anim.SetFloat("guard", (float)guardForm);
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
            dialogueSystem.isFinal = isFinal;
            if (transform.position.y <= -10f && transform.position.y > -18f || transform.position.y >= -3f && transform.position.y < 3f || transform.position.y >= 11f)
                dialogueSystem.yPos = -313f;
            else
                dialogueSystem.yPos = 313f;
            if(transform.position.x < other.transform.position.x && transform.localRotation.y != 0
                || transform.position.x >= other.transform.position.x && transform.localRotation.y == 0)
            {
                if (transform.localRotation.y == 0)
                {
                    transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    pressButtonMessage.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    pressButtonMessage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                }
            }
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
                    MecroSelectManager.instance.SetMecroUnlock(mecroToUnlock, true);
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
