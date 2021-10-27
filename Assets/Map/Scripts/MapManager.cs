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
    private Vector3[] points;
    //始点の色(線)
    public Color firstColor = Color.white;
    //終点の色(線)
    public Color endColor = Color.white;
    //プレイヤースクリプト
    private PlayerController_Map playerScript;
    //プレイヤーアニメーション
    private Animator playerAnim;

    public Text text;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        points = new Vector3[stageIcons.Length];
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController_Map>();
        text = GameObject.Find("StageName").GetComponent<Text>();
        playerAnim = GameObject.FindWithTag("Player").GetComponent<Animator>();

        //点をセット
        for (int i = 0; i < stageIcons.Length; i++)
        {
            //追加
            Vector3 pos = stageIcons[i].GetComponent<StageIcon>().GetPosision();
            points[i] = pos;
        }

        lr.startColor = firstColor;
        lr.endColor = endColor;
        SetPointsToLine();

        int num = 0;
        for (int i = 0; i < stageIcons.Length; i++)
        {
            StageIcon script = stageIcons[i].GetComponent<StageIcon>();
            if (!script.GetIsClear())
            {
                num = i;
                break;
            }
        }
        for(int i = 0; i < stageIcons.Length; i++)
        {
            if(i <= num)
            {
                stageIcons[i].SetActive(true);
            }
            else
            {
                stageIcons[i].SetActive(false);
            }
        }
    }

    private void Update()
    {
        //アイコンの上だったら
        if (!playerScript.GetCanMove())
        {
            //テキストUIにステージ名を表示(更新)
            string StageName = stageIcons[playerScript.GetGoNo()].GetComponent<StageIcon>().GetStageName();
            text.text = StageName;

            //スペースキーが押されたら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(LoadScene());
            }
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

    //public Vector3[] GetPoints()
    //{
    //    return points;
    //}

    //public GameObject[] GetStageIcons()
    //{
    //    return stageIcons;
    //}

    private List<Vector3> GetClearedPoints()
    {
        List<Vector3> clearedPoints = new List<Vector3>();
        for (int i = 0; i < stageIcons.Length; i++)
        {
            //追加
            clearedPoints.Add(stageIcons[i].GetComponent<StageIcon>().GetPosision());

            //もし未クリアのアイコンだったら
            if (!stageIcons[i].GetComponent<StageIcon>().GetIsClear())
            {
                //ここで終了
                break;
            }
        }
        return clearedPoints;
    }

    //sceneNameのステージをクリアしたときの処理
    public void Clear(string sceneName)
    {
        StageIcon stageIconScript;
        foreach (GameObject stageIcon in stageIcons)
        {
            if ((stageIconScript = stageIcon.GetComponent<StageIcon>()).GetSceneName().Equals(sceneName))
            {
                //アイコンの更新
                stageIconScript.Clear();
                //線、移動可能範囲更新
                SetPointsToLine();
                return;
            }
        }
    }

    public GameObject getStageIcon(int no)
    {
        return stageIcons[no];
    }
}
