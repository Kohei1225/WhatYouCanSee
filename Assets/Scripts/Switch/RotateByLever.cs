using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レバーの傾きに応じて自身の傾きを調整するクラス
public class RotateByLever : MonoBehaviour
{
    public float[] angleList = new float[4];//傾きリスト
    public GameObject LeverObject;          //参照するレバー
    LeverScript leverScript;
    public float rotateSpeed;               //傾くスピード

    // Start is called before the first frame update
    void Start()
    {
        leverScript = LeverObject.GetComponent<LeverScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //傾きを調整
        Vector3 barAngle = gameObject.transform.localEulerAngles;
        float angleDifference = angleList[leverScript.barPosition] - barAngle.z;

        //十分傾いてたら傾ける操作をしない
        if(Mathf.Abs(angleDifference) > 0.1f)
        {
            if(angleDifference > 0)barAngle.z += rotateSpeed;
            if(angleDifference < 0)barAngle.z -= rotateSpeed;
            gameObject.transform.localEulerAngles = barAngle;
        }
    }
}
