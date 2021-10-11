using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーがモノを持ち運んだり投げたりするためのクラス
public class HoldObjectScript: MonoBehaviour
{
    GameObject objectFrontMe;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //
    void OnTriggerStay2D(Collider2D otherObject)
    {
        //Debug.Log("当たった");
        //もしも触れてるオブジェクトがカラーオブジェクトだったら
        if(otherObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>())
        {
            GameObject touchingObject = otherObject.transform.parent.gameObject;
            ColorObjectVer3 colorObject = touchingObject.GetComponent<ColorObjectVer3>();
            
            //対象が運べる状態だったら
            if(colorObject.isObject && colorObject.canHold && touchingObject.tag == "ColorObject")
            {
                //プレイヤーが何ももってない時は記録しておく
                if(!transform.root.gameObject.GetComponent<PlayerController>().Get_isHoldingObject())
                {
                    //Debug.Log("前にオブジェクトある");
                    objectFrontMe = touchingObject;
                }
            }
            else objectFrontMe = null;
        }
        else objectFrontMe = null;
        //else Debug.Log("ない");
    }

    void OnTriggerExit2D(Collider2D otherObject)
    {
        //もしも触れてるオブジェクトがカラーオブジェクトだったら
        if(otherObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>())
        {
            GameObject touchingObject = otherObject.transform.parent.gameObject;
            ColorObjectVer3 colorObject = touchingObject.GetComponent<ColorObjectVer3>();

            //対象が運べる状態だったら
            if(colorObject.isObject && colorObject.canHold)
            {
                /*
                //プレイヤーが何ももってない時は記録しておく
                if(GetComponent<PlayerController>().havingObject)
                {
                    objectBeingHolden = otherObject.gameObject;
                }
                */
                objectFrontMe = null;
            }
        }
    }

    public GameObject Get_objectFrontMe()
    {
        return this.objectFrontMe;
    }
}
