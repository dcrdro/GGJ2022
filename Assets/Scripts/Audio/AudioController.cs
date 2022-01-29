using UnityEngine;

public abstract class AudioController : MonoBehaviour
{
    public AudioSource AudioSource;
    
    protected void PlayAudio(AudioData data)
    {
        var clip = data.GetRandom();
        AudioSource.clip = clip;
        AudioSource.Play();
    }
}