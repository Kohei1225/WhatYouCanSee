using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClowScript : BossBase
{
    #region define
    /// <summary> タスクの定義 </summary>
    enum TaskEnum
    {
        Idle,
        Move,
        FallAttack,
        Charge,
        WinidAttack,
        LowFlyAttack,
        ReturnPostion,
        Damage,
        Wait,
    }
    #endregion

    #region serialize
    /// <summary> カラスの体力 </summary>
    [SerializeField] private int _MaxHp = 9;
    /// <summary> カラスの移動スピード </summary>
    [SerializeField] private float _MoveSpeed = 0.075f;
    /// <summary> プレイヤーのオブジェクト </summary>
    [SerializeField] private GameObject _Player = null;
    /// <summary> 右側の固定位置 </summary>
    [SerializeField] private GameObject _RightSetPos = null;
    /// <summary> 左側の固定位置 </summary>
    [SerializeField] private GameObject _LeftSetPos = null;
    /// <summary> 飛ばす羽のプレハブ </summary>
    [SerializeField] private GameObject _WindPrefab = null;
    #endregion

    #region field
    /// <summary> 攻撃した回数 </summary>
    private int _AttackCounter = 0;
    /// <summary> いろんな時間計測用インスタンス </summary>
    TimerScript _Timer = new TimerScript();
    /// <summary> 飛び上がってから落ちてくるまでの時間 </summary>
    private float _FlyUpTime = 1f;
    /// <summary> 落ちてくる攻撃後の隙の時間 </summary>
    private float _FallAttackTime = 2.0f;
    /// <summary> 溜める時間 </summary>
    private float _ChargeTime = 2.0f;

    /// <summary> タスクリストのインスタンス </summary>
    private TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary> 羽攻撃 </summary>
    public override void Attack1()
    {

    }

    /// <summary> 低空飛行攻撃 </summary>
    public override void Attack2()
    {
        
    }

    /// <summary> 上から落ちてくる攻撃 </summary>
    public override void Attack3()
    {
        
    }

    public override void Damage()
    {
        
    }

    public override void Down()
    {
        throw new System.NotImplementedException();
    }

    public override void Idle()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void Wait(float waitTime)
    {
        throw new System.NotImplementedException();
    }

    #region Task function
    
    #endregion
}
