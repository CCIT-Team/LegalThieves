using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;


[CreateAssetMenu(fileName = "SoundTheme", menuName = "Scriptable Object/Sound", order = int.MaxValue)]
public class SoundTheme : ScriptableObject
{
    public AudioClip[] BGMS;
    public AudioClip[] ambiancsSounds;
    [Header("Player")]
    public AudioClip[] walk;
    public AudioClip[] run;
    [Header("Items")]
    public AudioClip[] pickUp;
    public AudioClip[] drop;
    public AudioClip[] putItem;
    public AudioClip[] buy;
    public AudioClip[] sell;
    [Header("FlashLight")]
    public AudioClip[] flashOn;
    public AudioClip[] flashOff;
    public AudioClip[] torch;

    public AudioClip[] loading;


    private AudioClip SelectClip(ESoundType soundType)
    {
        switch (soundType)
        {
            case ESoundType.BGM:
                return BGMS[Random.Range(0, BGMS.Length)];
            case ESoundType.Ambiance:
                return ambiancsSounds[Random.Range(0, ambiancsSounds.Length)];
            case ESoundType.Walk:
                return walk[Random.Range(0, walk.Length)];
            case ESoundType.Run:
                return run[Random.Range(0, run.Length)];
            case ESoundType.ItemPick:
                return pickUp[Random.Range(0, pickUp.Length)];
            case ESoundType.ItemDrop:
                return drop[Random.Range(0, drop.Length)];
            case ESoundType.ItemPut:
                return putItem[Random.Range(0, putItem.Length)];
                return null;
            case ESoundType.ItemBuy:
                return buy[Random.Range(0, buy.Length)];
            case ESoundType.ItemSell:
                return sell[Random.Range(0, sell.Length)];
            case ESoundType.Loading:
                return loading[Random.Range(0, loading.Length)];
            case ESoundType.FlashOn:
                return flashOn[Random.Range(0, flashOn.Length)];
            case ESoundType.FlashOff:
                return flashOff[Random.Range(0, flashOff.Length)];
            case ESoundType.TorchIdle:
                return torch[Random.Range(0,torch.Length)];
            default:
                return null; ;
        }
    }

    public void PlaySound(AudioSource audioSource ,ESoundType soundType)
    {
        
        audioSource.PlayOneShot(SelectClip(soundType));
    }

    public void PlayLoop(AudioSource audioSource, ESoundType soundType)
    {
        audioSource.loop = true;
        audioSource.clip = SelectClip(soundType);
        audioSource.Play();
    }
    public void Stop(AudioSource audioSource, ESoundType soundType)
    {
        if (audioSource.loop && audioSource.clip == SelectClip(soundType))
        {
            audioSource.Stop();
        }
    }
}
