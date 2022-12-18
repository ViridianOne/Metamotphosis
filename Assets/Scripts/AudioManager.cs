using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public void Awake()
    {
        foreach(Sound sound in sounds)
        {
            sound.source=gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }
    public void Play(string audioName)
    {
        Sound sound = Array.Find(sounds, s => s.clipName == audioName);
        sound.source.Play();
    }

}
