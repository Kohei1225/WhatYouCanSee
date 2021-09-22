using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldObjectScript: MonoBehaviour
{
    public GameObject objectFrontMe;

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
        if(otherObject.transform.root.gameObject.GetComponent<ColorObjectScript>() != null)
        {
            ColorObjectScript colorObject = otherObject.gameObject.transform.root.GetComponent<ColorObjectScript>();
            //Debug.Log("前のオブジェクトはコンポーネントもってる");
            //対象が運べる状態だったら
            if(colorObject.isObject && colorObject.canHold && otherObject.transform.root.gameObject.tag == "ColorObject")
            {
                //プレイヤーが何ももってない時は記録しておく
                if(!transform.root.gameObject.GetComponent<PlayerController>().isHoldingObject)
                {
                    //Debug.Log("前にオブジェクトある");
                    objectFrontMe = otherObject.gameObject;
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
        if(otherObject.transform.root.gameObject.GetComponent<ColorObjectScript>())
        {
            ColorObjectScript colorObject = otherObject.gameObject.transform.root.gameObject.GetComponent<ColorObjectScript>();

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
}
