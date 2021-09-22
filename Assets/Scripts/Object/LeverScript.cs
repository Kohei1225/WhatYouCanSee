using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    public int barPosition;//バーの状態
    public bool existMidPos;//バーが真ん中で止まるかどうか
    public bool canChangeBar;//プレイヤーが近くにいる時(バーを操作できる状態)
    public GameObject barObject;//実際に動くバーのオブジェクト
    float[] barAngleList = {160,90,20,90};//バーの傾きのリスト

    // Start is called before the first frame update
    void Start()
    {
        if(existMidPos)barPosition = 1;
        else barPosition = 0;
        canChangeBar = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canChangeBar && Input.GetKeyDown(KeyCode.C))
        {
            //３段階で変えられる時
            if(existMidPos)
            {
                barPosition++;      
            }
            //２段階の時
            else
            {
                barPosition += 2;
            }

            if(barPosition >= barAngleList.Length)barPosition = 0;
        }

        //バーの傾きを調整
        Vector3 barAngle = barObject.transform.localEulerAngles;
        float angleDifference = barAngleList[barPosition] - barAngle.z;

        //十分傾いてたら傾ける操作をしない
        if(Mathf.Abs(angleDifference) > 0.1f)
        {
            if(angleDifference > 0)barAngle.z += 10;
            if(angleDifference < 0)barAngle.z -= 10;
            barObject.transform.localEulerAngles = barAngle;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerController>())
        {
            canChangeBar = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerController>())
        {
            canChangeBar = false;
        }
    }
}
