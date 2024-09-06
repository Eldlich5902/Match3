using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSetting : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer mixer;
    private float value;

    private void Start()
    {
        mixer.GetFloat("Volume", out value);
        volumeSlider.value = value;
    }

    public void ChangeVolume()
    {
        mixer.SetFloat("Volume", volumeSlider.value);
    }
    
}
