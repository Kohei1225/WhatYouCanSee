using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスキャラクターの抽象クラス
public abstract class BossScript : MonoBehaviour
{
    int hp;             //体力
    bool dead = false;  //既に死んだかの判定
    float velocity;     //スピード
    bool attack = false;


    //通常状態の抽象メソッド
    public abstract void Idle();

    //攻撃する抽象メソッド
    public abstract void Attack1();

    public abstract void Attack2();

    public abstract void Attack3();

    //攻撃を受けた時のメソッド
    public void BeAttacked()
    {
        //体力を減らしてからその後の処理をする
        this.hp--;
        if(this.hp > 0)
            Damage();
        else Die();
    }

    //ダメージを受けた時のメソッド
    public abstract void Damage();

    //死ぬ時の抽象メソッド
    public abstract void Die();

    //体力を設定するメソッド
    public void SetHp(int n){this.hp = n;}

    //体力を返すメソッド
    public int GetHp(){return this.hp;}

    //スピードを設定するメソッド
    public void SetVelocity(float vel){this.velocity = vel;}

    //攻撃したかを設定するメソッド
    public void SetAttack(bool attack){this.attack = attack;}

    //攻撃したかどうかを返すメソッド
    public bool GetAttack(){return this.attack;}

    //既に死んだかを判定するメソッド
    public void SetDead(bool dead){this.dead = dead;}

    //既に死んだかを返すメソッド
    public bool GetDead(){return this.dead;}
}
