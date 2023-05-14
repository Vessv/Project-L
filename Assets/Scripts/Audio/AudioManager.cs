using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    public static AudioManager Instance { get; private set; }
    public AudioMixerGroup audioMixer;


    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in Sounds)
        {
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.outputAudioMixerGroup = audioMixer;
            s.Source.clip = s.Clip;

            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;

            s.Source.loop = s.loop;
        }

        StartMusic();
    }

    void StartMusic()
    {
        Play("music2", true);
    }

    public void Click()
    {
        Play("click", true);
    }

    public void Play(string name, bool instant)
    {
        Sound sound = Array.Find(Sounds, s => s.Name == name);
        if(sound == null) return;
        if(instant)
        {
            sound.Source.volume = sound.Volume;
            sound.Source.Play();
            return;
        }
        StartCoroutine(PlaySound(sound));
    }

    public void Stop(string name, bool instant)
    {
        Sound sound = Array.Find(Sounds, s => s.Name == name);
        if (sound == null) return;
        if (instant)
        {
            sound.Source.Stop();
            return;
        }
        StartCoroutine(StopSound(sound));
    }

    IEnumerator PlaySound(Sound sound)
    {
        sound.Source.volume = 0f;
        sound.Source.Play();
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            sound.Source.volume += 0.08f;
        }
        yield break;
    }

    IEnumerator StopSound(Sound sound)
    {
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            sound.Source.volume -= 0.08f;
        }

        sound.Source.Stop();
        yield break;

    }

}
