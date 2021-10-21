using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスキャラクターの抽象クラス
public abstract class BossBase : MonoBehaviour
{
    protected int hp;                           //体力
    protected bool dead = false;                //既に死んだかの判定
    protected float velocity;                   //スピード
    protected bool attack = false;              //攻撃する判定
    public int time{get; protected set;}        //時間測定用
    public int timeInterval{get; protected set;}//インターバル用

    //通常状態の抽象メソッド
    public abstract void Idle();

    //移動する抽象メソッド
    public abstract void Move();

    //攻撃する抽象メソッド
    public abstract void Attack1();
    public abstract void Attack2();
    public abstract void Attack3();

    //攻撃を受けた時のメソッド
    public void BeAttacked()
    {
        //体力を減らしてからその後の処理をする
        this.hp--;
        this.time = 0;
        Debug.Log("hp:" + this.hp);
        if(this.hp > 0)
            Damage();
        else Die();
    }

    //ダメージを受けた時のメソッド
    public abstract void Damage();

    //死ぬ時の抽象メソッド
    public abstract void Die();

    //スピードを設定するメソッド
    public void SetVelocity(float vel){this.velocity = vel;}

    //攻撃したかを設定するメソッド
    public void SetAttack(bool attack){this.attack = attack;}

    //攻撃したかどうかを返すメソッド
    public bool GetAttack(){return this.attack;}
}
