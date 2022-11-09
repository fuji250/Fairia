using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private SoundManager soundManager;
    private GameObject pausePanel;
    public Image soundImage;
    public Sprite SoundOnSpr; 
    public Sprite SoundOffSpr; 
    
    // Start is called before the first frame update
    void Start()
    {
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        pausePanel = this.gameObject;
        pausePanel.SetActive(false);

        //ChangeSoundButtonImage();
    }

    public void ShowPausePanel()
    {
        Debug.Log("ShowPausePanel");
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
    
    public void HidePausePanel()
    {
        Debug.Log("HidePausePanel");
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void ChangeSoundButtonImage()
    {
        soundManager.SwitchingSound();
        if (PlayerPrefs.GetInt("soundVolume",0) == 1)
        {
            soundImage.sprite = SoundOnSpr;
        }
        else
        {
            soundImage.sprite = SoundOffSpr;
        }
    }

    public void ToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
}
