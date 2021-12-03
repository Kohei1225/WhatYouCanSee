using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レバースイッチにアタッチするスクリプト
public class SwitchByLever : MonoBehaviour
{
    public GameObject switchObject;//切り替えるオブジェクト
    public GameObject LeverObject;//操作元のレバー
    LeverScript leverScript;
    //反対か
    [SerializeField] private bool _IsReverse = false;

    void Awake()
    {
        leverScript = LeverObject.GetComponent<LeverScript>();
        if(leverScript.barPosition == 0)switchObject.SetActive(!_IsReverse);
        else switchObject.SetActive(_IsReverse);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(leverScript.barPosition == 0)switchObject.SetActive(!_IsReverse);
        else switchObject.SetActive(_IsReverse);
    }
}
