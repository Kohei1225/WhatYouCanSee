using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスキャラクターの抽象クラス
public abstract class BossBase : MonoBehaviour
{
    protected int hp;                           //体力
    protected bool dead = false;                //既に死んだかの判定
    protected float moveSpeed;                  //スピード
    protected bool canAttack = true;              //攻撃する判定
    protected int dir = 1;                      //向いてる方向(-1:左向き,1:右向き)
    protected float bossSize;                   //体のサイズ
    protected bool hasStartBattle = false;
    protected Animator animController;                  //アニメーターを格納する変数

    public int attackTime{get; protected set;}          //時間測定(攻撃)用
    public int attackTimeInterval{get; protected set;}  //インターバル(攻撃)用
    public int damageTime{get; protected set;}          //時間測定(ダメージ)用
    public int damageTimeInterval{get; protected set;}  //インターバル(ダメージ)用。無敵時間
    

    //通常状態の抽象メソッド
    public abstract void Idle();

    //移動する抽象メソッド
    public abstract void Move();

    //攻撃する抽象メソッド
    public abstract void Attack1();
    public abstract void Attack2();
    public abstract void Attack3();

    //ダメージを受けた時のメソッド
    public abstract void Damage();

    //死ぬ時の抽象メソッド
    public abstract void Down();

    //攻撃を受けた時のメソッド
    public void BeAttacked()
    {
        //Debug.Log("BeAttacked()");
        //体力を減らしてからその後の処理をする
        this.hp--;
        this.damageTime = 0;
        //Debug.Log("hp:" + this.hp);
        if(this.hp > 0)
            Damage();
        else Down();
    }    

    //プレイヤーの方向を向くメソッド
    public void TurnToPlayer()
    {
        float playerXPos = GameObject.Find("Player").transform.position.x;
        float myXPos = transform.position.x;
        //プレイヤーが自分より左にいるときはdirを-1,右にいるときは1にする。
        if(playerXPos < myXPos)this.dir = 1;
        else if(myXPos < playerXPos)this.dir = -1;

        //dirの方向によって画像の向きを変える
        transform.localScale = new Vector3(dir * bossSize,bossSize,1);

        //Debug.Log("TurnToPlayer()");
    }
}
