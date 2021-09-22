using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトの角にアタッチするスクリプト。同色オブジェクト、又は影に触れてるかを判定する
public class EdgeScript : MonoBehaviour
{
    ColorObjectScript colorObject;//親オブジェクトの色情報
    public enum EDGE_TYPE   //何用の角なのかの判定
    {
        COLOR,          //同色オブジェクトに触れてる判定用
        SHADOW,         //影に触れてる判定用
        TYPE_END,       //typeの数
    }
    public EDGE_TYPE edgeType;  //type自体はUnity側で操作する
    public int EdgeNum;         //角の番号
    public bool onRay;          //光が当たった判定
    public bool forDebug;
    public bool inSameColor;

    // Start is called before the first frame update
    void Start()
    {
        colorObject = transform.parent.gameObject.GetComponent<ColorObjectScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(forDebug)Debug.Log("TYPE:" + colorObject.colorType);
        if(this.edgeType == EDGE_TYPE.SHADOW)
        {
            if(GameObject.Find("Managers").transform.Find("GameManager").gameObject.GetComponent<GameManagerScript>().existRay)
            {
                transform.parent.gameObject.GetComponent<EdgeCheckerScript>().edgeShadowList[EdgeNum] = this.onRay;
            }
            else
            {
                transform.parent.gameObject.GetComponent<EdgeCheckerScript>().edgeShadowList[EdgeNum] = true;
            }
            this.onRay = false;
        }

            
        //Debug.Log((int)edgeType);
        //onRay = false;
        onRay = false;
    }

    

    void OnTriggerStay2D(Collider2D other)
    {
        //オブジェクトじゃないやつに触れてたらTrueになる
        if(other.gameObject.GetComponent<ColorObjectScript>())
        {
            //if(forDebug)Debug.Log(colorObject.colorType + ":" + other.gameObject.GetComponent<ColorObjectScript>().colorType);
            if(this.edgeType == EDGE_TYPE.COLOR && !other.gameObject.GetComponent<ColorObjectScript>().isObject)
            {
                //Debug.Log("触れて入る");
                //角がエリアに侵入してたらリストの要素をTrueにする
                if(other.gameObject.GetComponent<ColorObjectScript>().colorType == colorObject.colorType)
                {
                    transform.parent.gameObject.GetComponent<EdgeCheckerScript>().edgeColorList[EdgeNum] = true;
                    inSameColor = true;
                }
                else 
                {
                    transform.parent.gameObject.GetComponent<EdgeCheckerScript>().edgeColorList[EdgeNum] = false;
                    inSameColor = false;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<ColorObjectScript>() && this.edgeType == EDGE_TYPE.COLOR && !other.gameObject.GetComponent<ColorObjectScript>().isObject)
        {
            //角がエリアから出たら要素をFalseにする。
            if(other.gameObject.GetComponent<ColorObjectScript>().colorType == colorObject.colorType)
            {
                transform.parent.gameObject.GetComponent<EdgeCheckerScript>().edgeColorList[EdgeNum] = false;
                inSameColor = false;
            }
        }
    }
}
