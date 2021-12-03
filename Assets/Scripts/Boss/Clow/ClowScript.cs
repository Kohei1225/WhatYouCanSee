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
        FlyUp,
        FallAttack,
        Charge,
        WindAttack,
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
    [SerializeField] private AnimationCurve _LowRoute;
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

    private TaskEnum[] array;
    #endregion

    #region property
    /// <summary> 地面に接してるか </summary>
    private bool OnGround
    {
        get { return transform.Find("Foot").gameObject.GetComponent<FootAreaScript>().touchingStage; }
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _TaskList.DefineTask(TaskEnum.Idle, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
        _TaskList.DefineTask(TaskEnum.Move, TaskMoveEnter, TaskMoveUpdate, TaskMoveExit);
        _TaskList.DefineTask(TaskEnum.FlyUp, TaskFlyUpEnter, TaskFlyUpUpdate, TaskFlyUpExit);
        _TaskList.DefineTask(TaskEnum.FallAttack, TaskFallAttackEnter, TaskFallAttackUpdate, TaskFallAttackExit);
        _TaskList.DefineTask(TaskEnum.Charge, TaskChargeEnter, TaskChargeUpdate, TaskChargeExit);
        _TaskList.DefineTask(TaskEnum.WindAttack, TaskWindAttackEnter, TaskWindAttackUpdate, TaskWindAttackExit);
        _TaskList.DefineTask(TaskEnum.LowFlyAttack, TaskLowFlyAttackEnter, TaskLowFlyAttackUpdate, TaskLowFlyAttackExit);
        _TaskList.DefineTask(TaskEnum.ReturnPostion, TaskReturnPosEnter, TaskReturnPosUpdate, TaskReturnPosExit);
        _TaskList.DefineTask(TaskEnum.Damage, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
        _TaskList.DefineTask(TaskEnum.Wait, TaskWaitEnter, TaskWaitUpdate, TaskWaitExit);

        _WaitTime = 2.0f;
        _AttackInterval = 5.0f;

        _State = StateEnum.Move;
        _CurrentHP = _MaxHp;
        _BossSize = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }


    void UpdateState()
    {
        if(_State == StateEnum.Move)
        {
            if(_CurrentHP <= 0)
            {
                _State = StateEnum.Dead;
                return;
            }


            if(_TaskList.IsEnd)
            {
                PlayerController player = _Player?.GetComponent<PlayerController>();
                if(player.damage)
                {
                    _State = StateEnum.None;
                    return;
                }
                //タスクを追加
                SelectTask();
            }
            //タスクの処理を更新
            _TaskList.UpdateTask();
        }
    }

    /// <summary> タスクを追加する </summary>
    void SelectTask()
    {
        Debug.Log("SelectTask");
        _AttackIntervalTimer.UpdateTimer();
        if(!CanAttack)
        {
            Debug.Log("Cant Attack!");
            return;
        }
        Attack1();
        Attack2();
        Attack3();
    }

    /// <summary> 上から落ちてくる攻撃 </summary>
    public override void Attack1()
    {
        _TaskList.AddTask(TaskEnum.FlyUp);
        _TaskList.AddTask(TaskEnum.Wait);
        _TaskList.AddTask(TaskEnum.FallAttack);
        _TaskList.AddTask(TaskEnum.Wait);
    }

    /// <summary> 低空飛行攻撃 </summary>
    public override void Attack2()
    {
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.LowFlyAttack);
    }

    /// <summary> 羽攻撃 </summary>
    public override void Attack3()
    {
        _TaskList.AddTask(TaskEnum.Charge);
        _TaskList.AddTask(TaskEnum.WindAttack);
        _TaskList.AddTask(TaskEnum.Wait);
    }

    public override void Damage()
    {
        
    }

    public override void Down()
    {
        
    }

    public override void Idle()
    {
        
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

    #region Idle
    void TaskIdleEnter()
    {

    }

    bool TaskIdleUpdate()
    {
        return true;
    }

    void TaskIdleExit()
    {

    }
    #endregion

    #region Move
    void TaskMoveEnter()
    {

    }

    bool TaskMoveUpdate()
    {
        return true;
    }

    void TaskMoveExit()
    {

    }
    #endregion

    #region FlyUp
    void TaskFlyUpEnter()
    {

    }

    bool TaskFlyUpUpdate()
    {
        transform.Translate(0,1,0);

        return transform.position.y >= 40;
    }

    void TaskFlyUpExit()
    {

    }
    #endregion

    #region FallAttack
    void TaskFallAttackEnter()
    {
        var pos = transform.position;
        pos.x = _Player.transform.position.x;
        transform.position = pos;
    }

    bool TaskFallAttackUpdate()
    {
        transform.Translate(0, -1, 0);
        return OnGround;
    }

    void TaskFallAttackExit()
    {
        Debug.Log("落下攻撃完了!!");
    }
    #endregion

    #region Charge
    void TaskChargeEnter()
    {
        _WaitTimer.ResetTimer(_ChargeTime);
    }

    bool TaskChargeUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskChargeExit()
    {

    }
    #endregion

    #region WindAttack
    void TaskWindAttackEnter()
    {

    }

    bool TaskWindAttackUpdate()
    {
        return true;
    }

    void TaskWindAttackExit()
    {
        Debug.Log("羽攻撃完了!!");
    }
    #endregion

    #region LowFlyAttack
    GameObject _TargetObject = null;
    void TaskLowFlyAttackEnter()
    {
        //_LowAttackDir = _Player.transform.position - transform.position;
        if (CalcDistance(_RightSetPos,gameObject) < 1)
        {
            _TargetObject = _LeftSetPos;
        }
        else
        {
            _TargetObject = _RightSetPos;
        }
        TurnTo(_TargetObject);
    }

    bool TaskLowFlyAttackUpdate()
    {
        var pos = transform.position;
        pos.x += _Dir;
        pos.y = _LowRoute.Evaluate(pos.x) * 30;
        transform.position = pos;
        return CalcDistance(_TargetObject, gameObject) < 0.1f;
        //return Mathf.Abs(transform.position.x - _TargetObject.transform.position.x) < 0.1f;
    }

    void TaskLowFlyAttackExit()
    {
        Debug.Log("低空飛行攻撃完了!!");
    }
    #endregion

    #region Return
    void TaskReturnPosEnter()
    {
        if (_Dir < 0)
        {
            _TargetObject = _LeftSetPos;
        }
        else _TargetObject = _RightSetPos;
    }

    bool TaskReturnPosUpdate()
    {
        var vec = _TargetObject.transform.position - transform.position;
        transform.position += vec * 0.1f;
        return CalcDistance(_TargetObject,gameObject) < 0.1f;
    }

    void TaskReturnPosExit()
    {

    }
    #endregion

    #region Damage
    void TaskDamageEnter()
    {

    }

    bool TaskDamageUpdate()
    {
        return true;
    }

    void TaskDamageExit()
    {

    }
    #endregion

    #region Wait
    void TaskWaitEnter()
    {
        _WaitTimer.ResetTimer(_WaitTime);
    }

    bool TaskWaitUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskWaitExit()
    {
        //特になし
    }
    #endregion

    #endregion
}
