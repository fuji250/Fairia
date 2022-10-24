using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour 
{
    [SerializeField]
    private GameObject player;
    //�@����L�����N�^�[
    [SerializeField]
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

    bool onGround = false;//�n�ʂɗ����Ă���t���O

    //�@�S�[�X�gPrefab
    [SerializeField]
    public GameObject ghostPref;

    public List<GameObject> ghosts;
    public List<Rigidbody2D> rbodys;

    public List<bool> currentRight = new List<bool>();
    public List<bool> currentLeft = new List<bool>();
    public List<bool> currentJump = new List<bool>();

    private readonly List<List<bool>> rightLists = new List<List<bool>>();
    private readonly List<List<bool>> leftLists = new List<List<bool>>();
    private readonly List<List<bool>> jumpLists = new List<List<bool>>();

    public float xpos = 0.0f;
    public float ypos = 0.0f;

    //�S�[�X�g�Đ��̃t���O
    private bool isPlayback = false;
    private int ghostFrameNum = 0;

    private int previousGameState;

    // Start is called before the first frame update
    void Start()
    {
        StartRecord();
        speed = playerManager.speed;
        jump = playerManager.jump;
    }

    // Update is called once per frame
    void Update()
    {
        //�L�^
        if (isRecord && PlayerManager.gameState == (int)PlayerManager.State.playing)
        {
        }
        else if (previousGameState !=  PlayerManager.gameState && PlayerManager.gameState == (int)PlayerManager.State.gameover)
        {
            Invoke(nameof(Retry), 0.3f);
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
            ghosts.Add(Instantiate(ghostPref, new Vector2(xpos, ypos), Quaternion.identity));
            rbodys.Add(ghosts[ghosts.Count - 1].GetComponent<Rigidbody2D>());

            isPlayback = true;
        }
    }

    //�@�S�[�X�g�̍Đ�
    private void PlayBack(int ghostNum)
    {
        if (ghostFrameNum < rightLists[ghostNum].Count)
        {
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
            }else
            {
                rbodys[ghostNum].velocity = new Vector2(0, rbodys[ghostNum].velocity.y);
            }
            
            if (jumpLists[ghostNum][ghostFrameNum] == true)
            {
                //�n�㔻��
                onGround = Physics2D.Linecast(ghosts[ghostNum].transform.position - (ghosts[ghostNum].transform.up * 0.56f) + (ghosts[ghostNum].transform.right * 0.5f), ghosts[ghostNum].transform.position - (ghosts[ghostNum].transform.up * 0.56f) - (ghosts[ghostNum].transform.right * 0.5f), playerManager.groundLayer);
                
                if (onGround)
                {
                    Vector2 jumpPw = new Vector2(0, jump);
                    rbodys[ghostNum].AddForce(jumpPw, ForceMode2D.Impulse);
                }
            }
        }
    }
    
    //RESTARTButton�ŌĂяo��
    //��x����ł���ăX�^�[�g�̏���
    public void Retry()
    {
        //�L�����N�^���X�^�[�g�n�_�ɖ߂�
        player.transform.position = new Vector2(xpos, ypos);
        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].transform.position = new Vector2(xpos, ypos);
        }

        PlayerManager.gameState = (int)PlayerManager.State.playing;

        ghostFrameNum = 0;
        StopRecord();
        StartGhost();
        StartRecord();
    }

}
