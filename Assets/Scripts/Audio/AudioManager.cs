using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("#BGMJungle")]
    public AudioClip bgmJungleClip;
    public float bgmJungleVolume;
    AudioSource bgmJunglePlayer;

    [Header("#BGMCave")]
    public AudioClip bgmCaveClip;
    public float bgmCaveVolume;
    AudioSource bgmCavePlayer;

    [Header("#SFXTorchLoop")]
    public AudioClip sfxTorchLoopClip;
    public float sfxTorchLoopVolume;
    AudioSource sfxTorchLoopPlayer;

    [Header("#SFXBreath")]
    public AudioClip sfxBreathClip;
    public float sfxBreathVolume;
    AudioSource sfxBreathPlayer;

    [Header("#SFXGF")]  //GF->Gravel Footsteps �� �ظ� �������� ������ �߼Ҹ�
    public AudioClip sfxGFClip;
    public float sfxGFVolume;
    AudioSource sfxGFPlayer;

    [Header("#SFXDF")]  //GF->Gravel Footsteps �� �ظ� �����ܺ� ������ �߼Ҹ�
    public AudioClip sfxDFClip;
    public float sfxDFVolume;
    AudioSource sfxDFPlayer;

    [Header("#SFXHRGF")]  //HRGF->Heavy Running Gravel Footsteps �� �ظ� �������� �ۋ� �߼Ҹ�
    public AudioClip sfxHRGFClip;
    public float sfxHRGFVolume;
    AudioSource sfxHRGFPlayer;

    [Header("#SFXHRDF")]  //HRDF->Heavy Running Dirt Footsteps �� �ظ� �����ܺ� �ۋ� �߼Ҹ�
    public AudioClip sfxHRDFClip;
    public float sfxHRDFVolume;
    AudioSource sfxHRDFPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Bgm {Cave, Jungle }

    public enum Sfx 
    { 
        ATTACK_01, ATTACK_02, BandageSound1, BodyDrop5, Club1, Club2, Coins10, DEATH,
        DEATH3, JUMP_01, JUMP_03, TorchSwing4, TorchSwing6, TurningFlashlightOn, 
        TurningFlashlightOff3
    }


    private void Awake()
    {
        instance = this;
        Init();
    }
    void Init()
    {
        //���� ����� �÷��̾� �ʱ�ȭ
        GameObject bgmJungleObject = new GameObject("BgmJunglePlayer");
        bgmJungleObject.transform.parent = transform;
        bgmJunglePlayer = bgmJungleObject.AddComponent<AudioSource>();
        bgmJunglePlayer.playOnAwake = false;
        bgmJunglePlayer.loop = true;
        bgmJunglePlayer.volume = bgmJungleVolume;
        bgmJunglePlayer.clip = bgmJungleClip;

        //���� ����� �÷��̾� �ʱ�ȭ
        GameObject bgmCaveObject = new GameObject("BgmCavePlayer");
        bgmCaveObject.transform.parent = transform;
        bgmCavePlayer = bgmCaveObject.AddComponent<AudioSource>();
        bgmCavePlayer.playOnAwake = false;
        bgmCavePlayer.loop = true;
        bgmCavePlayer.volume = bgmCaveVolume;
        bgmCavePlayer.clip = bgmCaveClip;

        //ȶ��Ÿ�� �Ҹ� ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxTorchLoopObject = new GameObject("sfxTorchLoopPlayer");
        sfxTorchLoopObject.transform.parent = transform;
        sfxTorchLoopPlayer = sfxTorchLoopObject.AddComponent<AudioSource>();
        sfxTorchLoopPlayer.playOnAwake = false;
        sfxTorchLoopPlayer.loop = true;
        sfxTorchLoopPlayer.volume = sfxTorchLoopVolume;
        sfxTorchLoopPlayer.clip = sfxTorchLoopClip;

        //�� �� ���Ҹ� ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxBreathObject = new GameObject("sfxBreathPlayer");
        sfxBreathObject.transform.parent = transform;
        sfxBreathPlayer = sfxBreathObject.AddComponent<AudioSource>();
        sfxBreathPlayer.playOnAwake = false;
        sfxBreathPlayer.loop = true;
        sfxBreathPlayer.volume = sfxBreathVolume;
        sfxBreathPlayer.clip = sfxBreathClip;

        //���� ���� ������ �߼Ҹ� ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxGFObject = new GameObject("sfxHRGFPlayer");
        sfxGFObject.transform.parent = transform;
        sfxGFPlayer = sfxGFObject.AddComponent<AudioSource>();
        sfxGFPlayer.playOnAwake = false;
        sfxGFPlayer.loop = true;
        sfxGFPlayer.volume = sfxGFVolume;
        sfxGFPlayer.clip = sfxGFClip;

        //���� �ܺ� ������ �߼Ҹ� ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxDFObject = new GameObject("sfxHRGFPlayer");
        sfxDFObject.transform.parent = transform;
        sfxDFPlayer = sfxDFObject.AddComponent<AudioSource>();
        sfxDFPlayer.playOnAwake = false;
        sfxDFPlayer.loop = true;
        sfxDFPlayer.volume = sfxDFVolume;
        sfxDFPlayer.clip = sfxDFClip;

        //���� ���� �� �� �߼Ҹ� ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxHRGFObject = new GameObject("sfxHRGFPlayer");
        sfxHRGFObject.transform.parent = transform;
        sfxHRGFPlayer = sfxHRGFObject.AddComponent<AudioSource>();
        sfxHRGFPlayer.playOnAwake = false;
        sfxHRGFPlayer.loop = true;
        sfxHRGFPlayer.volume = sfxHRGFVolume;
        sfxHRGFPlayer.clip = sfxHRGFClip;

        //���� �ܺ� �� �� �߼Ҹ� ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxHRDFObject = new GameObject("sfxHRDFPlayer");
        sfxHRDFObject.transform.parent = transform;
        sfxHRDFPlayer = sfxHRDFObject.AddComponent<AudioSource>();
        sfxHRDFPlayer.playOnAwake = false;
        sfxHRDFPlayer.loop = true;
        sfxHRDFPlayer.volume = sfxHRDFVolume;
        sfxHRDFPlayer.clip = sfxHRDFClip;

        //ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("sfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels]; 

        for(int index=0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake=false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayJungleBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmJunglePlayer.Play();
        }
        else { bgmJunglePlayer.Stop();
        }
    }
    public void PlayCaveBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmCavePlayer.Play();
        }
        else
        {
            bgmCavePlayer.Stop();
        }
    }
    public void PlayTorchLoopSfx(bool isPlay)
    {
        if (isPlay)
        {
            sfxTorchLoopPlayer.Play();
        }
        else
        {
            sfxTorchLoopPlayer.Stop();
        }
    }
    public void PlayBreathSfx(bool isPlay)
    {
        if (isPlay)
        {
            sfxBreathPlayer.Play();
        }
        else
        {
            sfxBreathPlayer.Stop();
        }
    }
    public void PlayGFSfx(bool isPlay)
    {
        if (isPlay)
        {
            sfxGFPlayer.Play();
        }
        else
        {
            sfxGFPlayer.Stop();
        }
    }
    public void PlayDFSfx(bool isPlay)
    {
        if (isPlay)
        {
            sfxDFPlayer.Play();
        }
        else
        {
            sfxDFPlayer.Stop();
        }
    }
    public void PlayHRGFSfx(bool isPlay)
    {
        if (isPlay)
        {
            sfxHRGFPlayer.Play();
        }
        else
        {
            sfxHRGFPlayer.Stop();
        }
    }
    public void PlayHRDFSfx(bool isPlay)
    {
        if (isPlay)
        {
            sfxHRDFPlayer.Play();
        }
        else
        {
            sfxHRDFPlayer.Stop();
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int index = 0;index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
