using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] sounds;
    public AudioMixerGroup group;
    public void Awake()
    {
        instance = this;

        foreach(Sound sound in sounds)
        {
            sound.source=gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.outputAudioMixerGroup = group;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = sound.playOnAwake;
        }
    }
    public void Play(int index)
    {
        sounds[index].source.Play();
    }
    public void Stop(int index)
    {
        sounds[index].source.Stop();
    }
}
