using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTester : MonoBehaviour
{
    public DialogueSystem dialogeSystem;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogeSystem.StartDialogue();
        }  
    }
}
