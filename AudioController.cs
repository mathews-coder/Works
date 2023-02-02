using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public static AudioController InstanceObj;
    public AudioListener AudioListerObj;
    public Toggle SoundToggle,MusicToggle;
    public AudioSource[] ListAudioClips;
    public AudioSource HomePageMusic;
    public Slider SoundEfxSlider, MusicSlider;
    bool checkSoundBool;
    // Start is called before the first frame update
    private void Start()
    {
        if (PlayerPrefs.GetInt("FirstEverLaunch") == 0)
        {
            PlayerPrefs.SetFloat("MusicValue",1); PlayerPrefs.SetFloat("SoundValue", 1);
        }

        InstanceObj = this;
        //PlayerPrefs.DeleteAll();//test
        SoundToggle.onValueChanged.AddListener(delegate {
            SoundOffOn(SoundToggle);
        });
        MusicToggle.onValueChanged.AddListener(delegate {
            MusicOffOn(MusicToggle);
        });
        //CheckSoundValues();
        HomePageMusic.ignoreListenerVolume = true;

        AudioListener.volume = PlayerPrefs.GetFloat("SoundValue");
        HomePageMusic.volume = PlayerPrefs.GetFloat("MusicValue");
        SoundEfxSlider.value = PlayerPrefs.GetFloat("SoundValue");
        MusicSlider.value = PlayerPrefs.GetFloat("MusicValue");
    }
    public void PlayAudio(int ch)
    {
        /* 0 - back/cool/settings
           1 - no thanks
           2 - upgrade main btn
           3 - externalAds coins/buy */
        ListAudioClips[ch].Play();
    }

    //settings slider
    public void EfxSliderChange(System.Single sliderVal)
    {
//        print(sliderVal);
        PlayerPrefs.SetFloat("SoundValue",sliderVal);
        AudioListener.volume = sliderVal;
    }
    public void MusicSliderChange(System.Single sliderVal)
    {
      //  print(sliderVal);
        PlayerPrefs.SetFloat("MusicValue", sliderVal);
        HomePageMusic.volume = sliderVal;
    }

    public void CheckSoundValues()
    {
        if (PlayerPrefs.GetInt("SoundOFF") == 0)
        {
            print("sndON");
            //PlayerPrefs.SetInt("SoundOFF", 1);
            AudioListerObj.enabled = true;
            SoundToggle.isOn = true;
        }
        else
        {
            print("sndOFF");
            //PlayerPrefs.SetInt("SoundOFF", 0);
            print(AudioListerObj.name);
            AudioListerObj.enabled = false;
            SoundToggle.isOn = false;
        }

        if (PlayerPrefs.GetInt("MusicOFF") == 0)
        {
            print("MusicON");
            //PlayerPrefs.SetInt("SoundOFF", 1);
            HomePageMusic.mute = false;
            MusicToggle.isOn = true;
        }
        else
        {
            print("MusicOFF");
            //PlayerPrefs.SetInt("SoundOFF", 0);
            HomePageMusic.mute = true;
            MusicToggle.isOn = false;
        }
    }
    public void SoundOffOn(Toggle TogleVal)
    {
        print("sndOFFONCALL");
        if(SoundToggle.isOn)
        {
            PlayerPrefs.SetInt("SoundOFF", 0);
            AudioListerObj.enabled = true;
        }
        else
        {
            PlayerPrefs.SetInt("SoundOFF", 1);
            AudioListerObj.enabled = false;
        }
            
    }
    public void MusicOffOn(Toggle TogleVal)
    {
        print("musicOFFONCALL");
        if (MusicToggle.isOn)
        {
            PlayerPrefs.SetInt("MusicOFF", 0);
            HomePageMusic.mute = false;
        }
        else
        {
            PlayerPrefs.SetInt("MusicOFF", 1);
            HomePageMusic.mute = true;
        }

    }
}
