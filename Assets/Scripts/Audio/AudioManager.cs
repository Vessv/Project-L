using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager Instance { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;

            s.Source.loop = s.loop;
        }
    }


    public void Play(string name)
    {
        Sound sound = Array.Find(Sounds, s => s.Name == name);
        if(sound == null) return;
        sound.Source.Play();
    }

}
