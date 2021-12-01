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

    private float startAngle;
    private float endAngle;


    // Start is called before the first frame update
    void Start()
    {
        leverScript = LeverObject.GetComponent<LeverScript>();

        startAngle = angleList[leverScript.firstBarPos];
        endAngle = startAngle;
    }

    // Update is called once per frame
    void Update()
    {
        //傾きを調整
        Vector3 barAngle = gameObject.transform.localEulerAngles;

        if(leverScript.canChangeBar && Input.GetKeyDown(KeyCode.C))
        {
            ChangeAngle();
        }

        barAngle.z = Mathf.Lerp(startAngle,endAngle,(leverScript.timeSum * rotateSpeed)/Mathf.Abs(endAngle - startAngle));

        Debug.Log((leverScript.timeSum * rotateSpeed) / Mathf.Abs(endAngle - startAngle));
        gameObject.transform.localEulerAngles = barAngle;
    }

    /// <summary> バーの角度を変更 </summary>
    public void ChangeAngle()
    {
        Debug.Log("RotateScript");
        startAngle = gameObject.transform.localEulerAngles.z;

        endAngle = angleList[leverScript.barPosition];
    }
}
