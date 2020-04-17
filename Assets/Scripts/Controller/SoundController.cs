using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using Random = UnityEngine.Random;

public class SoundController : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource BackUpEffectSource;
    public AudioSource MusicSource;

    [Range(0, 1)]
    public float maximuBGMVolume = .25f;

    public AudioClip mainMenuBGM;
    public AudioClip bossBGM;
    public AudioClip normalBGM;


    public AudioClip winSound;
    public AudioClip loseSound;
    public AudioClip reviveSound;
    public AudioClip buttonSound;
    public AudioClip bounceSound;

    public List<AudioClip> listFireSound;



    // Random pitch adjustment range.
    public float LowPitchRange = .95f;
    public float HighPitchRange = 1.05f;

    // Singleton instance.
    public static SoundController Instance = null;

    public bool isBGMOn = true;
    public bool isSFXOn = true;

    private float lastPlaySwipeSound = 0;
    private DelayFunctionHelper _delay;
    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
            //isSFXOn = PlayerPrefs.GetInt("SFX") == 1 ? true : false;
            //isBGMOn = PlayerPrefs.GetInt("BGM") == 1 ? true : false;

        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }
        _delay = gameObject.AddComponent<DelayFunctionHelper>();
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (!PlayerPrefs.HasKey("BGM") || !PlayerPrefs.HasKey("SFX"))
        {
            PlayerPrefs.SetInt("BGM", 1);
            PlayerPrefs.SetInt("SFX", 1);
        }

        isBGMOn = PlayerPrefs.GetInt("BGM", 0) == 1 ? true : false;
        isSFXOn = PlayerPrefs.GetInt("SFX", 0) == 1 ? true : false;

        if (!isBGMOn)
            MusicSource.Stop();
    }


    public void PlayBGM(AudioClip clip, bool skipFade = false)
    {


        if (skipFade)
        {
            MusicSource.volume = maximuBGMVolume;
            MusicSource.clip = clip;

            if (isBGMOn)
                MusicSource.Play();
        }
        else
        {
            MusicSource.DOFade(0, 1f).OnComplete(() =>
            {
                MusicSource.volume = maximuBGMVolume;
                MusicSource.clip = clip;
                if (isBGMOn)
                    MusicSource.Play();
            });
        }
    }

    public void PlayBGM()
    {
        MusicSource.Play();
    }
    public void StopBGM()
    {
        MusicSource.Stop();
    }
    public void PlayMainMenuBGM()
    {
        PlayBGM(mainMenuBGM);
    }
    public void PlayBossBGM()
    {
        PlayBGM(bossBGM);
    }
    public void PlayNormalBGM()
    {
        PlayBGM(normalBGM, true);
    }

    public void SwitchSFX()
    {
        isSFXOn = !isSFXOn;
        if (isSFXOn)
            PlayerPrefs.SetInt("SFX", 1);
        else
            PlayerPrefs.SetInt("SFX", 0);
    }
    public void SwitchBGM()
    {
        isBGMOn = !isBGMOn;
        if (isBGMOn)
        {
            PlayerPrefs.SetInt("BGM", 1);
            MusicSource.Play();
        }
        else
        {
            PlayerPrefs.SetInt("BGM", 0);
            MusicSource.Stop();
        }
    }
    public void Mute()
    {
        StopBGM();
        isSFXOn = false;
        isBGMOn = false;
    }
    public void UnMute()
    {
        PlayBGM();
        isSFXOn = true;
        isBGMOn = true;
    }

    public void PlayFireSound()
    {
        Play(listFireSound[Random.Range(0,listFireSound.Count)]);
    }

    public void PlayBounceSound()
    {
        Play((bounceSound));
    }
    
    public void PlayReviveSound()
    {
        Play(reviveSound, true);
    }
    public void PlayLoseSound()
    {
        Play(loseSound);
    }
    public void PlayButtonSound()
    {
        Play(buttonSound);
    }
    public void PlayWinSound()
    {
        Play(winSound, false, 0.2f);
        //_delay.delayFunction(() =>
        //{
        //    PlayNormalBGM();
        //}, 4f);
    }


    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip, bool pauseBGM = false, float volume = 1f, float pitch = 1)
    {
        if (!isSFXOn)
            return;
        if (EffectsSource.isPlaying)
        {
            BackUpEffectSource.volume = volume;
            BackUpEffectSource.pitch = pitch;
            BackUpEffectSource.PlayOneShot(clip);
        }
        else
        {
            EffectsSource.volume = volume;
            EffectsSource.pitch = pitch;
            EffectsSource.PlayOneShot(clip);
        }

        if (pauseBGM)
        {
            MusicSource.Pause();
            _delay.delayFunction(() =>
            {
                MusicSource.UnPause();
                MusicSource.volume = 0;
                MusicSource.DOFade(maximuBGMVolume, 1);
            }, 3);
        }

    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }
}
