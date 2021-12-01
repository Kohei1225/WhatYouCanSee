using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> レバーの傾きに応じて自身の傾きを調整するクラス </summary>
public class RotateByLever : MonoBehaviour
{
    /// <summary> 傾きリスト </summary>
    public float[] angleList;
    /// <summary> 参照するレバー </summary>
    public GameObject LeverObject;
    LeverScript leverScript;
    /// <summary> 傾くスピード </summary>
    public float rotateSpeed;               
    /// <summary> 回転前の角度 </summary>
    private float startAngle;
    /// <summary> 回転後の角度 </summary>
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
        Vector3 objAngle = gameObject.transform.localEulerAngles;

        if(leverScript.canChangeBar && Input.GetKeyDown(KeyCode.C))
        {
            ChangeAngle();
        }

        objAngle.z = Mathf.Lerp(startAngle,endAngle,(leverScript.timeSum * rotateSpeed)/Mathf.Abs(endAngle - startAngle));

        Debug.Log((leverScript.timeSum * rotateSpeed) / Mathf.Abs(endAngle - startAngle));
        gameObject.transform.localEulerAngles = objAngle;
    }

    /// <summary> 自身の角度を変更 </summary>
    public void ChangeAngle()
    {
        startAngle = gameObject.transform.localEulerAngles.z;

        endAngle = angleList[leverScript.barPosition];
    }
}
