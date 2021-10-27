using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    //ズームが終わるまでの所要時間
    public float zoomSec = 1.0f;

    //移動の速さ
    private float moveSpeed;
    //最初の位置
    private Vector3 firstPos;
    //最後の位置
    private Vector3 lastPos; 
    //最終的な位置のゲームオブジェクト
    public GameObject lastPosObj;

    //ズームの速さ
    private float zoomSpeed;
    //最初のサイズ
    private float firstOrthographicSize;
    //最終的なサイズ
    public float lastOrthographicSize = 10;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        firstPos = transform.position;
        lastPos = lastPosObj.transform.position;
        firstOrthographicSize = cam.orthographicSize;
        int count = (int)(zoomSec / Time.deltaTime);
        float distance = GetLength(firstPos, lastPos);
        moveSpeed = distance / count;
        float deltaSize = Mathf.Abs(lastOrthographicSize - firstOrthographicSize);
        zoomSpeed = deltaSize / count;
    }

    // Update is called once per frame
    void Update()
    {
        CameraZoom();
        CameraMove();
    }

    private void CameraMove()
    {
        Vector3 from = transform.position;
        Vector3 to = lastPos;
        if(GetLength(from, to) <= moveSpeed)
        {
            transform.position = to;
        }
        else
        {
            transform.Translate(GetVector(from, to) * moveSpeed);
        }
    }

    private void CameraZoom()
    {
        if(cam.orthographicSize > lastOrthographicSize)
        {
            cam.orthographicSize -= zoomSpeed;
        }
        else
        {
            GoalController goalCtrl = GameObject.Find("Goal").GetComponentInChildren<GoalController>();
            //goalCtrl.Set_canSuck(true);
            CameraController camCtrl = GetComponent<CameraController>();
            camCtrl.enabled = false;
        }
    }

    //fromからtoまでの単位ベクトルを返す
    private Vector3 GetVector(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    //fromからtoまでの長さを返す
    private float GetLength(Vector3 from, Vector3 to)
    {
        return (to - from).magnitude;
    }
}
