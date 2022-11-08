using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using DG.Tweening;


public class PlayerManager : MonoBehaviour
{
    private Camera mainCamera = default;
    private Vector3 camPosition;
    
    private Recorder recorder = default;

    private Rigidbody2D rbody;
    private float axisH = 0.0f; //入力の数値
    public float speed = 5.0f; //移動速度

    public float jump = 9.0f; //ジャンプ力
    
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

    public static int failedNum = 0;
    
    
    public enum State
    {
        Playing,
        Gameclear,
        Gameover,
        Sub
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        if (GameObject.Find("Recorder") != null)
        {
            recorder = GameObject.Find("Recorder").GetComponent<Recorder>();
        }

        if (recorder == default)
        {
            gameState = (int)State.Sub;
        }
        
        Init();
        
        //ゲーム開始
        gameState = (int)State.Playing;

        failedNum = PlayerPrefs.GetInt("FAILED", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != (int)State.Playing)
        {
            axisH = 0;
            rbody.velocity = new Vector2(0, 0);
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
        
        
        /*
        /// ----------------------------------------------------------------------
        //スマホ用はUpdate内の入力の取得を消す必要がある
        if (rightMove) axisH = 1;
        if (leftMove) axisH = -1;
        if (!rightMove && !leftMove) axisH = 0;
        if (rightMove && leftMove) axisH = 0;
        /// ----------------------------------------------------------------------
        */
    }

    private void FixedUpdate()
    {
        if (gameState != (int)State.Playing)
        {
            return;
        }else if (gameState == (int)State.Gameclear)
        {
            
        }

        //CheckOnGround();
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
        leftMove = isDown;
    }
    public void JumpButtonDown()
    {
        goJump = true;
    }

    /// ----------------------------------------------------------------------
    ///

    private void Init()
    {
        camPosition = mainCamera.transform.position;
        
        //Rigidbody2Dを取得
        rbody = this.GetComponent<Rigidbody2D>();
        //Anmatorを取得
        animator = GetComponent<Animator>();
        nowAnime = idleAnime;
        oldAnime = idleAnime;
    }
    
    private void Jump()
    {
        if (goJump)
        {
            //地面の上にいるならジャンプ
            if (OnGround())
            {
                Vector2 jumpPw = new Vector2(0, jump);
                rbody.velocity = new Vector2(0, 0);
                rbody.AddForce(jumpPw, ForceMode2D.Impulse);
                
                SoundManager.instance.PlaySE(0);
            }
        }
        goJump = false;
    }
    
    private bool OnGround()
    {
        //Groundの上にいるかチェック
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        Vector3 x = (transform1.right * onGroundNum2);
        Vector3 y = (transform1.up * onGroundNum1);
        //Debug.DrawLine(position - y + x,position - y - x, Color.white);

        return Physics2D.Linecast(position - y + x, position - y - x, groundLayer);
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
        if (OnGround())
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
                Goal();
                //StartCoroutine(nameof(Goal));
                Debug.Log("Goal");
                break;
            case "Dead":
                GameOver(); //�Q�[���I�[�o�[�I�I
                Debug.Log("GameOver");
                break;
            case "FinalGoal":
                //LastFade.Instance.LoadScene("LAST", 1.0f);
                break;
        }
    }

    void  Goal()
    {
        axisH = 0;
        rbody.velocity = new Vector2(0, 0);
        gameState = (int)State.Gameclear;
        Camera.main.transform.DOMoveX(Camera.main.transform.position.x + 22, 0.8f).SetEase(Ease.OutQuad).OnComplete(
            () =>
            {
                SceneManager.LoadScene(nextScene);
            });
    }
    
    private void GameOver()
    {
        
        gameState = (int)State.Gameover;

        SoundManager.instance.PlaySE(0);
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