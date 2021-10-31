using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
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
    [SerializeField] public static int lastGoNo = 0;

    //挑戦中のステージのアイコン番号
    public static int tryNo;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        //Vector3[] points = new Vector3[stageIcons.Length];
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController_Map>();
        text = GameObject.Find("StageName").GetComponent<Text>();
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();

        lr.startColor = firstColor;
        lr.endColor = endColor;
        SetPointsToLine();

        for(int i = 0; i < stageIcons.Length; i++)
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
                //挑戦するステージアイコン記憶
                tryNo = playerScript.GetGoNo();
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
        playerScript.Jump();
        yield return new WaitForSeconds(2);
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
    }

    public GameObject getStageIcon(int no)
    {
        return stageIcons[no];
    }
}
