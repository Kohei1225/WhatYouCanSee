using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//頭の部分にアタッチする踏まれたかどうかを判定するスクリプト
public class HeadPointScript : MonoBehaviour
{
    [SerializeField] private BossBase bossScript;

    // Start is called before the first frame update
    void Start()
    {
        //名前によって取得するコンポーネントを変更...?(まぁとりあえず)
        switch(gameObject.transform.root.gameObject.name)
        {
            case "Panda":
                bossScript = gameObject.transform.root.gameObject.GetComponent<PandaScript>();
                break;
            //default:break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D otherObject)
    {
        //プレイヤーに当たったら
        if(otherObject.transform.root.gameObject 
        && otherObject.transform.root.gameObject.GetComponent<PlayerController>())
        {
            GameObject playerObject = otherObject.transform.root.gameObject;
            PlayerController pc = playerObject.GetComponent<PlayerController>();
            //Debug.Log("damage:" + pc.damage + " time:" + bossScript.time + " interval:" + bossScript.timeInterval);

            //踏めないタイミングだったら何もしない
            if(pc.damage || bossScript.time < bossScript.timeInterval)return;

            //踏めるタイミングならプレイヤーを上に飛ばしてダメージ処理
            Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
            Vector3 force = new Vector3(0,pc.vForce,0);
            rb.AddForce(force);
            bossScript.BeAttacked();
        }
        //Debug.Log(otherObject.name);
    }
}
