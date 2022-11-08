using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public GameObject MovingBlock1;
    bool isTrigger1 = false;

    public float speed;
    public float height;
    public float checkHeight;
    float evHeight1;

    void Start()
    {
        evHeight1 = MovingBlock1.transform.position.y;
    }

    void FixedUpdate()
    {
        evHeight1 = MovingBlock1.transform.position.y;
        if (isTrigger1 == false && evHeight1 <= height)
        {
            //下に動かす
            MovingBlock1.transform.Translate(new Vector3(0, 1 * speed, 0));
        }
    }
    
    //接触開始
    void OnTriggerStay2D(Collider2D col)
    {
        if (evHeight1 >= checkHeight)
        {
            isTrigger1 = true;
            //動きを止める
            if (col.gameObject.CompareTag("Player"))
            {
                MovingBlock1.transform.Translate(new Vector3(0, 0, 0));
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        isTrigger1 = false;
    }
}

