using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody2D rbody;//Rigidbody2Dの変数
    public float axisH = 0.0f;//入力
    public float speed = 5.0f;//移動速度

    public float jump = 9.0f;//ジャンプ力
    public LayerMask groundLayer;//着地できるレイヤー

    public BoxCollider2D body;
    public BoxCollider2D up;
    public BoxCollider2D down;


    private bool _goJump = false;//ジャンプ開始フラグ
    private bool _onGround = false;//地面に立っているフラグ

    public static string gameState = "playing";//ゲームの状態


    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody2Dを取ってくる
        rbody = this.GetComponent<Rigidbody2D>();
        gameState = "playing";//ゲーム中にする
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState != "playing")
        {
            return;
        }

            //水平方向の入力をチェックする
            axisH = Input.GetAxisRaw("Horizontal");
        
        //向きの調整
        if (axisH > 0.0f)
        {
            //右向き
            transform.localScale = new Vector2(1, 1);
        }
        else if(axisH < 0.0f)
        {
            //左向き
            transform.localScale = new Vector2(-1, 1);//左右反転させる
        }
        //キャラクターをジャンプさせる
        if (Input.GetButtonDown("Jump") && _onGround)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (gameState != "playing")
        {
            return;
        }

        //地上判定 インスペクタ上でGhostUpも指定
        var transform1 = transform;
        var position = transform1.position;
        _onGround = Physics2D.Linecast(position - (transform1.up * 0.57f) + (transform1.right * 0.5f), position - (transform1.up * 0.57f) - (transform1.right * 0.5f), groundLayer);
        

        //速度を更新する
        rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);

        if (_goJump)
        {
            Vector2 jumpPw = new Vector2(0, jump);//ジャンプさせるベクトルを作る
            rbody.velocity = new Vector2(0, 0);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);//瞬間的な力を加える
            _goJump = false;
        }

    }

    public void Jump()
    {
        _goJump = true;
    }


    //接触開始
    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Goal":
                //Goal();//ゴール！！
                Debug.Log("ゴール");
                break;
            case "Dead":
                GameOver();//ゲームオーバー！！
                Debug.Log("ゲームオーバー");
                break;
            case "FinalGoal":
                //FadeManager.fadeColor = Color.white;
                //dia.SetActive(false);
                //LastFade.Instance.LoadScene("LAST", 1.0f);
                break;
        }
    }

    private void GameOver()
    {
        gameState = "gameover";
        GameStop();//ゲーム停止
        // =======================================
        // ゲームオーバー演出
        // =======================================

        //プレイヤー当たり判定を消す
        HideCollider();

        //プレイヤーを上に少し跳ね上げる演出
        rbody.AddForce(new Vector2(0, 100), ForceMode2D.Impulse);
    }
    private void GameStop()
    {
        //速度を０にして強制停止
        rbody.velocity = new Vector2(0, 0);
    }


    private void HideCollider()
    {
        body.enabled = false;
        up.enabled = false;
        down.enabled = false;
    }
    private void ShowCollider()
    {
        body.enabled = true;
        up.enabled = true;
        down.enabled = true;
    }


}
