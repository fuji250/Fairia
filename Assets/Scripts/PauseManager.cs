using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PauseManager : MonoBehaviour
{
    private GameObject pausePanel;
    public Image soundImage;
    public Sprite SoundOnSpr; 
    public Sprite SoundOffSpr; 
    
    // Start is called before the first frame update
    void Start()
    {
        pausePanel = this.gameObject;
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowPausePanel()
    {
        Debug.Log("ShowPausePanel");
        pausePanel.SetActive(true);
    }
    
    public void HidePausePanel()
    {
        Debug.Log("HidePausePanel");

        pausePanel.SetActive(false);
    }

    public void ChangeSoundButtonImage()
    {
        if (soundImage.sprite == SoundOffSpr)
        {
            soundImage.sprite = SoundOnSpr;
        }
        else if (soundImage.sprite == SoundOnSpr)
        {
            soundImage.sprite = SoundOffSpr;
        }
    }
}
