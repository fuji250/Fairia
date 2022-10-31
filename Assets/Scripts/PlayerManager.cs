using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;


public class PlayerManager : MonoBehaviour
{
    private Recorder recorder = default;

    private Rigidbody2D rbody; //Rigidbody2D�̕ϐ�
    public float axisH = 0.0f; //����
    public float speed = 5.0f; //�ړ����x

    public float jump = 9.0f; //�W�����v��
    public LayerMask groundLayer; //���n�ł��郌�C���[

    public BoxCollider2D body;
    public BoxCollider2D up;
    public BoxCollider2D down;


    private bool goJump = false; //�W�����v�J�n�t���O
    private bool onGround = false; //�n�ʂɗ����Ă���t���O

    public static int gameState = default; //�Q�[���̏��

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
        
        //Rigidbody2D������Ă���
        rbody = this.GetComponent<Rigidbody2D>();
        gameState = (int)State.playing; //�Q�[�����ɂ���
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState != (int)State.playing)
        {
            return;
        }

        //���������̓��͂��`�F�b�N����
        axisH = Input.GetAxisRaw("Horizontal");


        //Jump�̓��͂��L�^����
        if (Input.GetButtonDown("Jump"))
        {
            //FixedUpdate��Jump������
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

        //�n�㔻�� �C���X�y�N�^���GhostUp���w��
        Transform transform1 = transform;
        Vector3 position = transform1.position;
        onGround = Physics2D.Linecast(position - (transform1.up * 0.57f) + (transform1.right * 0.5f),
            position - (transform1.up * 0.57f) - (transform1.right * 0.5f), groundLayer);

        AdjustmentDirection();

        
        if (recorder != default)
        {
            //Recorder�ɓ��͋L�^�𑗂�
            recorder.RecordMove((int)axisH);
            recorder.RecordJump(goJump);
        }
        
        Jump();

        //���x���X�V����
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
        Debug.Log("Left");
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
        var transformLocalScale = transform.localScale;

        //向きを変更する
        if (axisH > 0.0f)
        {
            //右向き
            transformLocalScale.x = Math.Abs(transformLocalScale.x);
        }
        else if (axisH < 0.0f)
        {
            //左向き
            transformLocalScale.x = Math.Abs(transformLocalScale.x) * -1;
        }
        transform.localScale = transformLocalScale;

    }

    public void Jump()
    {
        if (goJump)
        {
            //地面の上にいるならジャンプ
            if (onGround)
            {
                Vector2 jumpPw = new Vector2(0, jump);
                rbody.velocity = new Vector2(0, 0);
                rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            }
        }
        goJump = false;
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
        gameState = (int)State.gameover;
        GameStop(); //�Q�[����~
        // =======================================
        // �Q�[���I�[�o�[���o
        // =======================================

        //�v���C���[�����蔻�������
        //HideCollider();

        //�v���C���[����ɏ������ˏグ�鉉�o
        //rbody.AddForce(new Vector2(0, 100), ForceMode2D.Impulse);
    }

    private void GameStop()
    {
        //���x���O�ɂ��ċ�����~
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