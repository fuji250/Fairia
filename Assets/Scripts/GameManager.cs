using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public GameObject failedPanel;
    public TextMeshProUGUI failedText;
    
    public Volume volume;
    private int previousGameState;


    // Start is called before the first frame update
    void Start()
    {
        //パネルを非表示にする
        HidePanel();

        PlayerManager.gameState = (int)PlayerManager.State.Playing;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gameState == (int)PlayerManager.State.Playing)
        {
                    HidePanel();
        }
        else if (PlayerManager.gameState == (int)PlayerManager.State.Gameclear)
        {

        }
        else if (previousGameState !=  PlayerManager.gameState && PlayerManager.gameState == (int)PlayerManager.State.Gameover)
        {
            ShowFailedPanel();
        }

        //RESET
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(PlayerManager.gameState);
        }
        
        //ゲーム状態を記録する
        previousGameState = PlayerManager.gameState;
    }

    public void ShowFailedPanel()
    {
        //失敗した回数
        SaveData.FailedData failedData = SaveData.LoadPlayerData();
        failedData.failedNum  = SaveData.LoadPlayerData().failedNum +1;
        SaveData.SavePlayerData(failedData);
        

        //パネルを一秒だけ表示
        failedText.text = failedData.failedNum.ToString();
        failedPanel.SetActive(true);
            
        Invoke(nameof(HidePanel),1f);
    }

    void HidePanel()
    {
        failedPanel.SetActive(false);
    }

    public delegate void LabelUpdateDelegate();
    
    public void Reset()
    {
        SoundManager.instance.PlaySE(0);
        PlayerManager.gameState = (int)PlayerManager.State.Gameover;
        Invoke(nameof(Load),1f);
    }

    private void Load()
    {
        //HidePanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
