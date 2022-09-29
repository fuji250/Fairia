using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    public Button restartButtton;
    [SerializeField]
    public Button nextButtton;

    public GameObject panel;
    public GameObject rPanel;
    public GameObject nPanel;

    

    // Start is called before the first frame update
    void Start()
    {
        //ƒpƒlƒ‹‚ð”ñ•\Ž¦‚É‚·‚é
        HidePanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gameState == "playing")
        {
                    HidePanel();


        }
        else if (PlayerManager.gameState == "gameclear")
        {

        }
        else if (PlayerManager.gameState == "gameover")
        {
            nPanel.SetActive(false);
            rPanel.SetActive(true);
        }
    }

    void HidePanel()
    {
        nPanel.SetActive(false);
        rPanel.SetActive(false);
    }
}
