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
        AfterFall,
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
    /// <summary> 低空飛行の際の軌道 </summary>
    [SerializeField] private AnimationCurve _LowRoute;
    /// <summary> 頭の当たり判定を含んだオブジェクト </summary>
    [SerializeField] private GameObject _Head = null;
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
    /// <summary> 落下攻撃後の隙の時間 </summary>
    private float _AfterFallTime = 4.0f;

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
        _TaskList.DefineTask(TaskEnum.AfterFall, TaskAfterFallEnter, TaskAfterFallUpdate, TaskAfterFallExit);
        _TaskList.DefineTask(TaskEnum.Charge, TaskChargeEnter, TaskChargeUpdate, TaskChargeExit);
        _TaskList.DefineTask(TaskEnum.WindAttack, TaskWindAttackEnter, TaskWindAttackUpdate, TaskWindAttackExit);
        _TaskList.DefineTask(TaskEnum.LowFlyAttack, TaskLowFlyAttackEnter, TaskLowFlyAttackUpdate, TaskLowFlyAttackExit);
        _TaskList.DefineTask(TaskEnum.ReturnPostion, TaskReturnPosEnter, TaskReturnPosUpdate, TaskReturnPosExit);
        _TaskList.DefineTask(TaskEnum.Damage, TaskDamageEnter, TaskDamageUpdate, TaskDamageExit);
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
        _TaskList.AddTask(TaskEnum.Wait);
        Attack2();
        _TaskList.AddTask(TaskEnum.Wait);
        Attack3();
    }

    /// <summary> 上から落ちてくる攻撃 </summary>
    public override void Attack1()
    {
        _TaskList.AddTask(TaskEnum.FlyUp);
        _TaskList.AddTask(TaskEnum.Wait);
        _TaskList.AddTask(TaskEnum.FallAttack);
        _TaskList.AddTask(TaskEnum.AfterFall);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
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
        _TaskList.CancelAllTask();
        _TaskList.AddTask(TaskEnum.Damage);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
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
        var speed = -1;
        transform.Translate(0, -1, 0);
        return OnGround;
    }

    void TaskFallAttackExit()
    {
        Debug.Log("落下攻撃完了!!");
    }
    #endregion

    #region FallAttack
    void TaskAfterFallEnter()
    {
        _WaitTimer.ResetTimer(_AfterFallTime);
    }

    bool TaskAfterFallUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskAfterFallExit()
    {
    }
    #endregion

    #region Charge
    void TaskChargeEnter()
    {
        Debug.Log("Charge");
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
       // Debug.Log("低空飛行攻撃完了!!");
    }
    #endregion

    #region Return
    void TaskReturnPosEnter()
    {
        //ステージの右側にいるか左側にいるかで対象を変える
        if(transform.position.x < 0)
        {
            _TargetObject = _LeftSetPos;
        }
        else _TargetObject = _RightSetPos;

        Debug.Log("I will return position");
    }

    bool TaskReturnPosUpdate()
    {
        TurnTo(_Player);

        //対象に向かうベクトルを取得
        var vec = transform.position;
        vec.x = _TargetObject.transform.position.x - transform.position.x;
        vec.y = _TargetObject.transform.position.y - transform.position.y;
        vec.z = 0;

        //ベクトルを単位ベクトル化
        vec /= CalcDistance(_TargetObject, gameObject);

        //対象まで一定のスピードで移動
        transform.position += vec * _MoveSpeed;

        return CalcDistance(_TargetObject,gameObject) < 0.5f;
    }

    void TaskReturnPosExit()
    {
        _Head.SetActive(true);
    }
    #endregion

    #region Damage
    void TaskDamageEnter()
    {
        _AnimController.Play("Damage");
        _Head.SetActive(false);
        _WaitTimer.ResetTimer(1.0f);
    }

    bool TaskDamageUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskDamageExit()
    {
        _AnimController.SetBool("IsDamage", false);
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
        Debug.Log("HasWait");
    }
    #endregion

    #endregion
}
