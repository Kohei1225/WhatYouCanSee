using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 踏まれたかどうかを判定.
/// 頭の部分にアタッチする
/// </summary>
public class HeadPointScript : MonoBehaviour
{
    [SerializeField] private BossBase _BossScript;
    // Start is called before the first frame update
    void Start()
    {
        _BossScript = gameObject.transform.root?.gameObject.GetComponent<BossBase>();

        if(_BossScript == null)
        {
            Debug.Log("BossBaseを継承したクラスが見つかりません.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D otherObject)
    {
        //触れた対象を一旦保存
        GameObject playerObject = otherObject.transform.root?.gameObject;
        PlayerController pc = playerObject?.GetComponent<PlayerController>();

        //対象がプレイヤーの足意外なら終了
        if (pc == null || otherObject.gameObject.name != "Foot")
        {
            //Debug.Log("Unable to Attack Panda!!");
            return;
        }

        //踏めないタイミングだったら何もしない
        if(pc.damage)return;

        //踏めるタイミングならプレイヤーを上に飛ばしてダメージ処理
        var vel = playerObject.GetComponent<Rigidbody2D>().velocity;
        vel.y = playerObject.GetComponent<PlayerController>().jumpSpeed;
        playerObject.GetComponent<Rigidbody2D>().velocity = vel;

        //無敵時間だったらダメージは受けない
        if (_BossScript.IsUnableBeAttacked) return;
        _BossScript.BeAttacked();
    }
}
