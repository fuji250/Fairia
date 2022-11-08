using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject targetMoveBlock;
    public Sprite imageOn;
    public Sprite imageOff;
    private bool on = false;//スイッチの状態(true:押されている　false：押されていない)

    // Start is called before the first frame update
    void Start()
    {
        if (on)
        {
            GetComponent<SpriteRenderer>().sprite = imageOn;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = imageOff;
        }
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
                GetComponent<SpriteRenderer>().sprite = imageOn;
                MoveGround movBlock = targetMoveBlock.GetComponent<MoveGround>();
                movBlock.Move();
            }
        }
    }
}
