using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ダメージを与えるオブジェクトにアタッチするクラス
public class DamageObjectScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D otherObject)
    {
        //プレイヤーに当たったらダメージを与える
        if(otherObject.gameObject.tag == "Player")
        {
            otherObject.gameObject.GetComponent<PlayerController>().Set_damage(true);
        }
    }

    void OnTriggerEnter2D(Collider2D otherObject)
    {
        //プレイヤーに当たったらダメージを与える
        if (otherObject.gameObject.transform.root.gameObject.tag == "Player")
        {
            otherObject.gameObject.transform.root.gameObject.GetComponent<PlayerController>().Set_damage(true);
        }
    }
}
