using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    //ワールド名
    public enum WorldName {
        LABORATORY,
        NATURE,
        ABANDONED_FACTORY,
        CITY
    }

    private GameObject[] stageIcons;
    //アイコンの数
    public int stageIconNum = 16;
    private LineRenderer lr;
    //
    //始点の色(線)
    public Color firstColor = Color.white;
    //終点の色(線)
    public Color endColor = Color.white;

    //プレイヤースクリプト
    private PlayerController_Map playerScript;
    //マスクスクリプト
    public MaskManager maskManager;
    //アルファ値を変えるスクリプト
    public ChangeAlpha changeAlpha;

    //プレイヤーアニメーション
    private Animator playerAnim;

    public Text stageName_text;
    public Text stageName2_text;

    //public static bool isClear = false;

    private Vector3 appearIconPos;
    private bool isAppear = false;
    public float appearSpeed = 0.1f;

    //移動可能アイコン番号
    public static int lastGoNo = 3;

    //挑戦中のステージのアイコン番号
    public static int tryNo = 0;

    private AudioSource audioSource;
    public AudioClip[] audioClips;

    public enum ScreenStatuses
    {
        NORMAL,
        CLEAR,
        SELECT,
        DARK,
        TEXT_FADE_IN,
        TEXT_FADE_OUT
    }
    public static ScreenStatuses screenStatus = ScreenStatuses.NORMAL;

    private void Awake()
    {
        stageIcons = new GameObject[stageIconNum];
        for(int i = 0; i < stageIconNum; i++)
        {
            string stageIconName = "StageIcon" + i;
            stageIcons[i] = GameObject.Find(stageIconName);
            if (i <= lastGoNo)
            {
                stageIcons[i].SetActive(true);
            }
            else
            {
                stageIcons[i].SetActive(false);
            }
        }

        //DontDestroyOnLoad(this);
        lr = GetComponent<LineRenderer>();
        //Vector3[] points = new Vector3[stageIcons.Length];
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController_Map>();
        stageName_text = GameObject.Find("StageName").GetComponent<Text>();
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();

        lr.startColor = firstColor;
        lr.endColor = endColor;
        SetPointsToLine();

        //テキストUIにステージ名を表示(更新)
        string StageName = stageIcons[tryNo].GetComponent<StageIcon>().GetStageName();
        stageName_text.text = StageName;

        if (screenStatus == ScreenStatuses.CLEAR)
        {
            Clear();
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //アイコンの上かつプレイヤーのキーが押せる状態だったら
        if (screenStatus == ScreenStatuses.NORMAL)
        {
            //テキストUIにステージ名を表示(更新)
            string StageName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetStageName();
            stageName_text.text = StageName;

            //スペースキーが押されたら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StageIcon stageIcon = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>();
                //挑戦するステージアイコン記憶
                tryNo = playerScript.GetGoNo();
                //プレイヤージャンプ
                playerScript.Jump();
                //音再生
                audioSource.PlayOneShot(audioClips[0]);
                //状態を選択状態にする
                screenStatus = ScreenStatuses.SELECT;
            }
        }
        else if (screenStatus == ScreenStatuses.CLEAR)
        {
            //線の最後の位置取得
            Vector3 lastLinePos = lr.GetPosition(lastGoNo);
            //少し目標に近づける
            Vector3 deltaVec = appearIconPos - lastLinePos;
            //もし次ではみ出そうなら
            if(deltaVec.magnitude <= appearSpeed * Time.deltaTime)
            {
                //調整
                lastLinePos = appearIconPos;
                //終わり
                screenStatus = ScreenStatuses.NORMAL;
                //アイコンを見えるようにする
                stageIcons[lastGoNo].SetActive(true);
                SetPointsToLine();
                ////動けるようにする
                //playerScript.SetCanPush(true);
                return;
            }
            lastLinePos += (deltaVec.normalized / deltaVec.magnitude) * appearSpeed * Time.deltaTime;
            //線の最後の位置更新
            lr.SetPosition(lastGoNo, lastLinePos);
        }
        else if(screenStatus == ScreenStatuses.SELECT)
        {
            if (!playerScript.Get_isJump())
            {
                //円が縮むのを開始
                maskManager.Set_isShrink(true);
                screenStatus = ScreenStatuses.DARK;
            }
        }
        else if(screenStatus == ScreenStatuses.DARK)
        {
            if (!maskManager.Get_isShrink())
            {
                //もじが浮かび上がる
                string stageName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetStageName();
                stageName2_text.text = stageName;
                changeAlpha.Restart(true);
                screenStatus = ScreenStatuses.TEXT_FADE_IN;
            }
        }
        else if(screenStatus == ScreenStatuses.TEXT_FADE_IN)
        {
            if (changeAlpha.Get_isFin())
            {
                changeAlpha.Restart(false);
                screenStatus = ScreenStatuses.TEXT_FADE_OUT;
            }
        }
        else if(screenStatus == ScreenStatuses.TEXT_FADE_OUT)
        {
            if (changeAlpha.Get_isFin())
            {
                //テキストUIにステージ名を表示(更新)
                string sceneName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetSceneName();
                SceneManager.LoadScene(sceneName);
            }
        }
    }

    public void printStageInfo()
    {
        string stageName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetStageName();

    }

    public void LoadScene()
    {
        //シーン読み込み
        string sceneName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetSceneName();
        SceneManager.LoadScene(sceneName);
    }

    //今いける位置をLineRendererにセット
    private void SetPointsToLine()
    {
        List<Vector3> clearedPoints = GetClearedPoints();
        //点の数を設定
        lr.positionCount = clearedPoints.Count;
        //クリア済みのみ線を引く
        lr.SetPositions(clearedPoints.ToArray());
        //プレイヤースクリプトに記憶
        playerScript.SetClearedPoints(clearedPoints);
    }

    //今いけるステージのアイコンの位置を取得
    public List<Vector3> GetClearedPoints()
    {
        List<Vector3> clearedPoints = new List<Vector3>();
        for (int i = 0; i <= lastGoNo; i++)
        {
            //追加
            clearedPoints.Add(stageIcons[i].GetComponent<StageIcon>().GetPosision());
        }
        return clearedPoints;
    }

    //sceneNameのステージをクリアしたときの処理
    public void Clear()
    {
        GameObject clearStageIcon = stageIcons[tryNo];
        //もしクリアしたアイコンが既にクリア済みならreturn
        if (tryNo != lastGoNo || stageIcons.Length == lastGoNo + 1)
        {
            Debug.Log("クリア済み");
            screenStatus = ScreenStatuses.NORMAL;
            return;
        }
        StageIcon stageIconScript = clearStageIcon.GetComponent<StageIcon>();
        //アイコンの更新
        lastGoNo = tryNo + 1;
        //次のアイコン位置取得
        appearIconPos = stageIcons[lastGoNo].transform.position;
        //新しい線を追加
        Vector3 lastLinePos = clearStageIcon.transform.position;
        lr.positionCount = lastGoNo + 1;
        lr.SetPosition(lastGoNo, lastLinePos);
        //lastGoNosにも登録
    }

    public GameObject getStageIcon(int no)
    {
        return stageIcons[no];
    }

    //移動可能番号を取得
    private int Get_lastGoNo()
    {
        return lastGoNo;
    }
}
