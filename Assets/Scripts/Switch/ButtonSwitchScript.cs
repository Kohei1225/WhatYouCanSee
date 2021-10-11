using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボタン(スイッチ)にアタッチするスクリプト
public class ButtonSwitchScript : MonoBehaviour
{
    bool isPushed;                  //押されてるかを判定する変数
    float changeSizeSpeed = 0.04f;
    const float HEIGHT_NOT_PUSH = 0.2f;
    const float HEIGHT_ON_PUSH = 0;

    GameObject pushButtonObject;    //踏む部分にあるオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを名前で取得
        pushButtonObject = transform.Find("Button").gameObject;
        isPushed = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ボタンのサイズを取得
        Vector3 buttonSize = pushButtonObject.transform.localScale;
        float heightDifference = HEIGHT_NOT_PUSH - buttonSize.y;
        if(isPushed)heightDifference = HEIGHT_ON_PUSH - buttonSize.y;

        //ボタンの大きさを調整(踏んでる間はオブジェクトの大きさ自体が縮小する感じ)
        if(Mathf.Abs(heightDifference) > 0.001f)
        {
            if(heightDifference > 0)buttonSize.y += changeSizeSpeed;
            if(heightDifference < 0)buttonSize.y -= changeSizeSpeed;
            pushButtonObject.transform.localScale = buttonSize;
        }
        //isPushed = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        isPushed = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isPushed = false;
    }

    //押されてるかを返すメソッド
    public bool Get_isPushed()
    {
        return this.isPushed;
    }
}
