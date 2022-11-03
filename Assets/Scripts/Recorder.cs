using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour 
{
    private GameObject player;
    private PlayerManager playerManager;
    private float speed;
    private float jump;

    private Recorder recorder;
        
    //�@���݋L�����Ă��邩�ǂ���
    private bool isRecord = false;
    //�@�L�^�Ԋu
    [SerializeField]
    private float recordDuration = 0.005f;
    //�@�o�ߎ���
    private float elapsedTime = 0f;

    private bool onGround = false;//�n�ʂɗ����Ă���t���O
    private LayerMask groundLayer;//�W�����v�ł���n�ʂ̃��C���[

    //�@�S�[�X�gPrefab
    [SerializeField]
    public GameObject ghostPref;

    public List<GameObject> ghosts;
    public List<Rigidbody2D> rbodys = default;

    public List<bool> currentRight = new List<bool>();
    public List<bool> currentLeft = new List<bool>();
    public List<bool> currentJump = new List<bool>();

    private readonly List<List<bool>> rightLists = new List<List<bool>>();
    private readonly List<List<bool>> leftLists = new List<List<bool>>();
    private readonly List<List<bool>> jumpLists = new List<List<bool>>();

    //�X�^�[�g�n�_
    private float xPos = 0.0f;
    private float yPos = 0.0f;

    //�S�[�X�g�Đ��̃t���O
    private bool isPlayback = false;
    private int ghostFrameNum = 0;

    private int previousGameState;
    
    //�A�j���[�V�����Ή�
    public List<Animator> animators = default;
    private const string idleAnime = "PlayerIdle";
    private const string moveAnime = "PlayerMove";
    private const string jumpAnime = "PlayerJump";

    //public string goalAnime = "PlayerGoal";
    //public string deadAnime = "PlayerOver";
    public List<string> nowAnimes = default;
    public List<string>  oldAnimes = default;

    //onGround����Ɏg�p���鐔�l
    private const float onGroundNum1 = 0.05f;
    private const float onGroundNum2 = 0.45f;

    // Start is called before the first frame update
    void Start()
    {
        GetPlayerInformation();
        StartRecord();

        groundLayer = playerManager.groundLayer;
    }

    // Update is called once per frame
    void Update()
    {
        //�L�^
        if (isRecord && PlayerManager.gameState == (int)PlayerManager.State.Playing)
        {
        }
        else if (previousGameState !=  PlayerManager.gameState && PlayerManager.gameState == (int)PlayerManager.State.Gameover)
        {
            Invoke(nameof(Retry), 1f);
        }

        previousGameState = PlayerManager.gameState;
    }

    private void FixedUpdate()
    {
        if (isPlayback)
        {
            //�S�[�X�g�̍Đ��J�n
            for (int i = 0; i < ghosts.Count; i++)
            {
                PlayBack(i);
            }
            ghostFrameNum++;
        }
    }

    //Player�̏��擾
    private void GetPlayerInformation()
    {
        player = GameObject.Find("Player");
        playerManager = player.GetComponent<PlayerManager>();
        
        speed = playerManager.speed;
        jump = playerManager.jump;

        var position = player.transform.position;
        xPos = position.x;
        yPos = position.y;
    }

    //PlayerManager�ŌĂт���
    public  void RecordMove(int axisH)
    {
        if (axisH == 1) currentRight.Add(true);
        else currentRight.Add(false);
        
        if (axisH == -1) currentLeft.Add(true);
        else currentLeft.Add(false);
    }
    //PlayerManager�ŌĂт���
    public void RecordJump(bool onJump)
    {
        if (onJump)currentJump.Add(true);
        else currentJump.Add(false);
    }

    //�@�L�����N�^�[�f�[�^�̕ۑ�
    private void StartRecord()
    {
        isRecord = true;
        elapsedTime = 0f;
    }

    //�@�L�����N�^�[�f�[�^�̕ۑ��̒�~
    private void StopRecord()
    {
        //�Ō�ɓ��������Ă��܂��̂�h�~
        currentRight.Add(false);
        rightLists.Add(currentRight);
        currentLeft.Add(false);
        leftLists.Add(currentLeft);
        currentJump.Add(false);
        jumpLists.Add(currentJump);

        //���̃S�[�X�g�p�̃��X�g�ǉ�
        currentRight = new List<bool>();
        currentLeft = new List<bool>();
        currentJump = new List<bool>();

        isRecord = false;
    }

    //�S�[�X�g�̍쐬
    private void StartGhost()
    {
        if (rightLists == null)
        {
        }
        else
        {
            //�S�[�X�g����
            ghosts.Add(Instantiate(ghostPref, new Vector2(xPos, yPos), Quaternion.identity));
            //�S�[�X�g�̐���錾
            int i = ghosts.Count - 1;
            
            rbodys.Add(ghosts[i].GetComponent<Rigidbody2D>());
            
            //Animator������Ă���
            animators.Add(ghosts[i].GetComponent<Animator>());
            nowAnimes.Add(idleAnime);
            oldAnimes.Add(idleAnime);

            isPlayback = true;
        }
    }

    //�@�S�[�X�g�̍Đ�
    private void PlayBack(int ghostNum)
    {
        //�����܂����͋L�^���c���Ă�����
        if (ghostFrameNum < rightLists[ghostNum].Count)
        {
            OnGround(ghostNum);
            
            //�A�j���[�V�����̍Đ�
            if (onGround) nowAnimes[ghostNum] = moveAnime;
            else�@nowAnimes[ghostNum] = jumpAnime;
            
            
            //�E�̂݉�����Ă����ꍇ�A�E�ړ�
            if (rightLists[ghostNum][ghostFrameNum] == true && leftLists[ghostNum][ghostFrameNum] == false)
            {
                rbodys[ghostNum].velocity = new Vector2(speed * 1, rbodys[ghostNum].velocity.y);
                ghosts[ghostNum].transform.localScale = new Vector2(1, 1);
                
                
            }
            else if (leftLists[ghostNum][ghostFrameNum] == true)
            {
                rbodys[ghostNum].velocity = new Vector2(speed * -1, rbodys[ghostNum].velocity.y);
                ghosts[ghostNum].transform.localScale = new Vector2(-1, 1);
            }
            else
            {
                rbodys[ghostNum].velocity = new Vector2(0, rbodys[ghostNum].velocity.y);
                
                if (onGround) nowAnimes[ghostNum] = idleAnime;
            }
            
            if (jumpLists[ghostNum][ghostFrameNum] == true)
            {
                if (onGround)
                {
                    //Jump!
                    Vector2 jumpPw = new Vector2(0, jump);
                    rbodys[ghostNum].AddForce(jumpPw, ForceMode2D.Impulse);
                }
            }
            
            //��Ԃ��ς�������̂݃A�j���Đ�������
            if(nowAnimes[ghostNum] != oldAnimes[ghostNum])
            {
                oldAnimes[ghostNum] = nowAnimes[ghostNum];
                animators[ghostNum].Play(nowAnimes[ghostNum]);
            }
        }
    }
    
    //RESTARTButton�ŌĂяo��
    //��x����ł���ăX�^�[�g�̏���
    public void Retry()
    {
        //�L�����N�^���X�^�[�g�n�_�ɖ߂�
        player.transform.position = new Vector2(xPos, yPos);
        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].transform.position = new Vector2(xPos, yPos);
        }

        PlayerManager.gameState = (int)PlayerManager.State.Playing;

        ghostFrameNum = 0;
        StopRecord();
        StartGhost();
        StartRecord();
    }

    private void OnGround(int ghostNum) 
    {
        //Ground�̏�ɂ��邩�`�F�b�N
        Transform transform1 = ghosts[ghostNum].transform;
        Vector3 position = transform1.position;
        Vector3 x = (transform1.right * onGroundNum2);
        Vector3 y = (transform1.up * onGroundNum1);
                
        //�n�㔻��
        onGround = Physics2D.Linecast(position - y + x, position - y - x, groundLayer);
    }
}
