using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorByLever : MonoBehaviour
{
    LeverScript leverScript;            //レバーのスクリプト
    public GameObject leverObject;      //レバーのオブジェクト
    ColorObjectVer3 colorScript;        //色を変える対象
    public ColorObjectVer3.OBJECT_COLOR3[] colorList;  //変更される色の順番

    // Start is called before the first frame update
    void Start()
    {
        //各コンポーネント取得
        colorScript = gameObject.GetComponent<ColorObjectVer3>();
        leverScript = leverObject.GetComponent<LeverScript>();
        if(colorList.Length != leverScript.barAngleList.Length)
            Debug.Log("レバーと色の配列の長さが一致してません！！");
    }

    // Update is called once per frame
    void Update()
    {
        colorScript.colorType = colorList[leverScript.barPosition];
    }
}
