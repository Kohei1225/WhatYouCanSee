using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ステージの設定によって光源の管理をするクラス
public class LightsManagerScript : MonoBehaviour
{
    public GameObject[] lightList;//光源のリスト(全体が明るい奴[0]と範囲を照らす奴等[1])
    GameManagerScript gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        
        gameManagerScript = GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //ステージ全体の状態を取得して、それに応じてそれぞれの光源を種類別に有効/無効化する。
        lightList[0].SetActive(!gameManagerScript.existRay);//全体が明るい光源
        lightList[1].SetActive(gameManagerScript.existRay);//範囲を照らす光源
    }
}
