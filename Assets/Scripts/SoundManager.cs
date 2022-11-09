using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private float bgmVolume;
    private float seVolume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public AudioSource audioSourceBGM; // BGMのスピーカー
    public AudioClip[] audioClipsBGM; 

    public AudioSource audioSourceSE; // SEのスピーカー
    public AudioClip[] audioClipSE; // ならす素材

    public int soundSwitch;
    
    void Start()
    {
        bgmVolume = audioSourceBGM.volume;
        seVolume = audioSourceSE.volume;

        SetupSoundVolume();
    }
    
    
    public void PlayBGM(string sceneName)
    {
        audioSourceBGM.Stop();
        switch (sceneName)
        {
            default:
            case "Title":
                audioSourceBGM.volume = 0.5f;
                audioSourceBGM.clip = audioClipsBGM[0];
                break;
            case "OP":
                audioSourceBGM.volume = 0.1f;
                audioSourceBGM.clip = audioClipsBGM[1];
                break;
            case "ED":
                audioSourceBGM.volume = 0.1f;
                audioSourceBGM.clip = audioClipsBGM[6];
                break;
        }
        audioSourceBGM.Play();
    }
    
    public void PlaySE(int index)
    {
        audioSourceSE.PlayOneShot(audioClipSE[index]); // SEを一度だけならす
    }

    public void SetupSoundVolume()
    {
        if (PlayerPrefs.GetInt("soundVolume", 1) == 1)
        {
            audioSourceBGM.volume = bgmVolume;
            audioSourceSE.volume = seVolume;
        }
        else
        {
            audioSourceBGM.volume = 0;
            audioSourceSE.volume = 0;
        }
    }
    
    
    public void SwitchingSound()
    {
        if (PlayerPrefs.GetInt("soundVolume",0) == 0)
        {
            audioSourceBGM.volume = bgmVolume;
            audioSourceSE.volume = seVolume;
            
            PlayerPrefs.SetInt("soundVolume",1);
            PlayerPrefs.Save ();
        }
        else
        {
            audioSourceBGM.volume = 0;
            audioSourceSE.volume = 0;
            
            PlayerPrefs.SetInt("soundVolume",0);
            PlayerPrefs.Save ();
        }
    }
}
