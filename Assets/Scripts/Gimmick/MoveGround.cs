using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGround : MonoBehaviour
{
    public float moveX = 0.0f;          //X移動距離
    public float moveY = 0.0f;          //Y移動距離
    public float times = 0.0f;          //時間
    public float weight = 0.0f;         //停止時間
    public bool isMoveWhenOn = false;   //乗った時に動くフラグ

    public bool isCanMove = true;       //動くフラグ
    float perDX;                        //１フレームのX移動値
    float perDY;                        //１フレームのY移動値
    Vector3 defPos;                     //初期位置
    bool isReverse = false;             //反転フラグ
    public bool isRoop = false;             //反転フラグ

    // Start is called before the first frame update
    void Start()
    {
        //初期位置
        defPos = transform.position;
        //１フレームの移動時間取得
        float timestep = Time.fixedDeltaTime;
        //１フレームのX移動値
        perDX = moveX / (1.0f / timestep * times);
        //１フレームのX移動値
        perDY = moveY / (1.0f / timestep * times);

        if (isMoveWhenOn)
        {
            //乗った時に動くので最初は動かさない
            isCanMove = false;
        }
    }
    
    private void FixedUpdate()
    {
        if (isCanMove)
        {
            //移動中
            float x = transform.position.x;
            float y = transform.position.y;
            bool endX = false;
            bool endY = false;
            if (isReverse)
            {
                //逆方向移動中...
                //移動量がプラスで移動位置が初期位置より小さい
                //または、移動量がマイナスで移動位置が初期位置より大きい
                if ((perDX >= 0.0f && x <= defPos.x) || (perDX < 0.0f && x >= defPos.x))
                {
                    //移動量が+で
                    endX = true;	//X方向の移動終了
                }
                if ((perDY >= 0.0f && y <= defPos.y) || (perDY < 0.0f && y >= defPos.y))
                {
                    endY = true;	//Y方向の移動終了
                }
                //床を移動させる
                transform.Translate(new Vector3(-perDX, -perDY, defPos.z));
            }
            else
            {
                //正方向移動中...
                //移動量がプラスで位置が初期+移動距離より大きい
                //または、移動量がマイナスで位置が初期+移動距離より小さい
                if ((perDX >= 0.0f && x >= defPos.x + moveX) || (perDX < 0.0f && x <= defPos.x + moveX))
                {
                    endX = true;	//X方向の移動終了
                }
                if ((perDY >= 0.0f && y >= defPos.y + moveY) || (perDY < 0.0f && y <= defPos.y + moveY))
                {
                    endY = true;	//Y方向の移動終了
                }
                //床を移動させる
                Vector3 v = new Vector3(perDX, perDY, defPos.z);
                transform.Translate(v);
            }

            if (endX && endY)
            {
                //移動終了
                if (isReverse)
                {
                    //正方向移動に戻る前に初期位置に戻す、そうしておかないと位置がずれていくため
                    transform.position = defPos;
                }

                if (isRoop)
                {
                    isReverse = !isReverse; //フラグを反転させる

                }
                
                isCanMove = false;      //移動フラグ下ろす
                if (isMoveWhenOn == false)
                {
                    //乗った時に動くフラグOFF
                    Invoke("Move", weight);  //移動フラグを立てる遅延実行
                }
            }
        }
    }

    //移動フラグを立てる
    public void Move()
    {
        isCanMove = true;
    }

    //移動フラグを下ろす
    public void Stop()
    {
        isCanMove = false;
    }

    //接触開始
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //接触したのがプレイヤーなら移動床の子にする
            collision.transform.SetParent(transform);
            if (isMoveWhenOn)
            {
                //乗った時に動くフラグON
                isCanMove = true;   //移動フラグを立てる
            }
        }
    }
    //接触終了
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //接触したのがプレイヤーなら移動床の子から外す
            collision.transform.SetParent(null);
        }
    }
}
