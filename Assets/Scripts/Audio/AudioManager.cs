using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using static AudioManager;


public enum ESoundType
{ 
    Null = -1,
    BGM,
    Ambiance,
    Walk,
    Run,
    ItemPick,
    ItemDrop,
    ItemPut,
    ItemBuy,
    ItemSell,
    Loading,
    FlashOn,
    FlashOff,
    TorchIdle,
    Count
}

public enum EField
{
    Null = -1,
    Main,
    Camp,
    Temple,
    Count
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] public SoundTheme[] themes;
    [SerializeField] private SoundTheme theme;

    [SerializeField]
    AudioSource mainSource;
    [SerializeField]
    AudioSource BGMSource;
    [SerializeField]
    AudioSource PlayerSource;

    [Header("AudioManager Settings")]
    [SerializeField] private float maxDistance = 15F;
    [SerializeField] private float minDistance = 0.1F;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        if (mainSource == null)
            mainSource = Camera.main.GetComponent<AudioSource>();

        //Init();
    }

    private void Start()
    {
        PlayLoop(ESoundType.BGM);
    }
    

    public void SetSoundPack(SoundTheme soundTheme)
    {
        Debug.Log("SoundSet");
        theme = soundTheme;
    }

    public void SetBGMSource(AudioSource source)
    {
        BGMSource = source;
    }

    public void SetPlayerSource(AudioSource source)
    {
        PlayerSource = source;
    }

    public void PlaySound(ESoundType soundType)
    {
        Debug.Log("SoundSet");
        theme.PlaySound(mainSource, soundType);
    }

    public void PlayLoop(ESoundType soundType)
    {
        Debug.Log("LoopSet");
        theme.PlayLoop(mainSource, soundType);
    }
    public void Stop(ESoundType soundType)
    {
        theme.Stop(mainSource, soundType);
    }
}
