using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;

    private void Start()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    private void Update()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", _sfxSlider.value);
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
    }
    public void ToggleSFX() 
    {
        AudioManager.Instance.ToggleSFX();
    }
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
        
    }

}
