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

    [SerializeField]
    public Button restartButtton;
    [SerializeField]
    public Button nextButtton;

    public GameObject panel;
    public GameObject failedPanel;
    public TextMeshProUGUI failedText;
    
    public GameObject rPanel;
    public GameObject nPanel;
    
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    
    private int previousGameState;
    private float fadeSpeed = 40f;
    private bool isFadeIn;
    private bool isFadeOut;
    

    // Start is called before the first frame update
    void Start()
    {
        //パネルを非表示にする
        HidePanel();
        
        //ColorAdjustmentsを取得する
        volume.profile.TryGet(out colorAdjustments);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gameState == (int)PlayerManager.State.playing)
        {
                    HidePanel();
        }
        else if (PlayerManager.gameState == (int)PlayerManager.State.gameclear)
        {

        }
        else if (previousGameState !=  PlayerManager.gameState && PlayerManager.gameState == (int)PlayerManager.State.gameover)
        {
            nPanel.SetActive(false);
            rPanel.SetActive(true);

            //パネルを一秒だけ表示
            failedPanel.SetActive(true);
            SoundManager.instance.PlaySE(0);
            Invoke("HidePanel",1);
            
            isFadeOut = true;
        }

        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        /*
        if (isFadeOut) 
        {
            StartFadeOut ();
        }
        if(isFadeIn)
        {
            StartFadeIn ();
        }
        previousGameState = PlayerManager.gameState;
        */
    }

    void StartFadeOut()
    {
        colorAdjustments.postExposure.value += fadeSpeed * Time.deltaTime; // b)不透明度を徐々にあげる
        if (colorAdjustments.postExposure.value >= 10)
        {
            // d)完全に不透明になったら処理を抜ける
            isFadeOut = false;
            isFadeIn = true;
        }
    }
    
    void StartFadeIn()
    {
        colorAdjustments.postExposure.value -= fadeSpeed * Time.deltaTime; //a)不透明度を徐々に下げる
        if (colorAdjustments.postExposure.value <= 0)
        {
            //c)完全に透明になったら処理を抜ける
            isFadeIn = false;
        }
    }

    void HidePanel()
    {
        nPanel.SetActive(false);
        rPanel.SetActive(false);
        
        failedPanel.SetActive(false);
    }
}
