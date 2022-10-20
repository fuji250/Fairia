using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    //�@����L�����N�^�[
    [SerializeField]
    private PlayerManager playerManager;
    //�@���݋L�����Ă��邩�ǂ���
    private bool isRecord = false;
    //�@�L�^�Ԋu
    [SerializeField]
    private float recordDuration = 0.005f;
    //�@�o�ߎ���
    private float elapsedTime = 0f;

    //�@�Đ������ǂ���
    private bool isPlayBack;

    bool onGround = false;//�n�ʂɗ����Ă���t���O

    //�@�S�[�X�gPrefab
    [SerializeField]
    public GameObject ghostPref;

    public List<GameObject> ghosts;
    public List<Rigidbody2D> rbodys;

    public List<bool> currentRight = new List<bool>();
    public List<bool> currentLeft = new List<bool>();
    public List<float> currentJump = new List<float>();

    public List<List<bool>> rightLists = new List<List<bool>>();
    public List<List<bool>> leftLists = new List<List<bool>>();
    public List<List<float>> jumpLists = new List<List<float>>();

    public float xpos = 0.0f;
    public float ypos = 0.0f;

    //�@�S�[�X�g�f�[�^�����n�߂����ԁA�܂��͑O�̃f�[�^
    private float startTime;

    //�S�[�X�g�Đ��̃t���O
    bool playbackBool = false;
    int ghostFrameNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        StartRecord();
    }

    // Update is called once per frame
    void Update()
    {
        //�L�^
        if (isRecord && PlayerManager.gameState == "playing")
        {
            Rec();
        }
        else if (PlayerManager.gameState == "gameover")
        {
            //StopRecord();
        }
    }

    private void FixedUpdate()
    {
        if (playbackBool)
        {
            //�S�[�X�g�̍Đ��J�n
            for (int i = 0; i < ghosts.Count; i++)
            {
                PlayBack(i);
                StartCoroutine(PlayBackJump(i));
            }
            ghostFrameNum++;
        }

    }

    //Update�ŌJ��Ԃ�
    public void Rec()
    {
        var moveHorizontal = Input.GetAxisRaw("Horizontal");

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= recordDuration)
        {
            if (Input.GetAxisRaw("Horizontal") == 1) currentRight.Add(true);
            else currentRight.Add(false);

            if (Input.GetAxisRaw("Horizontal") == -1) currentLeft.Add(true);
            else currentLeft.Add(false);

            

            elapsedTime = 0f;

            /*
            //�@�f�[�^�ۑ������ő吔�𒴂�����L�^���X�g�b�v
            if (listData.rightLists[ghostCount].Count >= maxDataNum)
            {
                StopRecord();
            }
            */
        }

        //�@�W�����v�͉��������Ԃ�ێ�
            if (Input.GetButtonDown("Jump"))
            {
                Debug.Log("SSSSSSSSSSS");
                currentJump.Add(Time.realtimeSinceStartup - startTime);
                startTime = Time.realtimeSinceStartup;
            }
    }

    //�@�L�����N�^�[�f�[�^�̕ۑ�
    public void StartRecord()
    {
        isRecord = true;
        elapsedTime = 0f;
        startTime = Time.realtimeSinceStartup;
        Debug.Log("StartRecord");
    }

    //�@�L�����N�^�[�f�[�^�̕ۑ��̒�~
    public void StopRecord()
    {
        //�Ō�ɓ��������Ă��܂��̂�h�~
        currentRight.Add(false);
        rightLists.Add(currentRight);
        currentLeft.Add(false);
        leftLists.Add(currentLeft);

        jumpLists.Add(currentJump);

        currentRight = new List<bool>();
        currentLeft = new List<bool>();
        currentJump = new List<float>();

        isRecord = false;
    }

    public void StartGhost()
    {
        if (rightLists == null)
        {
            Debug.Log("�S�[�X�g�f�[�^������܂���");
        }
        else
        {
            isPlayBack = true;

            //�S�[�X�g����
            ghosts.Add(Instantiate(ghostPref, new Vector2(xpos, ypos), Quaternion.identity));
            rbodys.Add(ghosts[ghosts.Count - 1].GetComponent<Rigidbody2D>());

            playbackBool = true;

            
            //StartCoroutine(PlayBackJump());
        }
    }

    //�@�S�[�X�g�̍Đ�
    void PlayBack(int ghostNum)
    {
        if (ghostFrameNum < rightLists[ghostNum].Count)
        {
            if (rightLists[ghostNum][ghostFrameNum] == true)
            {
                rbodys[ghostNum].velocity = new Vector2(playerManager.speed * 1, rbodys[ghostNum].velocity.y);
                ghosts[ghostNum].transform.localScale = new Vector2(1, 1);
            }

            if (leftLists[ghostNum][ghostFrameNum] == true)
            {
                rbodys[ghostNum].velocity = new Vector2(playerManager.speed * -1, rbodys[ghostNum].velocity.y);
                ghosts[ghostNum].transform.localScale = new Vector2(-1, 1);
            }

            if (rightLists[ghostNum][ghostFrameNum] == false && leftLists[ghostNum][ghostFrameNum] == false)
            {
                rbodys[ghostNum].velocity = new Vector2(0, rbodys[ghostNum].velocity.y);
            }
        }
    }

    IEnumerator PlayBackJump(int ghostNum)
    {
        var i = 0;
            //�@�L�����N�^�[�̈ʒu��p�x�̏I����҂��Ă���A�j���[�V�����f�[�^���ŏ��ɖ߂�
            /*
            if (isLoopReset)
            {
                i = 0;
                isLoopReset = false;
            }
            */

            if (i < jumpLists[ghostNum].Count)
            {
                yield return new WaitForSeconds(jumpLists[ghostNum][i]);
                //�n�㔻��
                onGround = Physics2D.Linecast(ghosts[ghostNum].transform.position - (ghosts[ghostNum].transform.up * 0.56f) + (ghosts[ghostNum].transform.right * 0.5f), ghosts[ghostNum].transform.position - (ghosts[ghostNum].transform.up * 0.56f) - (ghosts[ghostNum].transform.right * 0.5f), playerManager.groundLayer);
                
                if (onGround)
                {
                    Vector2 jumpPw = new Vector2(0, playerManager.jump);
                    rbodys[ghostNum].AddForce(jumpPw, ForceMode2D.Impulse);

                    Debug.Log("�W�����v��!!!!!");
                }
                i++;
                //�@����ȊO��null��Ԃ�
            }
            else
            {
                yield return null;
            }
    }

    //RESTARTButton�ŌĂяo��
    //��x����ł���ăX�^�[�g�̏���
    public void roop()
    {
        //�L�����N�^���X�^�[�g�n�_�ɖ߂�
        GameObject.Find("Player").transform.position = new Vector2(xpos, ypos);
        for (int i = 0; i < ghosts.Count; i++)
        {
            ghosts[i].transform.position = new Vector2(xpos, ypos);
        }

        PlayerManager.gameState = "playing";

        ghostFrameNum = 0;
        StopRecord();
        StartGhost();
        StartRecord();
    }

}
