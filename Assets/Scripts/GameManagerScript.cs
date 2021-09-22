using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ステージの状態を前もって設定しておくクラス
public class GameManagerScript : MonoBehaviour
{
    public bool existRay;   //そもそも光が存在するか。falseならステージ全体が明るい
    public bool existShadow;//光があっても影が存在するか。falseならステージに影の部分が存在しなくなる
    public GameObject [] objectList; 

    void Awake()
    {
        if(!existRay)existShadow = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
