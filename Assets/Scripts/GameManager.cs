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
        //�p�l�����\���ɂ���
        HidePanel();
        
        //ColorAdjustments���擾����
        volume.profile.TryGet(out colorAdjustments);
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
            nPanel.SetActive(false);
            rPanel.SetActive(true);

            //�p�l������b�����\��
            failedPanel.SetActive(true);
            
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
        */
        
        //�Q�[����Ԃ��L�^����
        previousGameState = PlayerManager.gameState;
    }

    void StartFadeOut()
    {
        colorAdjustments.postExposure.value += fadeSpeed * Time.deltaTime; // b)�s�����x�����X�ɂ�����
        if (colorAdjustments.postExposure.value >= 10)
        {
            // d)���S�ɕs�����ɂȂ����珈���𔲂���
            isFadeOut = false;
            isFadeIn = true;
        }
    }
    
    void StartFadeIn()
    {
        colorAdjustments.postExposure.value -= fadeSpeed * Time.deltaTime; //a)�s�����x�����X�ɉ�����
        if (colorAdjustments.postExposure.value <= 0)
        {
            //c)���S�ɓ����ɂȂ����珈���𔲂���
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
