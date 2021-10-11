using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ステージの状態を前もって設定しておくクラス。いろんなクラスがこのクラスの値を参照する。
public class GameManagerScript : MonoBehaviour 
{
    [SerializeField] GameObject[] colorObjectList; //ステージにあるオブジェクトのリスト。LightRayのクラスで使う。
    public bool existRay;   //そもそも光源が存在するか。falseならステージ全体が明るくなる
    public bool existShadow;//光源があっても影が存在するか。falseならオブジェクトに影ができなくなる
    

    void Awake()
    {
        //そもそも光源が存在しないなら影も存在しないのでそこを調整する
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

    public GameObject[] Get_colorObjectList()
    {
        return this.colorObjectList;
    }
}
