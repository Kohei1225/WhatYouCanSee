using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchByLever : MonoBehaviour
{
    public GameObject switchObject;//切り替えるオブジェクト
    public GameObject LeverObject;//操作元のレバー
    LeverScript leverScript;

    // Start is called before the first frame update
    void Start()
    {
        leverScript = LeverObject.GetComponent<LeverScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if(leverScript.barPosition == 0)switchObject.SetActive(true);
        else switchObject.SetActive(false);
    }
}
