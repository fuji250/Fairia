using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

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

    public void SwitchingSound()
    {
        if (audioSourceBGM.volume == 0)
        {
            audioSourceBGM.volume = 0.5f;
            audioSourceSE.volume = 0.5f;
        }
        else
        {
            audioSourceBGM.volume = 0;
            audioSourceSE.volume = 0;
        }
            

    }
}
