using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject targetMoveBlock;
    private SpriteRenderer spriteRenderer;
    public Sprite imageOn;
    public Sprite imageOff;
    private bool on = false;//スイッチの状態(true:押されている　false：押されていない)

    private MoveGround moveGround;
    
    private int previousGameState;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveGround = targetMoveBlock.GetComponent<MoveGround>();
    }

    private void Update()
    {
        if (previousGameState !=  PlayerManager.gameState && PlayerManager.gameState == (int)PlayerManager.State.Gameover)
        {
            spriteRenderer.sprite = imageOff;
            on = false;
            moveGround.Init();

        }
        //ゲーム状態を記録する
        previousGameState = PlayerManager.gameState;
    }

    //接触開始
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            //押されていない状況で触れたら
            if (!on)
            {
                on = true;
                spriteRenderer.sprite = imageOn;
                moveGround.Move();
            }
        }
    }
}
