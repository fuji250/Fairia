using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour 
{
    [SerializeField]
    private GameObject player;
    //　操作キャラクター
    [SerializeField]
    private PlayerManager playerManager;
    

    private float speed;
    private float jump;

    private Recorder recorder;
        
    //　現在記憶しているかどうか
    private bool isRecord = false;
    //　記録間隔
    [SerializeField]
    private float recordDuration = 0.005f;
    //　経過時間
    private float elapsedTime = 0f;

    bool onGround = false;//地面に立っているフラグ

    //　ゴーストPrefab
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

    //ゴースト再生のフラグ
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
        //記録
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
            //ゴーストの再生開始
            for (int i = 0; i < ghosts.Count; i++)
            {
                PlayBack(i);
            }
            ghostFrameNum++;
        }
    }

    //PlayerManagerで呼びだし
    public  void RecordMove(int axisH)
    {
        if (axisH == 1) currentRight.Add(true);
        else currentRight.Add(false);
        
        if (axisH == -1) currentLeft.Add(true);
        else currentLeft.Add(false);
    }
    //PlayerManagerで呼びだし
    public void RecordJump(bool onJump)
    {
        if (onJump)currentJump.Add(true);
        else currentJump.Add(false);
    }

    //　キャラクターデータの保存
    private void StartRecord()
    {
        isRecord = true;
        elapsedTime = 0f;
    }

    //　キャラクターデータの保存の停止
    private void StopRecord()
    {
        //最後に動き続けてしまうのを防止
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

    //ゴーストの作成
    private void StartGhost()
    {
        if (rightLists == null)
        {
        }
        else
        {
            //ゴースト生成
            ghosts.Add(Instantiate(ghostPref, new Vector2(xpos, ypos), Quaternion.identity));
            rbodys.Add(ghosts[ghosts.Count - 1].GetComponent<Rigidbody2D>());

            isPlayback = true;
        }
    }

    //　ゴーストの再生
    private void PlayBack(int ghostNum)
    {
        if (ghostFrameNum < rightLists[ghostNum].Count)
        {
            //右のみ押されていた場合、右移動
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
                //地上判定
                onGround = Physics2D.Linecast(ghosts[ghostNum].transform.position - (ghosts[ghostNum].transform.up * 0.56f) + (ghosts[ghostNum].transform.right * 0.5f), ghosts[ghostNum].transform.position - (ghosts[ghostNum].transform.up * 0.56f) - (ghosts[ghostNum].transform.right * 0.5f), playerManager.groundLayer);
                
                if (onGround)
                {
                    Vector2 jumpPw = new Vector2(0, jump);
                    rbodys[ghostNum].AddForce(jumpPw, ForceMode2D.Impulse);
                }
            }
        }
    }
    
    //RESTARTButtonで呼び出し
    //一度死んでから再スタートの処理
    public void Retry()
    {
        //キャラクタをスタート地点に戻す
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
