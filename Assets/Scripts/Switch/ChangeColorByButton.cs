using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボタンが押されるたびに色を切り替えるクラス
public class ChangeColorByButton : MonoBehaviour
{
    ButtonSwitchScript buttonScript;    
    public GameObject buttonObject;     //ボタンのオブジェクト
    ColorObjectVer3 colorScript;
    public ColorObjectVer3.OBJECT_COLOR3[] colorList;  //変更される色の順番

    bool hasPushed;     //すでに押されてる状態なのかを記録
    int colorNum = 0;   //色を一つ目にリセットしておく

    // Start is called before the first frame update
    void Start()
    {
        colorScript = gameObject.GetComponent<ColorObjectVer3>();
        buttonScript = buttonObject.GetComponent<ButtonSwitchScript>();
        hasPushed = false;
    }

    // Update is called once per frame
    void Update()
    {
        //ボタンが押されたら
        if(buttonScript.Get_isPushed())
        {
            //押された瞬間だけ色を切り替える
            if(!hasPushed)
            {
                colorNum++;
                if(colorNum == colorList.Length)colorNum = 0;
                colorScript.colorType = colorList[colorNum];
                hasPushed = true;
            }
        }
        else hasPushed = false;
    }
}
