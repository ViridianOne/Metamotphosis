using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    private bool isStarted = false;
    [SerializeField] PlayableDirector cutscene;
    [SerializeField] RoomActiveZone room;

    // Update is called once per frame
    void Update()
    {
        if( room.isInRoom && !isStarted)
        {
            isStarted = true;
            cutscene.Play();
        }
    }
}
