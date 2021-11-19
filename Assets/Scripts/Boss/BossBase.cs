using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ボスキャラクターの抽象クラス
public abstract class BossBase : MonoBehaviour
{

    #region define
    /// <summary> ボスの状態 </summary>
    protected enum StateEnum
    {
        /// <summary>何もしない状態</summary>
        None,
        /// <summary>動ける状態</summary>
        Move,
        /// <summary>死んだ状態</summary>
        Dead,
    }
    #endregion

    #region field
    /// <summary> ボスの状態 </summary>
    protected StateEnum _State = StateEnum.None;
    /// <summary> 体力 </summary>
    protected int _CurrentHP;
    /// <summary> 既に死んだかの判定 </summary>
    protected bool _IsDead = false;
    /// <summary> スピード </summary>
    protected float _MoveSpeed;
    /// <summary> 攻撃する判定 </summary>
    protected bool _CanAttack = true;
    /// <summary> 向いてる方向(-1:左向き,1:右向き) </summary>
    protected int _Dir = 1;
    /// <summary> 体のサイズ </summary>
    protected float _BossSize;
    
    protected bool _HasStartBattle = false;
    /// <summary> アニメーターを格納する変数 </summary>
    protected Animator _AnimController;


    /// <summary> 攻撃インターバル </summary>
    protected float _AttackInterval;
    /// <summary> 攻撃のインターバル測定用 </summary>
    protected TimerScript _AttackIntervalTimer = new TimerScript();

    /// <summary> 時間測定(ダメージ)用 </summary>
    public float _DamageTime{get; protected set;}          
    /// <summary> インターバル(ダメージ)用。無敵時間 </summary>
    public float _DamageTimeInterval{get; protected set;}
    /// <summary> 攻撃を受けない状態か </summary>
    public bool _IsUnableBeAttacked { get; protected set; }

    /// <summary> 何もしない時間の測定用 </summary>
    protected TimerScript _WaitTimer = new TimerScript();
    /// <summary> 何もしない時間 </summary>
    protected float _WaitTime = 1.0f;
    #endregion

    #region property
    /// <summary> ダメージ中かどうか </summary>
    public bool InDamageInterval
    {
        get { return _DamageTime < _DamageTimeInterval; }
    }

    /// <summary> 次の攻撃ができるか </summary>
    public bool CanAttack
    {
        get { return _AttackIntervalTimer.IsTimeUp; }
    }
    #endregion

    #region abstract function

    /// <summary> 通常状態 </summary>
    public abstract void Idle();
    /// <summary> 移動 </summary>
    public abstract void Move();
    /// <summary> 攻撃１ </summary>
    public abstract void Attack1();
    /// <summary> 攻撃２ </summary>
    public abstract void Attack2();
    /// <summary> 攻撃３ </summary>
    public abstract void Attack3();
    /// <summary> ダメージ </summary>
    public abstract void Damage();
    /// <summary> 任意の時間何もしない </summary>
    /// <param name="waitTime">待つ時間</param>
    public abstract void Wait(float waitTime);
    /// <summary> ダウン </summary>
    public abstract void Down();
    #endregion

    #region public function
    /// <summary> 攻撃を受けた時 </summary>
    public void BeAttacked()
    {
        //Debug.Log("BeAttacked()");
        //体力を減らしてからその後の処理をする
        this._CurrentHP--;
        this._DamageTime = 0;
        if(this._CurrentHP > 0)
            Damage();
        else Down();
    }


    /// <summary> プレイヤーの方向を向く </summary>
    public void TurnTo(GameObject obj)
    {
        var objXPos = obj?.transform.position.x;
        var myXPos = transform.position.x;
        //プレイヤーが自分より左にいるときはdirを-1,右にいるときは1にする。
        if (objXPos < myXPos) _Dir = -1;
        else if (myXPos < objXPos) _Dir = 1;

        //dirの方向によって画像の向きを変える
        transform.localScale = new Vector3(-_Dir * _BossSize, _BossSize, 1);

        //Debug.Log("TurnToPlayer()");
    }
    #endregion
}
