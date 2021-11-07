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
    //ワールド数
    private static int worldNum = System.Enum.GetValues(typeof(WorldName)).Length;
    //今いけるステージの番号
    public static int[] lastStageGoNos = new int[]{
        4,
        1,
        1,
        1
    };
    //ワールドごとのステージ数
    public static int[] stageNum = new int[]{
        5,
        6,
        6,
        5
    };
    //ワールドの番号
    private WorldName worldName;


    public GameObject[] stageIcons;
    private LineRenderer lr;
    //
    //始点の色(線)
    public Color firstColor = Color.white;
    //終点の色(線)
    public Color endColor = Color.white;
    //プレイヤースクリプト
    private PlayerController_Map playerScript;
    //プレイヤーアニメーション
    private Animator playerAnim;

    public Text text;

    public static bool isClear = false;

    private Vector3 appearIconPos;
    private bool isAppear = false;
    public float appearSpeed = 0.1f;

    //移動可能アイコン番号
    public int lastGoNo = 0;

    //挑戦中のステージのアイコン番号
    public static int tryNo = 0;

    private AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Awake()
    {
        //今のワールドのクリア状況を読み取る
        lastGoNo = Get_lastGoNo();

        //DontDestroyOnLoad(this);
        lr = GetComponent<LineRenderer>();
        //Vector3[] points = new Vector3[stageIcons.Length];
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController_Map>();
        text = GameObject.Find("StageName").GetComponent<Text>();
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();

        lr.startColor = firstColor;
        lr.endColor = endColor;
        SetPointsToLine();

        for (int i = 0; i < stageIcons.Length; i++)
        {
            if(i <= lastGoNo)
            {
                stageIcons[i].SetActive(true);
            }
            else
            {
                stageIcons[i].SetActive(false);
            }
        }

        if (isClear)
        {
            isClear = false;
            Clear();
        }
        //テキストUIにステージ名を表示(更新)
        string StageName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetStageName();
        text.text = StageName;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //アイコンの上かつプレイヤーのキーが押せる状態だったら
        if (!playerScript.GetCanMove() && playerScript.GetCanPush())
        {
            //テキストUIにステージ名を表示(更新)
            string StageName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetStageName();
            text.text = StageName;

            //スペースキーが押されたら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //シーン読み込み
                StartCoroutine(LoadScene());
            }
        }

        if (isAppear)
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
                isAppear = false;
                //アイコンを見えるようにする
                stageIcons[lastGoNo].SetActive(true);
                SetPointsToLine();
                //動けるようにする
                playerScript.SetCanPush(true);
                return;
            }
            lastLinePos += (deltaVec.normalized / deltaVec.magnitude) * appearSpeed * Time.deltaTime;
            //線の最後の位置更新
            lr.SetPosition(lastGoNo, lastLinePos);
        }
    }

    private IEnumerator LoadScene()
    {
        //プレーヤー操作不能
        playerScript.SetCanPush(false);
        //もし今いるのが次のワールドに行くものだったら
        StageIcon stageIcon = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>();
        if (stageIcon.GetType() == typeof(WorldIcon))
        {
            WorldIcon worldIcon = (WorldIcon)stageIcon;
            //次に進むものだったら
            if (worldIcon.Get_isNext())
            {
                //次のワールドのプレイヤー位置は0
                tryNo = 0;
            }
            //戻るものだったら
            else
            {
                //次のワールドのプレイヤー位置はワールドのステージ数-1
                tryNo = stageNum[(int)worldName - 1] - 1;
            }
            //音再生
            audioSource.PlayOneShot(audioClips[1]);
        }
        else
        {
            //挑戦するステージアイコン記憶
            tryNo = playerScript.GetGoNo();
            //プレイヤージャンプ
            playerScript.Jump();
            //音再生
            audioSource.PlayOneShot(audioClips[0]);
        }
        yield return new WaitForSeconds(2);
        //シーン読み込み
        string sceneName = stageIcon.GetSceneName();
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
            return;
        }
        StageIcon stageIconScript = clearStageIcon.GetComponent<StageIcon>();
        //アイコンの更新
        lastGoNo = tryNo + 1;
        //次のアイコン位置取得
        appearIconPos = stageIcons[lastGoNo].transform.position;
        //道が現れるフラグON
        isAppear = true;
        //動けなくする
        playerScript.SetCanPush(false);
        //新しい線を追加
        Vector3 lastLinePos = clearStageIcon.transform.position;
        lr.positionCount = lastGoNo + 1;
        lr.SetPosition(lastGoNo, lastLinePos);
        //lastGoNosにも登録
        Set_lastGoNo(lastGoNo);
    }

    public GameObject getStageIcon(int no)
    {
        return stageIcons[no];
    }

    //ワールドに応じた移動可能番号を取得
    private int Get_lastGoNo()
    {
        return lastStageGoNos[(int)worldName];
    }
    //ワールドに応じた移動可能番号をセット
    private void Set_lastGoNo(int lastGoNo)
    {
        lastStageGoNos[(int)worldName] = lastGoNo;
    }
}
