using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    //�@����L�����N�^�[
    [SerializeField]
    private PlayerManager playerManager;
    //�@���݋L�����Ă��邩�ǂ���
    private bool isRecord;
    //�@�ۑ�����f�[�^�̍ő吔
    [SerializeField]
    private int maxDataNum = 2000000;
    //�@�L�^�Ԋu
    [SerializeField]
    private float recordDuration = 0.005f;
    //�@�o�ߎ���
    private float elapsedTime = 0f;

    //�@�S�[�X�g�f�[�^
    //private ListData listData;

    //�@�Đ������ǂ���
    private bool isPlayBack;

    //�@�S�[�X�gPrefab
    [SerializeField]
    public GameObject ghostPref;

    public List<GameObject> ghosts;
    public List<Rigidbody2D> rbodys;

    public List<bool> currentRight = new List<bool>();
    public List<bool> currentLeft = new List<bool>();
    public List<List<bool>> rightLists = new List<List<bool>>();
    public List<List<bool>> leftLists = new List<List<bool>>();

    public float xpos = 0.0f;
    public float ypos = 0.0f;

    //�@�S�[�X�g�f�[�^�����n�߂����ԁA�܂��͑O�̃f�[�^
    private float startTime;

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
    }

    //�@�L�����N�^�[�f�[�^�̕ۑ�
    public void StartRecord()
    {
        //�@�ۑ����鎞�̓S�[�X�g�̍Đ����~
        //StopGhost();

        isRecord = true;
        elapsedTime = 0f;
        //List�̏�����
        //listData = new ListData();
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

        currentRight = new List<bool>();
        currentLeft = new List<bool>();

        isRecord = false;
    }

    public void StartGhost()
    {
        /*
        rightLists.Add(currentRight);
        leftLists.Add(currentLeft);

        currentRight = new List<bool>();
        currentLeft = new List<bool>();
        */

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

            //�S�[�X�g�̍Đ��J�n
            for (int i = 0; i < ghosts.Count; i++)
            {
                StartCoroutine(PlayBack(i));
            }
            //StartCoroutine(PlayBackJump());
        }
    }

    //�@�S�[�X�g�̍Đ�
    IEnumerator PlayBack(int ghostNum)
    {
            for (int i = 0; i < rightLists[ghostNum].Count; i++)
            {
                yield return new WaitForSeconds(recordDuration);
                if (rightLists[ghostNum][i] == true)
                {
                    rbodys[ghostNum].velocity = new Vector2(playerManager.speed * 1, rbodys[ghostNum].velocity.y);
                    ghosts[ghostNum].transform.localScale = new Vector2(1, 1);
                }
                
                if (leftLists[ghostNum][i] == true)
                {
                    rbodys[ghostNum].velocity = new Vector2(playerManager.speed * -1, rbodys[ghostNum].velocity.y);
                    ghosts[ghostNum].transform.localScale = new Vector2(-1, 1);
                }
                
                if(rightLists[ghostNum][i] == false && leftLists[ghostNum][i] == false)
                {
                    rbodys[ghostNum].velocity = new Vector2(0, rbodys[ghostNum].velocity.y);
                }

                /*
                    if (listData.leftLists[ghostCount][i])
                    {
                        rbodys[j].velocity = new Vector2(ghostChara.speed * -1, rbodys[j].velocity.y);
                        ghost.transform.localScale = new Vector2(-1, 1);
                    }
                */
                //if (listData.rightLists[ghostCount][i] == false && listData.leftLists[ghostCount][i] == false)
                
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

        StopRecord();
        StartGhost();
        StartRecord();
    }

}
