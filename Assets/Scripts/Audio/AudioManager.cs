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

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Bgm {Cave, Jungle }

    public enum Sfx 
    { 
        ATTACK_01, ATTACK_02, BandageSound1, BodyDrop5, BREATHE, Club1, Club2, Coins10, DEATH,
        DEATH3, Dirtfootsteps15, Dirtfootsteps16, GravelFootsteps7, GravelFootsteps14,
        HeavyRunningDirtfootsteps1, HeavyRunningDirtfootsteps2, HeavyRunningGravelFootsteps1,
        HeavyRunningGravelFootsteps5, JUMP_01, JUMP_03, TorchLoop5, TorchSwing4, TorchSwing6,
        TurningFlashlightOn, TurningFlashlightOff3
    }


    private void Awake()
    {
        instance = this;
        Init();
    }
    void Init()
    {
        //정글 배경음 플레이어 초기화
        GameObject bgmJungleObject = new GameObject("BgmJunglePlayer");
        bgmJungleObject.transform.parent = transform;
        bgmJunglePlayer = bgmJungleObject.AddComponent<AudioSource>();
        bgmJunglePlayer.playOnAwake = false;
        bgmJunglePlayer.loop = true;
        bgmJunglePlayer.volume = bgmJungleVolume;
        bgmJunglePlayer.clip = bgmJungleClip;

        //동굴 배경음 플레이어 초기화
        GameObject bgmCaveObject = new GameObject("BgmCavePlayer");
        bgmCaveObject.transform.parent = transform;
        bgmCavePlayer = bgmCaveObject.AddComponent<AudioSource>();
        bgmCavePlayer.playOnAwake = false;
        bgmCavePlayer.loop = true;
        bgmCavePlayer.volume = bgmCaveVolume;
        bgmCavePlayer.clip = bgmCaveClip;

        //효과음 플레이어 초기화
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

    public void PlaySfx(Sfx sfx)
    {
        for(int index = 0;index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            if(sfx == Sfx.JUMP_01 || sfx == Sfx.JUMP_03)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
