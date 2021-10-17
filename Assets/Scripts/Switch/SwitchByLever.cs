using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レバースイッチにアタッチするスクリプト
public class SwitchByLever : MonoBehaviour
{
    public GameObject switchObject;//切り替えるオブジェクト
    public GameObject LeverObject;//操作元のレバー
    LeverScript leverScript;

    void Awake()
    {
        leverScript = LeverObject.GetComponent<LeverScript>();
        if(leverScript.Get_barPosition() == 0)switchObject.SetActive(true);
        else switchObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(leverScript.Get_barPosition() == 0)switchObject.SetActive(true);
        else switchObject.SetActive(false);
    }
}
