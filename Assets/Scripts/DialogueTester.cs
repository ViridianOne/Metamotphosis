using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public DialogueSystem dialogeSystem;
    public string[] npcSentences;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogeSystem.sentences = npcSentences;
            dialogeSystem.StartDialogue();
        }  
    }
}
