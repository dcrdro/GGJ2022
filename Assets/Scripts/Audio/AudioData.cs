using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AudioData
{
    public AudioClip[] Clips;

    public AudioClip GetRandom() => Clips[Random.Range(0, Clips.Length)];
}