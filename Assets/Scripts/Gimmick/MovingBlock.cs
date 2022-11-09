using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    private GameObject MovingBlock1;
    bool isTrigger1 = false;

    public float speed;
    public float maxHeight;
    public float checkHeight;
    float evHeight1;
    public bool IsUpper;
    
    private int previousGameState;
    private Vector3 firstTransform;

    void Start()
    {
        MovingBlock1 = this.gameObject;
        firstTransform = MovingBlock1.transform.position;
    }

    void FixedUpdate()
    {
        if (previousGameState !=  PlayerManager.gameState && PlayerManager.gameState == (int)PlayerManager.State.Playing)
        {
            MovingBlock1.transform.position = firstTransform;
        }
        //ゲーム状態を記録する
        previousGameState = PlayerManager.gameState;
        
        evHeight1 = MovingBlock1.transform.position.y;

        if (IsUpper)
        {
            if (isTrigger1 == false && evHeight1 >= maxHeight)
            {
                //下に動かす
                MovingBlock1.transform.Translate(new Vector3(0, -1 * speed, 0));
            }
        }
        else
        {
            if (isTrigger1 == false && evHeight1 <= maxHeight)
            {
                //下に動かす
                MovingBlock1.transform.Translate(new Vector3(0, 1 * speed, 0));
            }
        }
        
    }
    
    //接触開始
    void OnTriggerStay2D(Collider2D col)
    {
        isTrigger1 = true;

        if (IsUpper)
        {
            //動きを止める
            if (col.gameObject.CompareTag("Player"))
            {
                MovingBlock1.transform.Translate(new Vector3(0, 0, 0));
            }
        }
        else
        {
            if (evHeight1 <= checkHeight)
            {
                //動きを止める
                if (col.gameObject.CompareTag("Player"))
                {
                    MovingBlock1.transform.Translate(new Vector3(0, 0, 0));
                }
            }
        }
        
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        isTrigger1 = false;
    }
}

