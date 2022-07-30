using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound
{

    public string Name;

    public AudioClip Clip;

    [Range(0,1f)]
    public float Volume;
    [Range(.1f, 3f)]
    public float Pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource Source;
}
