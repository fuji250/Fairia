using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
    //　操作キャラクター
    [SerializeField]
    private PlayerManager playerManager;
    //　現在記憶しているかどうか
    private bool isRecord;
    //　保存するデータの最大数
    [SerializeField]
    private int maxDataNum = 2000000;
    //　記録間隔
    [SerializeField]
    private float recordDuration = 0.005f;
    //　経過時間
    private float elapsedTime = 0f;

    //　ゴーストデータ
    //private ListData listData;

    //　再生中かどうか
    private bool isPlayBack;

    //　ゴーストPrefab
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

    //　ゴーストデータを取り始めた時間、または前のデータ
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        StartRecord();

    }

    // Update is called once per frame
    void Update()
    {
        //記録
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

    //Updateで繰り返す
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
            //　データ保存数が最大数を超えたら記録をストップ
            if (listData.rightLists[ghostCount].Count >= maxDataNum)
            {
                StopRecord();
            }
            */
        }
    }

    //　キャラクターデータの保存
    public void StartRecord()
    {
        //　保存する時はゴーストの再生を停止
        //StopGhost();

        isRecord = true;
        elapsedTime = 0f;
        //Listの初期化
        //listData = new ListData();
        startTime = Time.realtimeSinceStartup;
        Debug.Log("StartRecord");
    }

    //　キャラクターデータの保存の停止
    public void StopRecord()
    {
        //最後に動き続けてしまうのを防止
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
            Debug.Log("ゴーストデータがありません");
        }
        else
        {
            isPlayBack = true;

            //ゴースト生成
            ghosts.Add(Instantiate(ghostPref, new Vector2(xpos, ypos), Quaternion.identity));
            rbodys.Add(ghosts[ghosts.Count - 1].GetComponent<Rigidbody2D>());

            //ゴーストの再生開始
            for (int i = 0; i < ghosts.Count; i++)
            {
                StartCoroutine(PlayBack(i));
            }
            //StartCoroutine(PlayBackJump());
        }
    }

    //　ゴーストの再生
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

    //RESTARTButtonで呼び出し
    //一度死んでから再スタートの処理
    public void roop()
    {
        //キャラクタをスタート地点に戻す
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
