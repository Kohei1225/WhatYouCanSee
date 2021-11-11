using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトの角がどんな状態なのかをまとめるクラス
public class EdgeCheckerScript : MonoBehaviour
{
    public bool[] edgeColorList;//同色オブジェクトに触れてればTrueになる
    public bool[] edgeShadowList;//光に当たってる角はTrueになる
    public bool[] edgeObjectList;//内側にオブジェクトが侵入してきてたらTrueになる
    public bool sameColor;
    ColorObjectScript colorObject;//背景のオブジェクトにアタッチされてるBackObjectScriptから操作する変数。どうしようもないからこうした
    GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        sameColor = false;
        //値の初期化
        for(int i = 0;i < edgeColorList.Length;i++)
        {           
            edgeColorList[i] = false;
        }

        for(int i = 0;i < edgeShadowList.Length;i++)
        {           
            edgeShadowList[i] = false;
        }

        for(int i = 0;i < edgeObjectList.Length;i++)
        {           
            edgeObjectList[i] = false;
        }

        //コンポーネント取得
        colorObject = GetComponent<ColorObjectScript>();
        //body = gameObject.transform.Find("Body");
        //Debug.Log(body.name);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        //完全に同色オブジェクトと被ってるかどうかの判定
        for(int i = 0;i < edgeColorList.Length;i++)
        {
            if(!edgeColorList[i])
            {
                GetComponent<ColorObjectScript>().inSameColor = false;
                //Debug.Log(gameObject.name + ":違うゾ");
                break;
            }

            //全部触れてたらinSameColorをTrueにする
            if(i == edgeColorList.Length - 1)
            {
                //Debug.Log(gameObject.name + ":同じ！！");
                GetComponent<ColorObjectScript>().inSameColor = true;
            }
        }
        */

        bool hasTouchedShadow = false;//影にふれた印

        /*
        //影に当たってるかどうかの判定(実際は光に当たってる判定)
        for(int i = 0;i < edgeShadowList.Length;i++)
        {
            //影に当たってたら
            if(!edgeShadowList[i])
            {
                //影には触れてるので印をつける
                GetComponent<ColorObjectScript>().touchingShadow = true;
                if(gameObject.name == "Square (10)")Debug.Log(gameObject.name + ":影に触れてるよ");
                //最初の角が触れてないのに影に触れてたら
                if(i > 0 && !hasTouchedShadow)
                {
                    GetComponent<ColorObjectScript>().outOfShadow = true;
                    break;
                }

                //影に触れてる印をつける
                hasTouchedShadow = true;
            }
            else 
            {
                GetComponent<ColorObjectScript>().outOfShadow = true;
            }

            //ここに来るということは全ての角が光に当たっているということなのでfaseを返す
            if(i == edgeShadowList.Length - 1)
            {
                //全部影に入ってたらoutOfShadowをfalseにする
                if(hasTouchedShadow)
                {
                    GetComponent<ColorObjectScript>().outOfShadow = false;
                    break;
                }

                if(gameObject.name == "Square (10)")Debug.Log(gameObject.name + ":影に触れてない！");
                GetComponent<ColorObjectScript>().touchingShadow = false;
                GetComponent<ColorObjectScript>().outOfShadow = true;
            }
        }
        */

        //全部の角が影に入ってたら
        if(AllBoolValueSameChecker(false,edgeShadowList))colorObject.outOfShadow = false;
        //一つでも影の外にあったら
        else colorObject.outOfShadow = true;

        //全部の角が影の外にあったら
        if(AllBoolValueSameChecker(true,edgeShadowList))colorObject.touchingShadow = false;
        //一つでも影の中にあったら
        else colorObject.touchingShadow = true;
        /*
        if(edgeShadowList[2] && edgeShadowList[3] && gameObject.tag == "Player")
        {
            colorObject.noBody = true;// body.SetActive(false);
            colorObject.active = true;
        }
        else colorObject.noBody = false;// body.SetActive(true);
        */

        //同色オブジェクトに入ってるかの判定
        if(AllBoolValueSameChecker(true,edgeColorList) || sameColor)colorObject.inSameColor = true;
        else colorObject.inSameColor = false;
        if(sameColor)colorObject.inSameColor = true;

        //他のオブジェクトに触れてるかの判定
        if(AllBoolValueSameChecker(false,edgeObjectList))colorObject.touchingObject = false;
        else colorObject.touchingObject = true;

        sameColor = false;
    }

    //全部の値が指定された値と同じ値かを返すメソッド
    bool AllBoolValueSameChecker(bool baseValue,bool [] array)
    {
        for(int i = 0;i < array.Length;i++)
        {
            if(baseValue != array[i])
            {
                return false;
            }
        }
        return true;
    }
}
