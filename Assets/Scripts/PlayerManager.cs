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


    bool goJump = false;//ジャンプ開始フラグ
    bool onGround = false;//地面に立っているフラグ

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
        if (Input.GetButtonDown("Jump") && onGround)
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
        onGround = Physics2D.Linecast(transform.position - (transform.up * 0.57f) + (transform.right * 0.5f), transform.position - (transform.up * 0.57f) - (transform.right * 0.5f), groundLayer);
        

        //速度を更新する
        rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);

        if (goJump)
        {
            Vector2 jumpPw = new Vector2(0, jump);//ジャンプさせるベクトルを作る
            rbody.velocity = new Vector2(0, 0);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);//瞬間的な力を加える
            goJump = false;
        }

    }

    public void Jump()
    {
        goJump = true;
    }


    //接触開始
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            //Goal();//ゴール！！
            Debug.Log("ゴール");
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver();//ゲームオーバー！！
            Debug.Log("ゲームオーバー");
        }
        else if (collision.gameObject.tag == "FinalGoal")
        {
            //FadeManager.fadeColor = Color.white;
            //dia.SetActive(false);
            //LastFade.Instance.LoadScene("LAST", 1.0f);
        }
    }

    public void GameOver()
    {
        gameState = "gameover";
        GameStop();//ゲーム停止
        // =======================================
        // ゲームオーバー演出
        // =======================================

        //プレイヤー当たり判定を消す
        HideCollider();

        //プレイヤーを上に少し跳ね上げる演出
        rbody.AddForce(new Vector2(0, 800), ForceMode2D.Impulse);
    }
    void GameStop()
    {
        //速度を０にして強制停止
        rbody.velocity = new Vector2(0, 0);
    }


    void HideCollider()
    {
        body.enabled = false;
        up.enabled = false;
        down.enabled = false;
    }
    public void ShowCollider()
    {
        body.enabled = true;
        up.enabled = true;
        down.enabled = true;
    }


}
