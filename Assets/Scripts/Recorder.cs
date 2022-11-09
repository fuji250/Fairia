using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class Recorder : MonoBehaviour 
{
    private GameObject player;
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

    private bool onGround = false;//地面に立っているフラグ
    private LayerMask groundLayer;//ジャンプできる地面のレイヤー

    //　ゴーストPrefab
    [SerializeField]
    public GameObject ghostPref;

    public List<GameObject> ghosts;
    public List<Rigidbody2D> rbodys = default;

    public List<int> currentRight = new List<int>();
    public List<int> currentJump = new List<int>();

    private readonly List<List<int>> rightLists = new List<List<int>>();
    private readonly List<List<int>> jumpLists = new List<List<int>>();

    //スタート地点
    private float xPos = 0.0f;
    private float yPos = 0.0f;

    //ゴースト再生のフラグ
    private bool isPlayback = false;
    private int ghostFrameNum = 0;

    private int previousGameState;
    
    //アニメーション対応
    public List<Animator> animators = default;
    private const string idleAnime = "PlayerIdle";
    private const string moveAnime = "PlayerMove";
    private const string jumpAnime = "PlayerJump";
    
    public List<string> nowAnimes = default;
    public List<string>  oldAnimes = default;

    //onGround判定に使用する数値
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
        //記録
        if (isRecord && PlayerManager.gameState == (int)PlayerManager.State.Playing)
        {
            /*
            if (SceneManager.GetActiveScene().name == "Stage3")
            {
                isRecord = false;
                ChangeMoveData();
            }
            */
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
            //ゴーストの再生開始
            for (int i = 0; i < ghosts.Count; i++)
            {
                PlayBack(i);
            }
            ghostFrameNum++;
        }
    }

    //Playerの情報取得
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

    //PlayerManagerで呼びだし
    public  void RecordMove(int axisH)
    {

        //2右、1左、0止
        if (axisH == 1)currentRight.Add(2);
        else if (axisH == -1)currentRight.Add(1);
        else currentRight.Add(0);
    }
    
    //PlayerManagerで呼びだし
    public void RecordJump(bool onJump)
    {
        if (onJump)currentJump.Add(1);
        else currentJump.Add(0);
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
        currentRight.Add(0);
        rightLists.Add(currentRight);
        currentJump.Add(0);
        jumpLists.Add(currentJump);
        
        //ゴーストの移動保存
        SaveData.FailedData failedData = SaveData.LoadPlayerData();
        failedData.rightLists.Add(String.Join("", this.currentRight));
        failedData.jumpLists.Add(String.Join("", this.currentJump));
        SaveData.SavePlayerData(failedData);

        //次のゴースト用のリスト追加
        currentRight = new List<int>();
        currentJump = new List<int>();

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
            MakeGhost();
            isPlayback = true;
        }
    }

    private void MakeGhost()
    {
        //ゴースト生成
        ghosts.Add(Instantiate(ghostPref, new Vector2(xPos, yPos), Quaternion.identity));
        //ゴーストの数を宣言
        int i = ghosts.Count - 1;
            
        rbodys.Add(ghosts[i].GetComponent<Rigidbody2D>());
            
        //Animatorを取ってくる
        animators.Add(ghosts[i].GetComponent<Animator>());
        nowAnimes.Add(idleAnime);
        oldAnimes.Add(idleAnime);
    }

    //　ゴーストの再生
    private void PlayBack(int ghostNum)
    {
        //もしまだ入力記録が残っていたら
        if (ghostFrameNum < rightLists[ghostNum].Count)
        {
            OnGround(ghostNum);
            
            //アニメーションの再生
            if (onGround) nowAnimes[ghostNum] = moveAnime;
            else　nowAnimes[ghostNum] = jumpAnime;
            
            //2右、1左、0止
            int moveDir = rightLists[ghostNum][ghostFrameNum];
            if (moveDir%48 == 2)
            {
                rbodys[ghostNum].velocity = new Vector2(speed * 1, rbodys[ghostNum].velocity.y);
                ghosts[ghostNum].transform.localScale = new Vector2(1, 1);
            }else if (moveDir % 48 == 1)
            {
                rbodys[ghostNum].velocity = new Vector2(speed * -1, rbodys[ghostNum].velocity.y);
                ghosts[ghostNum].transform.localScale = new Vector2(-1, 1);
            }else
            {
                rbodys[ghostNum].velocity = new Vector2(0, rbodys[ghostNum].velocity.y);
                if (onGround) nowAnimes[ghostNum] = idleAnime;
            }
            
            if (jumpLists[ghostNum][ghostFrameNum]%2 == 1)
            {
                if (onGround)
                {
                    //Jump!
                    Vector2 jumpPw = new Vector2(0, jump);
                    rbodys[ghostNum].AddForce(jumpPw, ForceMode2D.Impulse);
                }
            }
            
            //状態が変わった時のみアニメ再生させる
            if(nowAnimes[ghostNum] != oldAnimes[ghostNum])
            {
                oldAnimes[ghostNum] = nowAnimes[ghostNum];
                animators[ghostNum].Play(nowAnimes[ghostNum]);
            }
        }
    }
    
    //RESTARTButtonで呼び出し
    //一度死んでから再スタートの処理
    public void Retry()
    {
        //キャラクタをスタート地点に戻す
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
        //Groundの上にいるかチェック
        Transform transform1 = ghosts[ghostNum].transform;
        Vector3 position = transform1.position;
        Vector3 x = (transform1.right * onGroundNum2);
        Vector3 y = (transform1.up * onGroundNum1);
                
        //地上判定
        onGround = Physics2D.Linecast(position - y + x, position - y - x, groundLayer);
    }

    public void ChangeMoveData()
    {
        //PlayerManager.gameState = (int)PlayerManager.State.Sub;

        
        isPlayback = false;
        isRecord = false;
        
        ghostFrameNum = 0;
        
        ghosts.Clear();
        rbodys.Clear();
        animators.Clear();
        
        SaveData.FailedData failedData = SaveData.LoadPlayerData();
        //ゴーストの数分
        for (int i = 0; i < failedData.rightLists.Count; i++)
        {
            //初期化
            this.currentRight = new List<int>();
            this.currentJump = new List<int>();
            
            //数字の文字列をリストに収納(一人分)
            for (int j = 0; j < failedData.rightLists[i].Length; j++)
            {
                int x = failedData.rightLists[i][j];
                this.currentRight.Add(failedData.rightLists[i][j]);
                this.currentJump.Add(failedData.jumpLists[i][j]);
                
                Debug.Log(failedData.rightLists[i][j]);
            }

            this.rightLists.Add(currentRight);
            this.jumpLists.Add(currentJump);
            
            MakeGhost();

        }
        
        
        isPlayback = true;
        
        //IEnumerable<("0022",2)>("asv");
    }
    
    static IEnumerable<(char, int)> RLE(string s)
    {
        var count = 1;
        var prev = s[0];
        for (int i = 1; i < s.Length; i++)
        {
            if (prev == s[i])
            {
                count++;
            }
            else
            {
                yield return (prev, count);
                count = 1;
            }
            prev = s[i];
        }
        yield return (prev, count);
    }
}
