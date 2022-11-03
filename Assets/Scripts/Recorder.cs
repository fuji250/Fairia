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

    public List<bool> currentRight = new List<bool>();
    public List<bool> currentLeft = new List<bool>();
    public List<bool> currentJump = new List<bool>();

    private readonly List<List<bool>> rightLists = new List<List<bool>>();
    private readonly List<List<bool>> leftLists = new List<List<bool>>();
    private readonly List<List<bool>> jumpLists = new List<List<bool>>();

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

    //public string goalAnime = "PlayerGoal";
    //public string deadAnime = "PlayerOver";
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

        //次のゴースト用のリスト追加
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
            ghosts.Add(Instantiate(ghostPref, new Vector2(xPos, yPos), Quaternion.identity));
            //ゴーストの数を宣言
            int i = ghosts.Count - 1;
            
            rbodys.Add(ghosts[i].GetComponent<Rigidbody2D>());
            
            //Animatorを取ってくる
            animators.Add(ghosts[i].GetComponent<Animator>());
            nowAnimes.Add(idleAnime);
            oldAnimes.Add(idleAnime);

            isPlayback = true;
        }
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
}
