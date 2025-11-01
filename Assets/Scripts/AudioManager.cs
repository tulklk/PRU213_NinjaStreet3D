
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;   // Nhạc nền
    public AudioSource sfxSource;     // Hiệu ứng 
    public AudioSource drivingSource; // Loop âm xe 

    [Header("Audio Clips (optional)")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;
    public AudioClip coinCollect;
    public AudioClip coinCollectMysteryBox;
    public AudioClip gameOver;
    public AudioClip boostSound;
    public AudioClip vehicleDepart;
    public AudioClip vehicleDriving;
    public AudioClip spinMusic;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Defaults")]
    public bool playBackgroundOnStart = true;
    public bool defaultMusicOn = true;
    public bool defaultSfxOn = true;

    // === Runtime state ===
    private bool musicOn;
    private bool sfxOn;

    // ---- PlayerPrefs keys ----
    const string KEY_MUSIC_VOL = "MusicVolume";
    const string KEY_SFX_VOL = "SFXVolume";
    const string KEY_MUSIC_ON = "MusicOn";
    const string KEY_SFX_ON = "SFXOn";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }

        // Nguồn rỗng thì tự tạo nhanh để tránh null
        if (!musicSource) musicSource = gameObject.AddComponent<AudioSource>();
        if (!sfxSource) sfxSource = gameObject.AddComponent<AudioSource>();
        if (!drivingSource) drivingSource = gameObject.AddComponent<AudioSource>();

        // Load prefs
        musicVolume = PlayerPrefs.GetFloat(KEY_MUSIC_VOL, 1f);
        sfxVolume = PlayerPrefs.GetFloat(KEY_SFX_VOL, 1f);
        musicOn = PlayerPrefs.GetInt(KEY_MUSIC_ON, defaultMusicOn ? 1 : 0) == 1;
        sfxOn = PlayerPrefs.GetInt(KEY_SFX_ON, defaultSfxOn ? 1 : 0) == 1;

        // Áp dụng
        ApplyVolumes();
        ApplyMutes();

        // Bật nhạc nền nếu có
        if (playBackgroundOnStart && backgroundMusic)
            PlayMusic(backgroundMusic, true);
    }

    // ==================== Music ====================
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!musicSource || !clip) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.mute = !musicOn;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource) musicSource.Stop();
    }

    public void FadeMusicTo(float targetVolume, float duration)
    {
        if (!musicSource) return;
        StartCoroutine(CoFade(musicSource, Mathf.Clamp01(targetVolume), Mathf.Max(0.01f, duration)));
    }

    IEnumerator CoFade(AudioSource src, float to, float t)
    {
        float from = src.volume;
        float time = 0f;
        while (time < t)
        {
            time += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(from, to, time / t);
            yield return null;
        }
        src.volume = to;
    }

    // ==================== SFX ====================
    public void PlaySFX(AudioClip clip, float volScale = 1f)
    {
        if (!sfxOn || !sfxSource || !clip) return;
        sfxSource.PlayOneShot(clip, sfxVolume * Mathf.Clamp01(volScale));
    }

    // Presets nhanh
    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlayCoinCollect() => PlaySFX(coinCollect);
    public void PlayCoinCollectMysteryBox() => PlaySFX(coinCollectMysteryBox);
    public void PlayGameOver() => PlaySFX(gameOver);
    public void PlayBoostSound() => PlaySFX(boostSound);
    public void PlayVehicleDepart() => PlaySFX(vehicleDepart, 0.5f);

    // ==================== Spin music bằng SFX channel ====================
    public void PlaySpinMusic()
    {
        if (!sfxSource || !spinMusic) return;
        sfxSource.clip = spinMusic;
        sfxSource.loop = true;
        sfxSource.volume = sfxVolume;
        sfxSource.mute = !sfxOn;
        sfxSource.Play();
    }

    public void StopSpinMusic()
    {
        if (sfxSource && sfxSource.clip == spinMusic) sfxSource.Stop();
    }
    //public void StopDrivingMusic()
    //{
    //    if (sfxSource && sfxSource.clip == vehicleDriving) sfxSource.Stop();
    //}


    // ==================== Vehicle driving (loop riêng) ====================
    public void PlayVehicleDrivingLoop()
    {
        if (!drivingSource || !vehicleDriving) return;
        drivingSource.clip = vehicleDriving;
        drivingSource.loop = true;
        drivingSource.volume = sfxVolume * 0.5f;
        drivingSource.mute = !sfxOn;
        drivingSource.Play();
    }

    public void StopVehicleDrivingMusic()
    {
        if (drivingSource) drivingSource.Stop();
    }

    // ==================== Toggles từ UI ====================
    // Gọi trực tiếp từ SwitchToggle (Role = Music/SFX)
    public void ToggleMusic(bool isOn)
    {
        musicOn = isOn;
        ApplyMutes();
        PlayerPrefs.SetInt(KEY_MUSIC_ON, musicOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX(bool isOn)
    {
        sfxOn = isOn;
        ApplyMutes();
        PlayerPrefs.SetInt(KEY_SFX_ON, sfxOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ==================== Volumes từ Slider ====================
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource) musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat(KEY_MUSIC_VOL, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        // Đặt ngay cho các nguồn SFX/loop
        if (sfxSource && !sfxSource.loop) sfxSource.volume = sfxVolume;
        if (drivingSource) drivingSource.volume = sfxVolume * 0.5f;
        PlayerPrefs.SetFloat(KEY_SFX_VOL, sfxVolume);
        PlayerPrefs.Save();
    }

    // ==================== Helpers ====================
    private void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource && !sfxSource.loop) sfxSource.volume = sfxVolume;
        if (drivingSource) drivingSource.volume = sfxVolume * 0.5f;
    }

    private void ApplyMutes()
    {
        if (musicSource) musicSource.mute = !musicOn;
        if (sfxSource) sfxSource.mute = !sfxOn;
        if (drivingSource) drivingSource.mute = !sfxOn;
    }

    // Option tiện lợi
    public void SetBackgroundMusic(AudioClip clip, bool playImmediately = true, bool loop = true)
    {
        backgroundMusic = clip;
        if (playImmediately && clip) PlayMusic(clip, loop);
    }

    // Reset toàn bộ về mặc định (nếu cần)
    public void ResetAudioPrefs(bool applyImmediately = true)
    {
        PlayerPrefs.DeleteKey(KEY_MUSIC_VOL);
        PlayerPrefs.DeleteKey(KEY_SFX_VOL);
        PlayerPrefs.DeleteKey(KEY_MUSIC_ON);
        PlayerPrefs.DeleteKey(KEY_SFX_ON);

        musicVolume = 1f; sfxVolume = 1f;
        musicOn = defaultMusicOn; sfxOn = defaultSfxOn;

        if (applyImmediately)
        {
            ApplyVolumes();
            ApplyMutes();
        }
    }
}
