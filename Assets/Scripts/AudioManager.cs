using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource drivingSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;
    public AudioClip coinCollect;
    public AudioClip coinCollectMysteryBox;
    public AudioClip gameOver;
    public AudioClip boostSound;
    public AudioClip vehicleDepart;
    public AudioClip vehicleDriving;
    public AudioClip spinMusic;


    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // LOAD từ PlayerPrefs
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolumeSettings();

        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }


    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlaySpinMusic()
    {
        if (spinMusic == null) return;
        sfxSource.clip = spinMusic;
        sfxSource.loop = true;
        sfxSource.volume = sfxVolume;
        sfxSource.Play();
    }

    public void StopSpinMusic()
    {
        if (sfxSource.isPlaying && sfxSource.clip == spinMusic)
            sfxSource.Stop();
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void ToggleMusic(bool isOn)
    {
        musicSource.mute = !isOn;
    }

    public void ToggleSFX(bool isOn)
    {
        sfxSource.mute = !isOn;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }


    private void ApplyVolumeSettings()
    {
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    // Gợi ý hàm gọi nhanh
    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlayCoinCollect() => PlaySFX(coinCollect);
    public void PlayCoinCollectMysteryBox() => PlaySFX(coinCollectMysteryBox);
    public void PlayGameOver() => PlaySFX(gameOver);
    public void PlayBoostSound() => PlaySFX(boostSound);
    public void PlayVehicleDepart()
    {
        if (vehicleDepart == null) return;
        sfxSource.PlayOneShot(vehicleDepart, sfxVolume * 0.5f);
    }
    public void PlayVehicleDriving()
    {
        if (vehicleDriving == null) return;
        sfxSource.PlayOneShot(vehicleDriving, sfxVolume * 0.5f);


    }
    public void PlayVehicleDrivingLoop()
    {
        if (vehicleDriving == null || drivingSource == null) return;
        drivingSource.clip = vehicleDriving;
        drivingSource.loop = true;
        drivingSource.volume = sfxVolume * 0.5f;
        drivingSource.Play();
    }

    public void StopVehicleDriving()
    {
        if (drivingSource != null) drivingSource.Stop();
    }

}
