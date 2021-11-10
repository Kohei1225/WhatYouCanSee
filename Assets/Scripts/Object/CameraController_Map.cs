using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController_Map : MonoBehaviour
{
    //ワールドオブジェ
    public GameObject[] worlds;
    //ワールドの位置
    private Vector3[] worldPoses;
    //移動するワールドの番号
    public static int goWorldNo = 0;
    //カメラは動くか
    private bool canMove = false;
    //移動スピード
    private float moveSpeed = 40f;

    private float rate;
    // Start is called before the first frame update
    void Start()
    {
        worldPoses = new Vector3[worlds.Length];
        for (int i = 0; i < worlds.Length; i++)
        {
            worldPoses[i] = worlds[i].transform.position;
            //カメラのz座標に合わせる
            worldPoses[i].z = transform.position.z;
        }
        //カメラを決められたワールドの座標にする
        Vector3 worldPos = worlds[goWorldNo].transform.position;
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
    }

    public void MoveCamera(Vector3 playerMovedVec)
    {
        if(canMove)
            transform.Translate(Vector3.right * playerMovedVec.x * rate);
    }

    public void CuluculateRate(int goWorldNo, Vector3 playerToIcon)
    {
        //比率計算
        rate = (worldPoses[goWorldNo].x - transform.position.x) / playerToIcon.x;
        //行くワールド座標更新
        CameraController_Map.goWorldNo = goWorldNo;
        canMove = true;
    }

    public void TranslateGoPos()
    {
        transform.position = worldPoses[goWorldNo];
        canMove = false;
    }
}
