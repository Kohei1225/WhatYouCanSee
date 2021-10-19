using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        points = new Vector3[stageIcons.Length];
        playerScript = GameObject.FindWithTag("Player").GetComponent<PlayerController_Map>();

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
}
