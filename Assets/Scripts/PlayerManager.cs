using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class PlayerManager : MonoBehaviour
{
    
    
    private Recorder recorder = default;

    private Rigidbody2D rbody;
    private float axisH = 0.0f; //入力の数値
    public float speed = 5.0f; //移動速度

    public float jump = 9.0f; //ジャンプ力
    
    private bool onGround = false; //ジャンプ判定に使用するフラグ
    private bool onCharacter = false;
    public LayerMask groundLayer; //ジャンプできる地面のレイヤー
    public LayerMask CharacterLayer; //ジャンプできる地面のレイヤー

    public BoxCollider2D body;
    public BoxCollider2D up;
    public BoxCollider2D down;

    private bool goJump = false; //連続でジャンプしてしまう事を防ぐフラグ
    
    public static int gameState = default;

    public string nextScene = default;

    //アニメーション対応
    Animator animator;
    public string idleAnime = "PlayerIdle";
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    //public string goalAnime = "PlayerGoal";
    //public string deadAnime = "PlayerOver";
    public string nowAnime = "";
    public string oldAnime = "";

    /// ----------------------------------------------------------------------
    private bool rightMove;
    private bool leftMove;
    /// ----------------------------------------------------------------------

    //onGround判定に使用する数値
    private float onGroundNum1 = 0.05f;
    private float onGroundNum2 = 0.45f;
    
    public enum State
    {
        Playing,
        Gameclear,
        Gameover
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Recorder") != null)
        {
            recorder = GameObject.Find("Recorder").GetComponent<Recorder>();
        }
        
        //Rigidbody2Dを取得
        rbody = this.GetComponent<Rigidbody2D>();
        //Anmatorを取得
        animator = GetComponent<Animator>();
        nowAnime = idleAnime;
        oldAnime = idleAnime;
        
        //ゲーム開始
        gameState = (int)State.Playing;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != (int)State.Playing)
        {
            return;
        }

        //入力を取得する
        axisH = Input.GetAxisRaw("Horizontal");

        //Jump入力を記録する
        if (Input.GetButtonDown("Jump"))
        {
            //FixedUpdateで一度だけJumpさせる
            goJump = true;
        }

        //ChangeMass();
        
        
        
        /// ----------------------------------------------------------------------
        /*
        if (rightMove) axisH = 1;
        if (leftMove) axisH = -1;
        if (!rightMove && !leftMove) axisH = 0;
        if (rightMove && leftMove) axisH = 0;
        */
        /// ----------------------------------------------------------------------
    }

    private void FixedUpdate()
    {
        if (gameState != (int)State.Playing)
        {
            return;
        }

        CheckOnGround();
        AdjustmentDirection();
        SendRecord();
        PlaybackAnimation();
        Jump();

        //左右移動の反映
        rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);
    }

    /// ----------------------------------------------------------------------
   
    // MoveButttonのEventTriggerから使用
    public void PushRightButton(bool isDown)
    {
        rightMove = isDown;
    }
    public void PushLeftButton(bool isDown)
    {
        Debug.Log("Left");
        leftMove = isDown;
    }
    public void JumpButtonDown()
    {
        goJump = true;
    }
    /// ----------------------------------------------------------------------
    private void Jump()
    {
        if (goJump)
        {
            //地面の上にいるならジャンプ
            if (onGround)
            {
                //質量戻す
                //rbody.mass = 10;
                
                Vector2 jumpPw = new Vector2(0, jump);
                rbody.velocity = new Vector2(0, 0);
                rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            }
        }
        goJump = false;
    }

    private void ChangeMass()
    {
        if (onCharacter)
        {
            rbody.mass = 0;
        }
        else
        {
            rbody.mass = 10;
        }
    }

    private void CheckOnGround()
    {
        //Groundの上にいるかチェック
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        Vector3 x = (transform1.right * onGroundNum2);
        Vector3 y = (transform1.up * onGroundNum1);
        onGround = Physics2D.Linecast(position - y + x, position - y - x, groundLayer);
        onCharacter = Physics2D.Linecast(position - y + x, position - y - x, CharacterLayer);

        /*
        //上から落ちてきた時の重力を消す
        if (onCharacter)
        {
            var rbodyVelocity = rbody.velocity;
            rbodyVelocity.y = 0;
            rbody.velocity = rbodyVelocity;
        }
        */
        
    }
    
    //向きを変更する
    private void AdjustmentDirection()
    {
        var transformLocalScale = transform.localScale;

        if (axisH > 0.0f) transformLocalScale.x = Math.Abs(transformLocalScale.x);
        else if (axisH < 0.0f) transformLocalScale.x = Math.Abs(transformLocalScale.x) * -1;

        transform.localScale = transformLocalScale;
    }

    private void SendRecord()
    {
        if (recorder != default)
        {
            //Recorderに操作入力情報を送信
            recorder.RecordMove((int)axisH);
            recorder.RecordJump(goJump);
        }
    }

    private void PlaybackAnimation()
    {
        //アニメーションの再生
        if (onGround)
        {
            if(axisH == 0)　nowAnime = idleAnime;
            else　nowAnime = moveAnime;
        }
        else
        {
            nowAnime = jumpAnime;
        }
        if(nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);
        }
    }

    //当たり判定
    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Goal":
                Goal();//�S�[���I�I
                Debug.Log("�S�[��");
                break;
            case "Dead":
                GameOver(); //�Q�[���I�[�o�[�I�I
                Debug.Log("�Q�[���I�[�o�[");
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
        //gameState = (int)State.gameclear;
        //SceneManager.LoadScene(nextScene);
    }
    
    private void GameOver()
    {
        rbody.velocity = new Vector2(0, 0);
        gameState = (int)State.Gameover;
    }

    //一時停止
    public void Pause()
    {
        Time.timeScale = 0;
    }
    
    //再開
    public void Restart()
    {
        Time.timeScale = 1;
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