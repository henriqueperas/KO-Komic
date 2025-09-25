using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderChangeVolume : MonoBehaviour
{
    [SerializeField] AudioManager am;

    [SerializeField] string floatParam;

    private void Start()
    {
        if(floatParam == "MusicVolume")
        {
            ChangeVolumeMusic();
            am.floatVolumeMusic = gameObject.GetComponent<Slider>().value;
        }
        else
        {
            ChangeVolumeVFX();
            am.floatVolumeSFX = gameObject.GetComponent<Slider>().value;
        }
    }

    private void Update()
    {
        float currentVolume;

        if (floatParam == "MusicVolume")
        {
            am.audioMixer.GetFloat(floatParam, out currentVolume);
            am.floatVolumeMusic = currentVolume;
        }
        else
        {
            am.audioMixer.GetFloat(floatParam, out currentVolume);
            am.floatVolumeSFX = currentVolume;
        }
    }

    public void ChangeVolumeMusic()
    {
        am.SetMusicVolume(gameObject.GetComponent<Slider>().value);

        
    }

    public void ChangeVolumeVFX()
    {
        am.SetSFXVolume(gameObject.GetComponent<Slider>().value);

        
    }
}
