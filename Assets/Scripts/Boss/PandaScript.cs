using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PandaScript : BossScript
{
    [SerializeField] int FirstHp = 3;   
    [SerializeField] int FirstVelocity;
    int time = 0;
    int timeInterval = 50;

    // Start is called before the first frame update
    void Start()
    {
        this.SetHp(FirstHp);
        this.SetVelocity(FirstVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        //まだ体力がある
        if(this.GetHp() > 0)
        {
            //ある程度プレイヤーに近づくまでジリジリ近づく
            if(Mathf.Abs(GameObject.Find("Player").transform.position.x - transform.position.x) > 1)
                Idle();
            else if(!this.GetAttack())
            {
                SetAttack(true);

                //残り体力とか状況によって攻撃が変わる
                if(this.GetHp() == 3)
                    Attack1();
                else if(this.GetHp() == 2)
                    Attack2();
                else if(this.GetHp() == 1)
                    Attack3();
            }

            //攻撃にインターバルをつける。一定の時間が経つまでは攻撃できない。
            if(this.GetAttack())time++;
            if(time == timeInterval)
            {
                this.SetAttack(false);
                time = 0;
            }
        }
        //もう体力がない
        else 
        {
            //体力が無くなったら一回だけ処理する
            if(!this.GetDead())
            {
                this.SetDead(true);
                this.Die();
            }
        }
    }

    //通常状態
    public override void Idle()
    {

    }

    //攻撃1
    public override void Attack1()
    {

    }

    //攻撃2
    public override void Attack2()
    {

    }

    //攻撃3
    public override void Attack3()
    {

    }

    //ダメージ処理
    public override void Damage()
    {
        //ダメージを受けた時の処理(アニメーション再生とか)
    }

    public override void Die()
    {
        //死ぬ処理
    }
}
