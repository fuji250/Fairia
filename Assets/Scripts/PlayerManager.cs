using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerManager : MonoBehaviour
{
    private Recorder recorder = default;

    public Rigidbody2D rbody; //Rigidbody2Dの変数
    public float axisH = 0.0f; //入力
    public float speed = 5.0f; //移動速度

    public float jump = 9.0f; //ジャンプ力
    public LayerMask groundLayer; //着地できるレイヤー

    public BoxCollider2D body;
    public BoxCollider2D up;
    public BoxCollider2D down;


    private bool goJump = false; //ジャンプ開始フラグ
    private bool onGround = false; //地面に立っているフラグ

    public static int gameState = default; //ゲームの状態

    public string nextScene = default;


    /// ----------------------------------------------------------------------
    private bool rightMove;
    private bool leftMove;
    /// ----------------------------------------------------------------------

    public enum State
    {
        playing,
        gameclear,
        gameover
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Recorder") != null)
        {
            recorder = GameObject.Find("Recorder").GetComponent<Recorder>();
        }
        
        //Rigidbody2Dを取ってくる
        rbody = this.GetComponent<Rigidbody2D>();
        gameState = (int)State.playing; //ゲーム中にする
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != (int)State.playing)
        {
            return;
        }

        //水平方向の入力をチェックする
        axisH = Input.GetAxisRaw("Horizontal");

        AdjustmentDirection();

        //Jumpの入力を記録する
        if (Input.GetButtonDown("Jump"))
        {
            //FixedUpdateでJumpさせる
            goJump = true;
        }
        
        /// ----------------------------------------------------------------------
        /*
        if (rightMove)
        {
            axisH = 1;
        }
        if (leftMove)
        {
            axisH = -1;
        }
        if (!rightMove && !leftMove)
        {
            axisH = 0;
        }
        if (rightMove && leftMove)
        {
            axisH = 0;
        }
        */
        /// ----------------------------------------------------------------------
    }

    private void FixedUpdate()
    {
        if (gameState != (int)State.playing)
        {
            return;
        }

        //地上判定 インスペクタ上でGhostUpも指定
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        onGround = Physics2D.Linecast(position - (transform1.up * 0.57f) + (transform1.right * 0.5f),
            position - (transform1.up * 0.57f) - (transform1.right * 0.5f), groundLayer);

        if (recorder != default)
        {
            //Recorderに入力記録を送る
            recorder.RecordMove((int)axisH);
            recorder.RecordJump(goJump);
        }
        
        Jump();

        //速度を更新する
        rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);
    }

    /// ----------------------------------------------------------------------
    public void RightButtonDown()
    {
        rightMove = true;
    }
    public void RightButtonUp()
    {
        rightMove = false;
    }

    public void LeftButtonDown()
    {
        leftMove = true;
    }
    public void LeftButtonUp()
    {
        leftMove = false;
    }

    public void JumpButtonDown()
    {
        goJump = true;
    }
    /// ----------------------------------------------------------------------

    void AdjustmentDirection()
    {
        //向きの調整
        if (axisH > 0.0f)
        {
            //右向き
            transform.localScale = new Vector2(1, 1);
        }
        else if (axisH < 0.0f)
        {
            //左向き
            transform.localScale = new Vector2(-1, 1); //左右反転させる
        }
    }

    public void Jump()
    {
        if (goJump)
        {
            //地面の上だとジャンプする
            if (onGround)
            {
                Vector2 jumpPw = new Vector2(0, jump); //ジャンプさせるベクトルを作る
                rbody.velocity = new Vector2(0, 0);
                rbody.AddForce(jumpPw, ForceMode2D.Impulse); //瞬間的な力を加える
            }
        }
        goJump = false;
    }

    //接触開始
    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Goal":
                Goal();//ゴール！！
                Debug.Log("ゴール");
                break;
            case "Dead":
                GameOver(); //ゲームオーバー！！
                Debug.Log("ゲームオーバー");
                break;
            case "FinalGoal":
                //FadeManager.fadeColor = Color.white;
                //dia.SetActive(false);
                //LastFade.Instance.LoadScene("LAST", 1.0f);
                break;
        }
    }

    private void Goal()
    {
        gameState = (int)State.gameclear;
        //SceneManager.LoadScene(nextScene);
    }
    

    private void GameOver()
    {
        gameState = (int)State.gameover;
        GameStop(); //ゲーム停止
        // =======================================
        // ゲームオーバー演出
        // =======================================

        //プレイヤー当たり判定を消す
        //HideCollider();

        //プレイヤーを上に少し跳ね上げる演出
        //rbody.AddForce(new Vector2(0, 100), ForceMode2D.Impulse);
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