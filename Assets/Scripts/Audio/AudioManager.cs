using Fusion;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("AudioManager Settings")]
    [SerializeField] private float maxDistance = 15F;
    [SerializeField] private float minDistance = 0.1F;
    [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

    [Header("#BGMJungle")]
    public AudioClip bgmJungleClip;
    public float bgmJungleVolume;

    [Header("#BGMCave")]
    public AudioClip bgmCaveClip;
    public float bgmCaveVolume;

    private AudioSource localBgmPlayer;  // 로컬 BGM용 AudioSource

    [Header("#SFX")]
    public AudioClip sfxTorchLoopClip;
    public float sfxTorchLoopVolume;
    public AudioClip sfxBreathClip;
    public float sfxBreathVolume;
    public AudioClip sfxGFClip;
    public float sfxGFVolume;
    public AudioClip sfxDFClip;
    public float sfxDFVolume;
    public AudioClip sfxHRGFClip;
    public float sfxHRGFVolume;
    public AudioClip sfxHRDFClip;
    public float sfxHRDFVolume;
    public AudioClip sfxJUMPClip;
    public float sfxJUMPVolume;

    private AudioSource sfxTorchLoopPlayer;
    private AudioSource sfxBreathPlayer;
    private AudioSource sfxGFPlayer;
    private AudioSource sfxDFPlayer;
    private AudioSource sfxHRGFPlayer;
    private AudioSource sfxHRDFPlayer;
    private AudioSource sfxJumpPlayer;

    private Dictionary<string, AudioSource> sfxPlayers = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        // 로컬 플레이어의 BGM 플레이어 초기화
        localBgmPlayer = CreateAudioSource("LocalBgmPlayer", null, 0, true, isBgm: true);

        // SFX 플레이어 초기화
        sfxTorchLoopPlayer = CreateAudioSource("TorchLoopPlayer", sfxTorchLoopClip, sfxTorchLoopVolume, loop: true, isBgm: false);
        sfxBreathPlayer = CreateAudioSource("BreathPlayer", sfxBreathClip, sfxBreathVolume, loop: true, isBgm: false);
        sfxGFPlayer = CreateAudioSource("GFPlayer", sfxGFClip, sfxGFVolume, loop: false, isBgm: false);
        sfxDFPlayer = CreateAudioSource("DFPlayer", sfxDFClip, sfxDFVolume, loop: false, isBgm: false);
        sfxHRGFPlayer = CreateAudioSource("HRGFPlayer", sfxHRGFClip, sfxHRGFVolume, loop: false, isBgm: false);
        sfxHRDFPlayer = CreateAudioSource("HRDFPlayer", sfxHRDFClip, sfxHRDFVolume, loop: false, isBgm: false);
        sfxJumpPlayer = CreateAudioSource("JumpPlayer", sfxJUMPClip, sfxJUMPVolume, loop: false, isBgm: false);

        sfxPlayers.Add("TorchLoop", sfxTorchLoopPlayer);
        sfxPlayers.Add("Breath", sfxBreathPlayer);
        sfxPlayers.Add("GF", sfxGFPlayer);
        sfxPlayers.Add("DF", sfxDFPlayer);
        sfxPlayers.Add("HRGF", sfxHRGFPlayer);
        sfxPlayers.Add("HRDF", sfxHRDFPlayer);
        sfxPlayers.Add("Jump", sfxJumpPlayer);
    }

    private AudioSource CreateAudioSource(string name, AudioClip clip, float volume, bool loop, bool isBgm)
    {
        GameObject audioObject = new GameObject(name);
        audioObject.transform.parent = transform;
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = loop;

        ConfigureAudioSource(audioSource);
        return audioSource;
    }

    private void ConfigureAudioSource(AudioSource audioSource)
    {
        // 거리별 소리 감소 효과 설정
        audioSource.spatialBlend = 1.0f; // 3D 사운드로 설정
        audioSource.rolloffMode = rolloffMode;
        audioSource.maxDistance = maxDistance;
        audioSource.minDistance = minDistance;
        audioSource.dopplerLevel = 0f; // Doppler 효과 비활성화
    }

    public void PlayGFSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxGFPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxGFPlayer.Play();
        }
        else
        {
            sfxGFPlayer.Stop();
        }
    }

    public void PlayDFSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxDFPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxDFPlayer.Play();
        }
        else
        {
            sfxDFPlayer.Stop();
        }
    }

    public void PlayHRGFSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxHRGFPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxHRGFPlayer.Play();
        }
        else
        {
            sfxHRGFPlayer.Stop();
        }
    }

    public void PlayHRDFSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxHRDFPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxHRDFPlayer.Play();
        }
        else
        {
            sfxHRDFPlayer.Stop();
        }
    }

    public void PlayTorchLoopSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxTorchLoopPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxTorchLoopPlayer.Play();
        }
        else
        {
            sfxTorchLoopPlayer.Stop();
        }
    }

    public void PlayBreathSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxBreathPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxBreathPlayer.Play();
        }
        else
        {
            sfxBreathPlayer.Stop();
        }
    }

    public void PlayJumpSfx(Vector3 playerPosition, bool isPlay)
    {
        sfxJumpPlayer.transform.position = playerPosition;
        if (isPlay)
        {
            sfxJumpPlayer.Play();
        }
        else
        {
            sfxJumpPlayer.Stop();
        }
    }

    public void UpdateBgmBasedOnLocation(bool isInCave)
    {
        if (isInCave)
        {
            PlayLocalBgm(bgmCaveClip, bgmCaveVolume);
        }
        else
        {
            PlayLocalBgm(bgmJungleClip, bgmJungleVolume);
        }
    }

    private void PlayLocalBgm(AudioClip clip, float volume)
    {
        if (localBgmPlayer.clip != clip)
        {
            localBgmPlayer.Stop();
            localBgmPlayer.clip = clip;
            localBgmPlayer.volume = volume;
            localBgmPlayer.Play();
        }
    }
}
