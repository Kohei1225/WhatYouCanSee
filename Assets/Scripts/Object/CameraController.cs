using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    //ズームが終わるまでの所要時間
    public float zoomSec = 1.0f;

    //最初の位置
    private Vector3 firstPos;
    //最後の位置
    private Vector3 lastPos; 
    //最終的な位置のゲームオブジェクト
    public GameObject lastPosObj;
    //1秒に動くベクトル量
    private Vector3 movePosPerSec;

    //最初のサイズ
    private float firstOrthographicSize;
    //最終的なサイズ
    public float lastOrthographicSize = 10;
    //1秒に変化するサイズ量
    private float changeSizePerSec;

    private bool moveFin = false;
    private bool sizeFin = false;

    private float time = 0;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        firstPos = transform.position;
        lastPos = new Vector3(lastPosObj.transform.position.x, lastPosObj.transform.position.y, -10);
        firstOrthographicSize = cam.orthographicSize;

        movePosPerSec = (lastPos - firstPos) / zoomSec;
        changeSizePerSec = (lastOrthographicSize - firstOrthographicSize) / zoomSec;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        CameraZoom();
        CameraMove();
        FinCheck();
    }

    private void CameraMove()
    {
        transform.position = firstPos + movePosPerSec * time;
    }

    private void CameraZoom()
    {
        cam.orthographicSize = firstOrthographicSize + changeSizePerSec * time;
    }

    private void FinCheck()
    {
        if(time >= zoomSec)
        {
            GoalController goalCtrl = GameObject.Find("Goal").GetComponentInChildren<GoalController>();
            //goalCtrl.Set_canSuck(true);
            CameraController camCtrl = GetComponent<CameraController>();
            camCtrl.enabled = false;
        }
    }
}
