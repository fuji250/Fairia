using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Rigidbody2D rbody;//Rigidbody2D�̕ϐ�
    public float axisH = 0.0f;//����
    public float speed = 5.0f;//�ړ����x

    public float jump = 9.0f;//�W�����v��
    public LayerMask groundLayer;//���n�ł��郌�C���[

    public BoxCollider2D body;
    public BoxCollider2D up;
    public BoxCollider2D down;


    bool goJump = false;//�W�����v�J�n�t���O
    bool onGround = false;//�n�ʂɗ����Ă���t���O

    public static string gameState = "playing";//�Q�[���̏��


    // Start is called before the first frame update
    void Start()
    {
        //Rigidbody2D������Ă���
        rbody = this.GetComponent<Rigidbody2D>();
        gameState = "playing";//�Q�[�����ɂ���
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState != "playing")
        {
            return;
        }

            //���������̓��͂��`�F�b�N����
            axisH = Input.GetAxisRaw("Horizontal");
        
        //�����̒���
        if (axisH > 0.0f)
        {
            //�E����
            transform.localScale = new Vector2(1, 1);
        }
        else if(axisH < 0.0f)
        {
            //������
            transform.localScale = new Vector2(-1, 1);//���E���]������
        }
        //�L�����N�^�[���W�����v������
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

        //�n�㔻�� �C���X�y�N�^���GhostUp���w��
        onGround = Physics2D.Linecast(transform.position - (transform.up * 0.57f) + (transform.right * 0.5f), transform.position - (transform.up * 0.57f) - (transform.right * 0.5f), groundLayer);
        

        //���x���X�V����
        rbody.velocity = new Vector2(axisH * speed, rbody.velocity.y);

        if (goJump)
        {
            Vector2 jumpPw = new Vector2(0, jump);//�W�����v������x�N�g�������
            rbody.velocity = new Vector2(0, 0);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);//�u�ԓI�ȗ͂�������
            goJump = false;
        }

    }

    public void Jump()
    {
        goJump = true;
    }


    //�ڐG�J�n
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            //Goal();//�S�[���I�I
            Debug.Log("�S�[��");
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver();//�Q�[���I�[�o�[�I�I
            Debug.Log("�Q�[���I�[�o�[");
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
        GameStop();//�Q�[����~
        // =======================================
        // �Q�[���I�[�o�[���o
        // =======================================

        //�v���C���[�����蔻�������
        HideCollider();

        //�v���C���[����ɏ������ˏグ�鉉�o
        rbody.AddForce(new Vector2(0, 100), ForceMode2D.Impulse);
    }
    void GameStop()
    {
        //���x���O�ɂ��ċ�����~
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