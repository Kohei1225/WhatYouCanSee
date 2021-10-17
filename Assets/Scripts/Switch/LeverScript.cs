using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レバーにアタッチするクラス
public class LeverScript : MonoBehaviour
{
    int barPosition;//バーの状態
    public bool existMidPos;//バーが真ん中で止まるかどうか
    bool canChangeBar;//プレイヤーが近くにいる時(バーを操作できる状態)
    GameObject barObject;//実際に動くバーのオブジェクト
    public float[] barAngleList = {160,90,20,90};//バーの傾きのリスト

    // Start is called before the first frame update
    void Start()
    {
        //バーのオブジェクトを取得
        barObject = transform.Find("BarObjects").gameObject.transform.Find("Lever1_bar").gameObject;

        if(existMidPos)barPosition = 1;
        else barPosition = 0;

        //バーの傾きを初期化
        barObject.transform.localEulerAngles = new Vector3(0,0,barAngleList[barPosition]);

        canChangeBar = false;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが近くでCを押すと切り替わる
        if(canChangeBar && Input.GetKeyDown(KeyCode.C))
        {
            //３段階以上で変えられる時
            if(existMidPos)
            {
                barPosition++;      
            }
            //２段階の時
            else
            {
                barPosition += 2;
            }

            //配列の最後まで来たら最初に戻る
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

    //傾き具合を返すメソッド
    public int Get_barPosition()
    {
        return barPosition;
    }
}
