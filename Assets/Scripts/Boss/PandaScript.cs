using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaScript : BossBase
{
    [SerializeField] private int firstHp = 3;   //パンダの体力
    [SerializeField] private int firstVelocity; //パンダのスピード
    private const float DISTANCE1 = 3f;//攻撃１をする距離
    private const float DISTANCE2 = 6f;//攻撃２をする距離
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        this.hp = firstHp;
        this.velocity = firstVelocity;
        this.timeInterval = 50;
        this.time = timeInterval;
        //Debug.Log("time:" + time + " inter:" + timeInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
        //まだ体力がある
        if(this.hp > 0)
        {
            distance = Mathf.Abs(GameObject.Find("Player").transform.position.x - transform.position.x);
            //ある程度プレイヤーに近づくまでジリジリ近づく
            if( distance > DISTANCE2)
                Idle();
            else if(!this.attack)
            {
                attack = true;

                //残り体力とか状況によって攻撃が変わる
                if(this.hp == 3)
                    Attack1();
                else if(this.hp == 2)
                    Attack2();
                else if(this.hp == 1)
                    Attack3();
            }

            //攻撃にインターバルをつける。一定の時間が経つまでは攻撃できない。
            if(time < timeInterval)time++;
            if(time == timeInterval)
            {
                this.attack = false;
            }
        }
        //もう体力がない
        else 
        {
            //体力が無くなったら一回だけ処理する
            if(!this.dead)
            {
                this.dead = true;
                this.Die();
            }
        }
    }

    //通常状態
    public override void Idle()
    {
        //通常状態の処理
    }

    //移動
    public override void Move()
    {
        //移動の処理
    }

    //攻撃1
    public override void Attack1()
    {
        //攻撃１の処理
    }

    //攻撃2
    public override void Attack2()
    {
        //攻撃２の処理
    }

    //攻撃3
    public override void Attack3()
    {
        //攻撃３の処理
    }

    //ダメージ処理
    public override void Damage()
    {
        //ダメージを受けた時の処理(アニメーション再生とか)
        Debug.Log("ダメージ受けた！！残り残機:" + this.hp);
    }

    public override void Die()
    {
        //死ぬ処理
        Debug.Log("ダメージ。あ！死んだ！！");
    }
}
