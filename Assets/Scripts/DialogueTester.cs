using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueSystem;

public class DialogueTester : MonoBehaviour
{
    public DialogueSystem dialogeSystem;
    public NpcPhrase[] npcSentences;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogeSystem.sentences = npcSentences;
            dialogeSystem.StartDialogue();
        }  
    }
}
