using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClowScript : BossBase
{
    #region define
    /// <summary> タスクの定義 </summary>
    public enum TaskEnum
    {
        Idle,
        Move,
        FlyUp,
        BeforeFall,
        FallAttack,
        AfterFall,
        Charge,
        WingAttack,
        AfterWing,
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
    [SerializeField] private float _MoveSpeed = 60f;
    /// <summary> プレイヤーのオブジェクト </summary>
    [SerializeField] private GameObject _Player = null;
    /// <summary> 右側の固定位置 </summary>
    [SerializeField] private GameObject _RightSetPos = null;
    /// <summary> 左側の固定位置 </summary>
    [SerializeField] private GameObject _LeftSetPos = null;
    /// <summary> 飛ばす羽のプレハブ </summary>
    [SerializeField] private GameObject _WingPrefab = null;
    /// <summary> 低空飛行の際の軌道 </summary>
    [SerializeField] private AnimationCurve[] _LowRoutes;
    /// <summary> 頭の当たり判定を含んだオブジェクト </summary>
    [SerializeField] private GameObject _Head = null;
    /// <summary> クチバシの当たり判定を含んだオブジェクト </summary>
    [SerializeField] private GameObject _Beak = null;
    /// <summary> 低空飛行をする際のスピード </summary>
    [SerializeField] private float[] _LowSpeeds;
    [SerializeField] private WingGenerator _WingGenerator = null;

    /// <summary> 真ん中で回転してるミラーボールの子オブジェクト </summary>
    [SerializeField] private GameObject _MirrorBallChild = null;

    /// <summary> ゲームマネージャー </summary>
    [SerializeField] private GameManagerScript _GameManager = null;

    /// <summary> ライトの色の配列 </summary>
    [SerializeField] private ColorObjectVer3[] _MirrorLightColorObjects = null;

    /// <summary> 光源のオブジェクト </summary>
    [SerializeField] private GameObject _SingleLightObject = null;
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

    /// <summary> 見た目の状態 </summary>
    private int _VisualState = 1;

    /// <summary> 見た目の状態の文字列 </summary>
    private string _VisualStateString = "01";

    /// <summary> 一定の間隔でミラーボールの光を回すクラス </summary>
    private IntervalRotate _IntervalRotate = null;

    /// <summary> 羽の色の候補 </summary>
    private ColorObjectVer3.OBJECT_COLOR3[,] _WingColors = new ColorObjectVer3.OBJECT_COLOR3[,]
    {

        {
            ColorObjectVer3.OBJECT_COLOR3.RED,ColorObjectVer3.OBJECT_COLOR3.RED,ColorObjectVer3.OBJECT_COLOR3.RED,
            ColorObjectVer3.OBJECT_COLOR3.ORRANGE,ColorObjectVer3.OBJECT_COLOR3.ORRANGE,ColorObjectVer3.OBJECT_COLOR3.ORRANGE,
        },
        {
            ColorObjectVer3.OBJECT_COLOR3.YELLOW,ColorObjectVer3.OBJECT_COLOR3.YELLOW,ColorObjectVer3.OBJECT_COLOR3.YELLOW,
            ColorObjectVer3.OBJECT_COLOR3.LIME,ColorObjectVer3.OBJECT_COLOR3.LIME,ColorObjectVer3.OBJECT_COLOR3.LIME,
        },
        {
            ColorObjectVer3.OBJECT_COLOR3.BLUE,ColorObjectVer3.OBJECT_COLOR3.PURPLE,ColorObjectVer3.OBJECT_COLOR3.MAGENTA,
            ColorObjectVer3.OBJECT_COLOR3.BLUE,ColorObjectVer3.OBJECT_COLOR3.PURPLE,ColorObjectVer3.OBJECT_COLOR3.MAGENTA,
        },
        {
            ColorObjectVer3.OBJECT_COLOR3.GRAY,ColorObjectVer3.OBJECT_COLOR3.GRAY,ColorObjectVer3.OBJECT_COLOR3.GRAY,
            ColorObjectVer3.OBJECT_COLOR3.WHITE,ColorObjectVer3.OBJECT_COLOR3.WHITE,ColorObjectVer3.OBJECT_COLOR3.WHITE,
        },
        {
            ColorObjectVer3.OBJECT_COLOR3.BLACK,ColorObjectVer3.OBJECT_COLOR3.BLACK,ColorObjectVer3.OBJECT_COLOR3.BLACK,
            ColorObjectVer3.OBJECT_COLOR3.BLACK,ColorObjectVer3.OBJECT_COLOR3.BLACK,ColorObjectVer3.OBJECT_COLOR3.BLACK,
        },
    };


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
        _TaskList.DefineTask(TaskEnum.BeforeFall, TaskBeforeFallEnter, TaskBeforeFallUpdate, TaskBeforeFallExit);
        _TaskList.DefineTask(TaskEnum.FallAttack, TaskFallAttackEnter, TaskFallAttackUpdate, TaskFallAttackExit);
        _TaskList.DefineTask(TaskEnum.AfterFall, TaskAfterFallEnter, TaskAfterFallUpdate, TaskAfterFallExit);
        _TaskList.DefineTask(TaskEnum.Charge, TaskChargeEnter, TaskChargeUpdate, TaskChargeExit);
        _TaskList.DefineTask(TaskEnum.WingAttack, TaskWingAttackEnter, TaskWingAttackUpdate, TaskWingAttackExit);
        _TaskList.DefineTask(TaskEnum.AfterWing, TaskAfterWingEnter, TaskAfterWingUpdate, TaskAfterWingExit);
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

        ChangeLightColors(_VisualState);
        ChangeLight(true);

        _IntervalRotate = _MirrorBallChild?.GetComponent<IntervalRotate>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateState();
    }

    private bool _HasOnGround = false;
    void UpdateState()
    {
        if(_State == StateEnum.Dead)
        {
            //

            if(_HasOnGround)
            {
                return;
            }

            if(OnGround)
            {
                _HasOnGround = true;
                return;
            }

            transform.Translate(0, -1.5f*Time.deltaTime, 0);
        }


        if(_State == StateEnum.Move)
        {
            if(_CurrentHP <= 0)
            {
                _State = StateEnum.Dead;
                //音
                SoundManager.Instance.PlaySE("Damage_1");
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
        //Debug.Log("SelectTask");
        _AttackIntervalTimer.UpdateTimer();
        //Idle();
        if(!CanAttack)
        {
            //Debug.Log("Can't Attack!");
            return;
        }

        Attack2();
        Attack3();
        Attack1();

        //Attack3();
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
        _TaskList.AddTask(TaskEnum.BeforeFall);
        _TaskList.AddTask(TaskEnum.FallAttack);
        _TaskList.AddTask(TaskEnum.AfterFall);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
    }

    /// <summary> 低空飛行攻撃 </summary>
    public override void Attack2()
    {
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.Wait);
        _TaskList.AddTask(TaskEnum.LowFlyAttack);
    }

    /// <summary> 羽攻撃 </summary>
    public override void Attack3()
    {
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.Charge);
        _TaskList.AddTask(TaskEnum.WingAttack);
        _TaskList.AddTask(TaskEnum.AfterWing);
    }

    public override void Damage()
    {
        _TaskList.CancelAllTask();
        _TaskList.AddTask(TaskEnum.Damage);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.Wait);
        //_GameManager.existRay = false;

        if (_CurrentHP == 1)
        {
            _VisualState = 5;

            //最後はずっと色が変わり続ける
            for (int i = 0; i < _MirrorLightColorObjects.Length; i++)
            {
                var changeColorByTime = _MirrorLightColorObjects[i].gameObject.GetComponent<ChangeColorByTime>();

                changeColorByTime._CanChangeColor = true;
            }

        }
        else if (_CurrentHP <= 3)
        {
            _VisualState = 4;
        }
        else if (_CurrentHP <= 5)
        {
            _VisualState = 3;
        }
        else if (_CurrentHP <= 7)
        {
            _VisualState = 2;
        }
        else
        {
            _VisualState = 1;
        }

        _VisualStateString = "0" + _VisualState.ToString();

        ChangeLightColors(_VisualState);
        ChangeLight(true);
    }

    public override void Down()
    {
        _State = StateEnum.Dead;
        _TaskList.CancelAllTask();
        _AnimController.Play("Clow_Down", 0, 0);
        _Head.SetActive(false);
        _Beak.SetActive(false);

        //ライトを切り替える
        ChangeLight(true);
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


    /// <summary> ライトを切り替える </summary>
    /// <param name="isSingleLight">ミラーボールのライトがONかどうか</param>
    private void ChangeLight(bool isMirrorLight)
    {
        //それぞれのライトのSetActiveを切り替える
        _SingleLightObject.SetActive(!isMirrorLight);

        _MirrorBallChild.SetActive(isMirrorLight);

    }

    /// <summary> ミラーボールの光の色を変更する </summary>
    /// <param name="visualState"></param>
    private void ChangeLightColors(int visualState)
    {
        if(visualState == 5)
        {
            return;
        }

        visualState--;
        //カラスが出す色の候補とハズレ色を用意
        var mainColor1 = _WingColors[visualState,0];
        var mainColor2 = _WingColors[visualState,1];
        var mainColor3 = _WingColors[visualState,5];
        var badColor = ColorObjectVer3.OBJECT_COLOR3.CYAN;

        for(int i = 0; i < _MirrorLightColorObjects.Length; i++)
        {
            var objColor = badColor;

            if(i == 0 || i == 4)
            {
                objColor = mainColor1;
            }

            else if(i == 1 || i == 5)
            {
                objColor = mainColor3;
            }

            else if(i == 2 || i == 6)
            {
                objColor = mainColor2;
            }
           
            _MirrorLightColorObjects[i].colorType = objColor;
        }

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
        //ライトを消して部屋を暗くする
        _MirrorBallChild.SetActive(false);
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

    #region BeforeFallAttack
    float _StageYPos = 0;
    void TaskBeforeFallEnter()
    {
        // スポットライトをONにする(落ちる場所を予測)
        ChangeLight(false);

        //プレイヤーの位置に合わせて移動
        var pos = transform.position;
        pos.x = _Player.transform.position.x;
        transform.position = pos;

        //スポットライトの位置を調整
        pos.y = _SingleLightObject.transform.position.y;
        _SingleLightObject.transform.position = pos;

        //下方向にRayを飛ばして距離を計算する
        var dir = transform.position;
        dir.x = 0;
        dir.y = -1;

        foreach (RaycastHit2D hit in Physics2D.RaycastAll(transform.position, dir))
        {
            if (hit.collider.gameObject.tag == "Stage")
            {
                _StageYPos = hit.point.y + (transform.localScale.y * _BeakPoint);
                break;
            }
        }

        _WaitTimer.ResetTimer(2.0f);
    }

    bool TaskBeforeFallUpdate()
    {
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskBeforeFallExit()
    {

    }
    #endregion

    #region FallAttack

    void TaskFallAttackEnter()
    {
        //
        _AnimController.Play("Clow_FallAttack" + _VisualStateString,0,0);
        _AnimController.SetBool("IsFall", true);
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

        //ミラーボールをセットする
        ChangeLight(true);
    }
    #endregion

    #region Charge

    int _FrameNumber = 0;
    float _Xpos = 0f;

    void TaskChargeEnter()
    {
        //Debug.Log("start charge");
        _WaitTimer.ResetTimer(_ChargeTime);

        _FrameNumber = 0;
        _Xpos = transform.position.x;
        _AnimController.Play("Clow_Charge" + _VisualStateString,0,0);
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
        //Debug.Log("Finish Charge");
    }
    #endregion

    #region WingAttack

    private int _WingColorNum;
    void TaskWingAttackEnter()
    {
        ChangeLight(true);
        ChangeLightColors(_VisualState);
        _AnimController.SetBool("IsWing", true);
        _WaitTimer.ResetTimer(_WingAttackTime);

        _WingColorNum = Random.Range(0, 5);

        if(_IntervalRotate != null)
        {
            _IntervalRotate._CanRotate = false;
        }
        
    }

    bool TaskWingAttackUpdate()
    {
        TurnTo(_Player);
        _WaitTimer.UpdateTimer();
        _WingGenerator.ShotWing(_WingPrefab,_Dir,_WingColors[_VisualState - 1,_WingColorNum]);
        _IntervalRotate._CanRotate = false;
        return _WaitTimer.IsTimeUp;
    }

    void TaskWingAttackExit()
    {
        _IntervalRotate._CanRotate = true;

        _AnimController.SetBool("IsWing",false);
        //Debug.Log("Finish Wing");
        _WingGenerator.ResetPeriodCounter();
    }
    #endregion

    #region WingAttack
    void TaskAfterWingEnter()
    {
        _WaitTimer.ResetTimer(3.0f);
    }

    bool TaskAfterWingUpdate()
    {
        _IntervalRotate._CanRotate = false;
        _WaitTimer.UpdateTimer();
        return _WaitTimer.IsTimeUp;
    }

    void TaskAfterWingExit()
    {

    }
    #endregion

    #region LowFlyAttack
    GameObject _TargetObject = null;
    int _CurrentLowNumber;
    void TaskLowFlyAttackEnter()
    {
        _AnimController.Play("Clow_LowFlying" + _VisualStateString, 0, 0);
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

        _CurrentLowNumber = Random.Range(0,_LowRoutes.Length-1);
    }

    bool TaskLowFlyAttackUpdate()
    {
        var pos = transform.position;
        pos.x += _LowSpeeds[_CurrentLowNumber] * _Dir * Time.deltaTime;

        //var tmpY = Mathf.Lerp(0,1, _RightSetPos.transform.position.x - pos.x);
        //現在地の割合を計算
        var tmpX = (transform.position.x - _LeftSetPos.transform.position.x)/(_RightSetPos.transform.position.x - _LeftSetPos.transform.position.x);
        //Debug.Log("tmp:" + tmpX);
        pos.y = _LowRoutes[_CurrentLowNumber].Evaluate(tmpX);

        pos.y *= _TargetObject.transform.position.y;


        var unit = 2f;
        var targetYPos = _TargetObject.transform.position.y;
        pos.y *= unit;
        targetYPos *= unit;
        targetYPos -= targetYPos * ((unit - 1)/unit);
        pos.y -= targetYPos;
        
        transform.position = pos;

        //対象を超えたかどうかを返す
        if(_Dir < 0)
        {
            return transform.position.x <= _TargetObject.transform.position.x;
        }


        return _TargetObject.transform.position.x <= transform.position.x;
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

        //Debug.Log("I will return " + _TargetObject.name);
        //Debug.Log("I will return position");
        _Beak.SetActive(false);

        //対象へ向かう単位ベクトルを取得
        _Vec = _TargetObject.transform.position - gameObject.transform.position;
        _Vec.z = 0;
        _Vec /= CalcDistance(_TargetObject, gameObject);

        TurnTo(_TargetObject);
    }

    bool TaskReturnPosUpdate()
    {
        if((_TargetObject.transform.position.x <= gameObject.transform.position.x && 0 < _Dir)
            ||(gameObject.transform.position.y <= _TargetObject.transform.position.x && _Dir < 0 )
            || _TargetObject.transform.position.y <= gameObject.transform.position.y)
        {
            return true;
        }

        //対象まで一定のスピードで移動
        transform.position += _Vec * _MoveSpeed * Time.deltaTime;
        return false;
    }

    void TaskReturnPosExit()
    {
        TurnTo(_Player);

        //Debug.Log("移動終わり");
        _Head.SetActive(true);
        _Beak.SetActive(true);

        //Debug.Log("Finish Return");
    }
    #endregion

    #region Damage
    void TaskDamageEnter()
    {
        _AnimController.Play("Clow_Damage" + _VisualStateString,0,0);
        _AnimController.SetBool("IsDamage", true);
        _Head.SetActive(false);
        _Beak.SetActive(false);
        _WaitTimer.ResetTimer(1.0f);
        _IntervalRotate._CanRotate = true;
        //音
        SoundManager.Instance.PlaySE("Damage_2");
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
