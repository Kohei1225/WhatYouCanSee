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
        if(otherObject.transform.parent.gameObject
        && otherObject.transform.parent.gameObject.GetComponent<PlayerController>())
        {
            GameObject playerObject = otherObject.transform.root.gameObject;
            PlayerController pc = playerObject.GetComponent<PlayerController>();
            //Debug.Log("damage:" + pc.damage + " dadmageTime:" + bossScript.dadmageTime + " interval:" + bossScript.damageTimeInterval);

            //踏めないタイミングだったら何もしない
            if(pc.damage || bossScript._IsUnableBeAttacked)return;

            //踏めるタイミングならプレイヤーを上に飛ばしてダメージ処理
            var vel = playerObject.GetComponent<Rigidbody2D>().velocity;
            vel.y = playerObject.GetComponent<PlayerController>().jumpSpeed;
            playerObject.GetComponent<Rigidbody2D>().velocity = vel;
            bossScript.BeAttacked();

            //Debug.Log("プレイヤー当たった");
        }
        //Debug.Log(otherObject.gameObject.transform.root.gameObject.name);
    }
}
