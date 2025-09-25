using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer References")]
    public AudioMixer audioMixer;
    public string musicVolumeParameter = "MusicVolume";
    public string sfxVolumeParameter = "SFXVolume";

    [Header("Music Settings")]
    public int musicSources = 2;
    public float crossfadeDuration = 2f;
    public float floatVolumeMusic = 0.5f;

    [Header("SFX Settings")]
    public int maxSFXInstances = 5;
    public float floatVolumeSFX = 0.5f;

    [SerializeField] List<AudioSource> musicSourcesList = new List<AudioSource>();
    [SerializeField] List<AudioSource> sfxPool = new List<AudioSource>();
    //[SerializeField] Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InitializeAudioSources();
    }

    void InitializeAudioSources()
    {
        // Cria sources para m�sica (crossfade)
        for (int i = 0; i < musicSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
            source.loop = true;
            musicSourcesList.Add(source);
        }

        // Cria pool de SFX
        for (int i = 0; i < maxSFXInstances; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            source.playOnAwake = false;
            sfxPool.Add(source);
        }

    }

    #region MUSIC METHODS

    /// <summary>
    /// Toca uma m�sica com op��o de loop
    /// </summary>
    public void PlayMusic(AudioClip musicClip)
    {
        bool loop = true;
        float volume = 1f;

        if (musicClip == null) return;

        // Para a m�sica atual suavemente
        StopAllCoroutines();
        StartCoroutine(CrossfadeMusic(musicClip, loop, volume));
    }

    /// <summary>
    /// Transi��o suave entre m�sicas
    /// </summary>
    private IEnumerator CrossfadeMusic(AudioClip newClip, bool loop, float targetVolume)
    {
        AudioSource fadingOutSource = musicSourcesList[0];
        AudioSource fadingInSource = musicSourcesList[1];

        // Configura nova m�sica
        fadingInSource.clip = newClip;
        fadingInSource.loop = loop;
        fadingInSource.volume = 0f;
        fadingInSource.Play();

        float timer = 0f;

        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / crossfadeDuration;

            // Fade out da m�sica atual
            fadingOutSource.volume = Mathf.Lerp(fadingOutSource.volume, 0f, progress);

            // Fade in da nova m�sica
            fadingInSource.volume = Mathf.Lerp(0f, targetVolume, progress);

            yield return null;
        }

        // Finaliza transi��o
        fadingOutSource.Stop();
        fadingOutSource.volume = 0f;
        fadingInSource.volume = targetVolume;

        // Troca as refer�ncias para pr�xima transi��o
        musicSourcesList.RemoveAt(0);
        musicSourcesList.Add(fadingOutSource);
    }

    /// <summary>
    /// Para a m�sica atual
    /// </summary>
    public void StopMusic()
    {
        foreach (AudioSource source in musicSourcesList)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// Pausa a m�sica atual
    /// </summary>
    public void PauseMusic()
    {
        foreach (AudioSource source in musicSourcesList)
        {
            source.Pause();
        }
    }

    /// <summary>
    /// Retoma a m�sica pausada
    /// </summary>
    public void ResumeMusic()
    {
        foreach (AudioSource source in musicSourcesList)
        {
            source.UnPause();
        }
    }

    #endregion

    #region SFX METHODS

    /// <summary>
    /// Toca um efeito sonoro �nico
    /// </summary>
    public void PlaySFX(AudioClip sfxClip, float volume = 1f, float pitch = 1f)
    {
        if (sfxClip == null) return;

        AudioSource availableSource = GetAvailableSFXSource();
        if (availableSource != null)
        {
            availableSource.clip = sfxClip;
            availableSource.volume = volume;
            availableSource.pitch = pitch;
            availableSource.Play();
        }
    }

    /// <summary>
    /// Toca efeito sonoro a partir de ScriptableObject
    /// </summary>
    public void PlaySFXFromSO(AttackData soundEffect, float volumeMultiplier = 1f)
    {
        if (soundEffect == null || soundEffect.audioClip == null) return;

        PlaySFX(soundEffect.audioClip, soundEffect.volume * volumeMultiplier, soundEffect.pitch);
    }

    /// <summary>
    /// Busca source de SFX dispon�vel na pool
    /// </summary>
    private AudioSource GetAvailableSFXSource()
    {
        foreach (AudioSource source in sfxPool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // Se n�o achou dispon�vel, usa o mais antigo
        return sfxPool[0];
    }

    /// <summary>
    /// Para todos os SFX
    /// </summary>
    public void StopAllSFX()
    {
        foreach (AudioSource source in sfxPool)
        {
            source.Stop();
        }
    }

    #endregion

    #region VOLUME CONTROL

    /// <summary>
    /// Ajusta volume da m�sica via AudioMixer
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        // Converte volume linear (0-1) para decib�is (-80 to 0)
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(musicVolumeParameter, dB);
    }

    /// <summary>
    /// Ajusta volume dos SFX via AudioMixer
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        audioMixer.SetFloat(sfxVolumeParameter, dB);
    }

    /// <summary>
    /// Salva configura��es de volume no PlayerPrefs
    /// </summary>
    public void SaveVolumeSettings()
    {
        audioMixer.GetFloat(musicVolumeParameter, out float musicdB);
        audioMixer.GetFloat(sfxVolumeParameter, out float sfxdB);

        PlayerPrefs.SetFloat("MusicVolume", musicdB);
        PlayerPrefs.SetFloat("SFXVolume", sfxdB);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Carrega configura��es de volume salvas
    /// </summary>
    public void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            audioMixer.SetFloat(musicVolumeParameter, PlayerPrefs.GetFloat("MusicVolume"));
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            audioMixer.SetFloat(sfxVolumeParameter, PlayerPrefs.GetFloat("SFXVolume"));
        }
    }

    #endregion

    #region UTILITY METHODS

    /// <summary>
    /// Muda pitch de todos os SFX (�til para slow motion)
    /// </summary>
    public void SetGlobalSFXPitch(float pitch)
    {
        foreach (AudioSource source in sfxPool)
        {
            source.pitch = pitch;
        }
    }

    /// <summary>
    /// Verifica se alguma m�sica est� tocando
    /// </summary>
    public bool IsMusicPlaying()
    {
        foreach (AudioSource source in musicSourcesList)
        {
            if (source.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    #endregion
}
