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
        WingAttack,
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
    [SerializeField] private GameObject _Beak = null;
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

    /// <summary> 中心からクチバシまでの長さ(比率的な長さ) </summary>
    private const float _BeakPoint = 5.4f;

    private float _WingAttackTime = 5.0f;
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
        _TaskList.DefineTask(TaskEnum.WingAttack, TaskWingAttackEnter, TaskWingAttackUpdate, TaskWingAttackExit);
        _TaskList.DefineTask(TaskEnum.LowFlyAttack, TaskLowFlyAttackEnter, TaskLowFlyAttackUpdate, TaskLowFlyAttackExit);
        _TaskList.DefineTask(TaskEnum.ReturnPostion, TaskReturnPosEnter, TaskReturnPosUpdate, TaskReturnPosExit);
        _TaskList.DefineTask(TaskEnum.Damage, TaskDamageEnter, TaskDamageUpdate, TaskDamageExit);
        _TaskList.DefineTask(TaskEnum.Wait, TaskWaitEnter, TaskWaitUpdate, TaskWaitExit);

        _WaitTime = 2.0f;
        _AttackInterval = 5.0f;
        _ChargeTime = 3.0f;

        _State = StateEnum.Move;
        _CurrentHP = _MaxHp;
        _BossSize = transform.localScale.x;

        _AnimController = GetComponent<Animator>();

        _AttackIntervalTimer.ResetTimer(_AttackInterval);
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
        //Idle();
        if(!CanAttack)
        {
            Debug.Log("Can't Attack!");
            return;
        }
        Attack1();
        Attack2();
        Attack3();
        //_TaskList.AddTask(TaskEnum.Wait);
        //Attack2();
        //_TaskList.AddTask(TaskEnum.Wait);
        //Attack3();
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
        _TaskList.AddTask(TaskEnum.Wait);
    }

    /// <summary> 羽攻撃 </summary>
    public override void Attack3()
    {
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.Charge);
        _TaskList.AddTask(TaskEnum.WingAttack);
        _TaskList.AddTask(TaskEnum.Wait);
    }

    public override void Damage()
    {
        _TaskList.CancelAllTask();
        _TaskList.AddTask(TaskEnum.Damage);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.Wait);
    }

    public override void Down()
    {
        _State = StateEnum.Dead;
        _TaskList.CancelAllTask();
        _AnimController.Play("Clow_Down", 0, 0);
        _Head.SetActive(false);
        _Beak.SetActive(false);
    }

    public override void Idle()
    {
        _TaskList.AddTask(TaskEnum.Idle);
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
    float _StageYPos = 0;
    void TaskFallAttackEnter()
    {
        var pos = transform.position;
        pos.x = _Player.transform.position.x;
        transform.position = pos;

        _AnimController.Play("Clow_FallAttack01",0,0);
        _AnimController.SetBool("IsFall", true);

        var dir = transform.position;
        dir.x = 0;
        dir.y = -1;
        
        foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position,dir))
        {
            if (hit.collider.gameObject.tag == "Stage")
            {
                _StageYPos = hit.point.y + (transform.localScale.y * _BeakPoint);
                break;
            }
        }
    }

    bool TaskFallAttackUpdate()
    {
        var fallSpeed = 1f;
        if(transform.position.y - fallSpeed < _StageYPos)
        {
            fallSpeed = transform.position.y - _StageYPos;
        }
        transform.Translate(0, -fallSpeed, 0);
        return _StageYPos >= transform.position.y ;
    }

    void TaskFallAttackExit()
    {
        _AnimController.SetBool("IsFallShock",true);
        //Debug.Log("落下攻撃完了!!");
    }
    #endregion

    #region FallAttack

    void TaskAfterFallEnter()
    {
        _Beak.SetActive(false);
        _WaitTimer.ResetTimer(_AfterFallTime);
    }

    bool TaskAfterFallUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskAfterFallExit()
    {
        _AnimController.SetBool("IsFall", false);
        _AnimController.SetBool("IsFallShock", false);
    }
    #endregion

    #region Charge

    int _FrameNumber = 0;
    float _Xpos = 0f;

    void TaskChargeEnter()
    {
        Debug.Log("start charge");
        _WaitTimer.ResetTimer(_ChargeTime);

        _FrameNumber = 0;
        _Xpos = transform.position.x;
        _AnimController.Play("Clow_Charge01",0,0);
    }

    bool TaskChargeUpdate()
    {
        var pos = transform.position;
        var dir = 1;

        //フレームの数に応じて振動する
        if ((_FrameNumber / 4) % 2 == 0)
        {
            dir = -1;
        }
        pos.x = _Xpos + dir * 0.25f;
        transform.position = pos;

        _FrameNumber++;
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskChargeExit()
    {
        Debug.Log("Finish Charge");
    }
    #endregion

    #region WingAttack
    void TaskWingAttackEnter()
    {
        _AnimController.SetBool("IsWing", true);
        _WaitTimer.ResetTimer(_WingAttackTime);
    }

    bool TaskWingAttackUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskWingAttackExit()
    {
        _AnimController.SetBool("IsWing",false);
        Debug.Log("Finish Wing");
    }
    #endregion

    #region LowFlyAttack
    GameObject _TargetObject = null;
    void TaskLowFlyAttackEnter()
    {
        _AnimController.Play("Clow_LowFlying01", 0, 0);
        _AnimController.SetBool("IsLowFly", true);

        //右にいたら左へ、左にいたら右へ移動する
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

        //var tmpY = Mathf.Lerp(0,1, _RightSetPos.transform.position.x - pos.x);
        var tmpX = (transform.position.x - _LeftSetPos.transform.position.x)/(_RightSetPos.transform.position.x - _LeftSetPos.transform.position.x);
        Debug.Log("tmp:" + tmpX);
        pos.y = _LowRoute.Evaluate(tmpX);
        pos.y *= _TargetObject.transform.position.y;
        
        transform.position = pos;
        return Mathf.Abs(_TargetObject.transform.position.x - transform.position.x) <= 1f;
    }

    void TaskLowFlyAttackExit()
    {
        _AnimController.SetBool("IsLowFly", false);
        TurnTo(_Player);
       // Debug.Log("Finish LowFly");
    }
    #endregion

    #region Return
    public Vector3 _Vec;
    void TaskReturnPosEnter()
    {
        
        //ステージの右側にいるか左側にいるかで対象を変える
        if(transform.position.x < 0)
        {
            _TargetObject = _LeftSetPos;
        }
        else _TargetObject = _RightSetPos;

        Debug.Log("I will return " + _TargetObject.name);
        //Debug.Log("I will return position");
        _Beak.SetActive(false);

        //対象へ向かう単位ベクトルを取得
        _Vec = _TargetObject.transform.position - gameObject.transform.position;
        _Vec.z = 0;
        _Vec /= CalcDistance(_TargetObject, gameObject);
    }

    bool TaskReturnPosUpdate()
    {
        TurnTo(_Player);

        if(_TargetObject.transform.position.y <= gameObject.transform.position.y)
        {
            return true;
        }

        //対象まで一定のスピードで移動
        transform.position += _Vec * _MoveSpeed;
        return false;
    }

    void TaskReturnPosExit()
    {
        TurnTo(_Player);

        //Debug.Log("移動終わり");
        _Head.SetActive(true);
        _Beak.SetActive(true);

        Debug.Log("Finish Return");
    }
    #endregion

    #region Damage
    void TaskDamageEnter()
    {
        _AnimController.Play("Clow_Damage01",0,0);
        _AnimController.SetBool("IsDamage", true);
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
        //Debug.Log("Finish wait");
    }
    #endregion

    #endregion
}
