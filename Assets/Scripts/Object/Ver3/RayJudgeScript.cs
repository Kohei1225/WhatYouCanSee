using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//それぞれのRay用の当たり判定がどんな状態かを保持して更新するクラス
public class RayJudgeScript : MonoBehaviour
{
    public GameObject[] colliderList;//Ray用コライダーのリスト
    public bool[] rayList;//値を保持するリスト
    ColorObjectVer3 colorObject;
    GameManagerScript gameManagerScript;

    void Awake()
    {
        //コライダーに番号を振り分けていく
        for(int i = 0;i < colliderList.Length;i++)
        {
            colliderList[i].GetComponent<RayColliderScript>().SetListNumber(i);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        colorObject = GetComponent<ColorObjectVer3>();
        rayList = new bool[colliderList.Length];
        gameManagerScript = GameObject.Find("Managers").GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //全体が明るいステージなら常に光に当たってる判定
        if(!gameManagerScript.existRay)
        {
            colorObject.Set_onLightRay(true);
            colorObject.Set_onShadowRay(false);
        }
        else
        {
            //光が当たってるかを報告
            colorObject.Set_onLightRay(CheckListContain(true));
            //影が当たってるかどうかを報告
            colorObject.Set_onShadowRay(CheckListContain(false));
        }
        
    }

    //指定した値が含まれているかを調べる
    bool CheckListContain(bool type)
    {
        for(int i = 0;i < rayList.Length;i++)
        {
            //Debug.Log(i + ":" + rayList[i]);
            if(type == rayList[i])return true;
        }
        return false;
    }

    //外部から配列の要素を更新させるメソッド
    public void Set_RayJudgeValue(int num,bool value)
    {
        this.rayList[num] = value;
    }

}