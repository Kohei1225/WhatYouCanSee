using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ray用の小さい当たり判定を管理するクラス
public class RayColliderScript : MonoBehaviour
{
    bool onRay;     //そもそもRayに当たったかどうかの判定
    bool onLightRay;//光に当たった判定用
    int listNumber;//割り当てられる番号
    public bool forDebug = false;

    // Start is called before the first frame update
    void Start()
    {
        //コライダーが正方形になるようにサイズ調整
        Vector3 scale = transform.localScale;
        scale.x = scale.x/(transform.parent.gameObject.transform.localScale.x/3);
        scale.y = scale.y/(transform.parent.gameObject.transform.localScale.y/3);
        transform.localScale = scale;
    }

    void FixedUpdate()
    {
        //transform.parent.gameObject.GetComponent<RayJudgeScript>().Set_RayJudgeValue(listNumber,onLightRay);
    }

    // Update is called once per frame
    void Update()
    {
        transform.parent.gameObject.GetComponent<RayJudgeScript>().Set_RayJudgeValue(listNumber, onLightRay);
        if (gameObject.transform.root.gameObject.Equals(GameObject.Find("Player")))
        {
            if (!onRay)
            {
                Debug.LogError("Not Hit Ray");
            }
        }
        //Rayに当たらなければ一旦リセット
        if (!onRay)onLightRay = false;
        onRay = false;
    }

    //外部から番号を設定するメソッド
    public void SetListNumber(int num)
    {
        this.listNumber = num;
    }

    //光が当たった時に設定する
    public void SetLightVariable(bool value)
    {
        this.onRay = true;
        this.onLightRay = value;
    }

    public void Set_onRay()
    {
        this.onRay = true;
    }
}