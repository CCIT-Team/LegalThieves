using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;


[CreateAssetMenu(fileName = "SoundTheme", menuName = "Scriptable Object/Sound", order = int.MaxValue)]
public class SoundTheme : ScriptableObject
{
    public AudioClip[] audioClips;

    public void PlaySound(AudioSource audioSource ,ESoundType soundType)
    {
        audioSource.PlayOneShot(audioClips[(int)soundType]);
    }

    public void PlayLoop(AudioSource audioSource, ESoundType soundType)
    {
        audioSource.loop = true;
        audioSource.clip = audioClips[(int)soundType];
        audioSource.Play();
    }
    public void Stop(AudioSource audioSource, ESoundType soundType)
    {
        if (audioSource.loop && audioSource.clip == audioClips[(int)soundType])
        {
            audioSource.Stop();
        }
    }
}
