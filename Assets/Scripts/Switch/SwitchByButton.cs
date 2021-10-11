using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトをボタンの状態に応じてSetActiveを切り替えるクラス
public class SwitchByButton : MonoBehaviour
{
    public GameObject switchObject;     //参照するボタン
    public GameObject buttonObject;     //切り替える対象
    ButtonSwitchScript buttonScript;

    void Awake()
    {
        buttonScript = buttonObject.GetComponent<ButtonSwitchScript>();
        switchObject.SetActive(buttonScript.Get_isPushed());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //ボタンが押されてる間だけオブジェクトを無効化する
        switchObject.SetActive(buttonScript.Get_isPushed());
    }

}
