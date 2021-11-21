using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PandaScript : BossBase
{
    #region define
    /// <summary> タスクの定義 </summary>
    enum TaskEnum
    {
        Idle,
        Walk,
        SwingUp,    //攻撃前
        SwingDown,  //攻撃後
        Kick,
        Charge,
        SuperKick,
        ReturnPostion,
        Damage,
        Defend,
        Wait,
    }
    #endregion

    #region serialize
    /// <summary> パンダの体力 </summary>
    [SerializeField] private int _MaxHp = 9;       
    /// <summary> パンダの移動スピード </summary>
    [SerializeField] private float _MoveSpeed = 0.075f;  
    /// <summary> 背景のスクリプト </summary>
    [SerializeField]private ColorObjectVer3 _BackColorScript = null;
    /// <summary> レバーのオブジェクト </summary>
    [SerializeField] private LeverScript _LeverSwitch; //
    /// <summary> プレイヤーのオブジェクト </summary>
    [SerializeField] private GameObject _Player = null;
    /// <summary> 右側の固定位置 </summary>
    [SerializeField] private GameObject _RightSetPos = null;
    /// <summary> 左側の固定位置 </summary>
    [SerializeField] private GameObject _LeftSetPos = null;
    /// <summary> 飛び蹴りベクトル </summary>
    [SerializeField] private Vector2 _kickVel = new Vector2(25f, 20f);
    #endregion

    #region field
    /// <summary> バトルを開始する距離 </summary>
    private const float BATTLE_START_DISTANCE = 50f; 
    /// <summary> 攻撃１をする距離 </summary>
    private const float DISTANCE1 = 7.5f;             
    /// <summary> 攻撃２をする距離 </summary>
    private const float DISTANCE2 = 25f;             
    /// <summary>  </summary>
    private bool hasKick = false;
    /// <summary> 身体の黒い部分(主に攻撃判定) </summary>
    private GameObject _BlackBody = null;                   
    /// <summary> 身体の白い部分 </summary>
    private GameObject _WhiteBody = null;                   
          
    
    /// <summary> 攻撃した回数 </summary>
    private int _AttackCounter = 0;
    /// <summary> いろんな時間計測用インスタンス </summary>
    TimerScript _Timer = new TimerScript();
    /// <summary> 爪攻撃のスピード </summary>
    private float _SwingUpTime = 1f;
    /// <summary> 攻撃後の隙の時間 </summary>
    private float _SwingDownTime = 2.0f;
    /// <summary> 防御する時間 </summary>
    private float _DefendTime = 2.0f;
    /// <summary> 溜める時間 </summary>
    private float _ChargeTime = 2.0f;

    /// <summary> タスクリストのインスタンス </summary>
    private TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();
    #endregion

    #region property
    /// <summary> X軸上でのプレイヤーとの距離 </summary>
    public float Distance
    {
        get { return Mathf.Abs(transform.position.x - _Player.transform.position.x); }
    }
    /// <summary> 地面に接してるか </summary>
    private bool OnGround
    {
        get { return transform.Find("Foot").gameObject.GetComponent<FootAreaScript>().touchingStage; }
    }

    /// <summary> プレイヤーが見えなくなってるか </summary>
    private bool IsNoPlayer
    {
        get { return !_Player.transform.Find("Body").gameObject.activeSelf; }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _CurrentHP = _MaxHp;
        _AttackInterval = 2;
        _DamageTimeInterval = 1;
        _DamageTime = _DamageTimeInterval;
        _AnimController = GetComponent<Animator>();
        _BossSize = transform.localScale.x;
        _BlackBody = transform.Find("Black").gameObject;
        _WhiteBody = transform.Find("White").gameObject;
        _BlackBody.SetActive(false);


        //タスクの登録
        _TaskList.DefineTask(TaskEnum.Idle, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
        _TaskList.DefineTask(TaskEnum.Walk, TaskWalkEnter, TaskWalkUpdate, TaskWalkExit);
        _TaskList.DefineTask(TaskEnum.SwingUp, TaskSwingUpEnter, TaskSwingUpUpdate, TaskSwingUpExit);
        _TaskList.DefineTask(TaskEnum.SwingDown, TaskSwingDownEnter, TaskSwingDownUpdate, TaskSwingDownExit);
        _TaskList.DefineTask(TaskEnum.Kick,TaskKickEnter,TaskKickUpdate,TaskKickExit);
        _TaskList.DefineTask(TaskEnum.Charge, TaskChargeEnter, TaskChargeUpdate, TaskChargeExit);
        _TaskList.DefineTask(TaskEnum.SuperKick, TaskSuperKickEnter, TaskSuperKickUpdate, TaskSuperKickExit);
        _TaskList.DefineTask(TaskEnum.ReturnPostion, TaskReturnEnter, TaskReturnUpdate, TaskReturnExit);
        _TaskList.DefineTask(TaskEnum.Damage, TaskDamageEnter, TaskDamageUpdate, TaskDamageExit);
        _TaskList.DefineTask(TaskEnum.Defend, TaskDefendEnter, TaskDefendUpdate, TaskDefendExit);
        _TaskList.DefineTask(TaskEnum.Wait, TaskWaitEnter, TaskWaitUpdate, TaskWaitExit);

        TurnTo(_Player);

        //乱数用の初期設定
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    // Update is called once per frame
    void Update()
    {
        //バトルが始まってたら処理する
        if(_HasStartBattle)
        {
            

            //戦える状態だったら戦う
            if (!_IsDead && !_Player.GetComponent<PlayerController>().damage)
            {
                _AnimController.SetBool("IsIdle", false);

                //身体の処理
                CheckBackColorAndControlBody();
            }

            UpdateState();
        }
        //一定の距離近づいたらバトルが始まる
        else if(Distance < BATTLE_START_DISTANCE)
        {
            _HasStartBattle = true;
            
        }
    }


    /// <summary>
    /// 状態毎の毎フレーム呼ばれる処理(状態で分けたUpdate)
    /// </summary>
    private void UpdateState()
    {
        var ply = _Player.GetComponent<PlayerController>();

        // 状態毎の毎フレーム呼ばれる処理
        switch (_State)
        {
            case StateEnum.None:
                // None時に毎フレーム呼ばれる処理
                {

                    if(ply.damage)
                    {
                        break;
                    }

                    //一回でもある程度近づいてきたらバトル開始
                    if (Distance < BATTLE_START_DISTANCE)
                    {  
                        _HasStartBattle = true;
                        _AnimController.SetBool("IsBattle",true);
                        _State = StateEnum.Move;
                    }
                }
                break;
            case StateEnum.Move:
                // Move時に毎フレーム呼ばれる処理
                {
                    //死亡する処理
                    if (_CurrentHP <= 0)
                    {
                        _State = StateEnum.Dead;
                        break;
                    }

                    //登録された一連のタスクの処理がすべて完了したら、次の一連のタスクを登録する
                    if (_TaskList.IsEnd)
                    {
                        if(ply.damage)
                        {
                            Idle();
                            _State = StateEnum.None;
                        }
                        else
                        {
                            // 一連のタスクを登録する
                            SelectTasks();
                            
                        }
                    }

                    // セットしてるタスクのUpdate()を呼ぶ
                    _TaskList.UpdateTask();
                    // 攻撃のインターバルを測定する
                    _AttackIntervalTimer.UpdateTimer();
                }
                break;
            case StateEnum.Dead:
                // Dead時に毎フレーム呼ばれる処理
                {
                }
                break;
        }
    }
    /// <summary> 戦い方のタイプ(残りHPで変わる) </summary>
    int _BattleType = 2;
    private void SelectTasks()
    {
        //プレイヤーが見えない間は動き続ける
        if(IsNoPlayer)
        {
            Move();
            return;
        }

        switch (_BattleType)
        {
            //状態１
            case 2:
                {
                    //インターバルが終わってなければ攻撃しない
                    if(!CanAttack)
                    {
                        break;
                    }
                    // 攻撃範囲に入ってきたら攻撃
                    if (Distance < DISTANCE1*1.25f)
                    {
                        Wait(0.25f);
                        Attack1();
                    }
                    else Move();
                    
                }
                break;
            //状態２
            case 1:
                {
                    if (!CanAttack)
                    {
                        break;
                    }

                    Move();

                    //プレイヤーが近くでジャンプしたら防御
                    if (!_Player.GetComponent<PlayerController>().onStage && Distance < DISTANCE1*1.5f)
                    {
                        Defend(2.0f);
                    }

                    if (Distance < DISTANCE1)
                    {
                        //2回は爪攻撃をする
                        if (_AttackCounter < 1)
                        {
                            Attack1();
                            Wait(2.0f);
                            Defend(1.0f);
                            _AttackCounter++;
                        }
                        else
                        {
                            Attack1();
                            Charge(0.3f);
                            Attack2();
                            _AttackCounter = 0;
                        }
                    }
                    else if(Distance > DISTANCE2)
                    {
                        //
                        Charge(1.0f);
                        Attack2();
                        Wait(1.5f);
                    }

                }
                break;
            //状態３
            case 0:
                {
                    //プレイヤーが近くでジャンプしたら防御
                    if(!_Player.GetComponent<PlayerController>().onStage && Distance < 5)
                    {
                        Defend(2.0f);
                    }

                    if(!CanAttack)
                    {
                        Debug.Log("AttackTime!!!!");
                        break;
                    }
            
                    // 2回以上攻撃したら突撃攻撃
                    if (_AttackCounter >= 2)
                    {
                        Attack3();
                        Wait(2.0f);
                        _AttackCounter = 0;
                    }

                    //近くにいたら爪攻撃
                    else if (Distance < DISTANCE1)
                    {
                        Wait(0.2f);
                        Attack1();
                        Attack1();
                        Attack1();
                        Attack1();
                        _AttackCounter++;
                    }
                    //遠くにいたら飛び蹴りからの爪攻撃
                    else if (Distance > DISTANCE2)
                    {
                        Attack2();
                        Attack1();
                        Attack1();
                        Attack1();
                        Wait(0.2f);
                        Defend(2.0f);
                        _AttackCounter++;
                    }
                    //その中間にいたら近づく
                    else
                    {
                        Move();
                    }
                }
                break;
        }

    }

    public override void Idle()
    {
        //通常状態の処理
        _TaskList.AddTask(TaskEnum.Idle);
    }

    public override void Move()
    {
        var obj = _Player;

        if(IsNoPlayer)
        {
            //今の向きによって対象物を決める
            if(_Dir == 1)
            {
                obj = _RightSetPos;
            }
            else
            {
                obj = _LeftSetPos;
            }

            //目的地にある程度近づいたら対象を変える
            if (Mathf.Abs(transform.position.x - obj.transform.position.x) < 2)
            {
                if (obj == _RightSetPos)
                {
                    obj = _LeftSetPos;
                }
                else
                {
                    obj = _RightSetPos;
                }
            }
        }

        //向きを決める
        TurnTo(obj);

        _TaskList.AddTask(TaskEnum.Walk);
    }

    /// <summary> 爪攻撃 </summary>
    public override void Attack1()
    {
        _TaskList.AddTask(TaskEnum.SwingUp);
        if (_BattleType != 0)
        {
            Charge(0.3f);
        }
        _TaskList.AddTask(TaskEnum.SwingDown);
    }

    /// <summary> 飛び蹴り </summary>
    public override void Attack2()
    {
        _TaskList.AddTask(TaskEnum.Kick);
    }

    /// <summary> プレイヤーの方向に突撃 </summary>
    public override void Attack3()
    {
        Defend(1.0f);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        Charge(1.25f);
        _TaskList.AddTask(TaskEnum.SuperKick);
    }

    public override void Wait(float waitTime)
    {
        _WaitTime = waitTime;
        _TaskList.AddTask(TaskEnum.Wait);
    }

    public override void Damage()
    {
        _TaskList.CancelAllTask();
        _TaskList.AddTask(TaskEnum.Damage);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        Defend(2.0f);

        // 攻撃タイプを更新
        var preBattleType = _BattleType;
        _BattleType = (_CurrentHP - 1) / 3;

        //攻撃タイプが変わったら攻撃回数をリセット
        if (preBattleType != _BattleType)
        {
            _AttackCounter = 0;
        }
        //ダメージを受けた時の処理(アニメーション再生とか)
        //Debug.Log("ダメージ受けた！！残り残機:" + this._CurrentHP);

        //背景色を黒以外に変更
        _LeverSwitch?.ResetBarPos();

        if(_BattleType == 1)
        {
            Attack2();
            _AttackInterval = 0.75f;
            _SwingUpTime = 0.5f;
            _SwingDownTime = 0.5f;
            _LeverSwitch?.ChangeBarPos();
        }

        //ラストは攻撃間隔を短くする
        if (_BattleType == 0)
        {
            _AttackInterval = 0.5f;
            _SwingUpTime = 0.2f;
            _SwingDownTime = 0.25f;
            if(_CurrentHP == 1)
            {
                Attack3();
            }
            else
            {
                Attack2();
            }
            _LeverSwitch?.ChangeBarPos();
            _LeverSwitch?.ChangeBarPos();
            _LeverSwitch?.ChangeBarPos();
        }
        //animController.SetBool("HasDamage", false);
    }

    public override void Down()
    {
        //死ぬ処理
        //Debug.Log("ダメージ。あ！死んだ！！");
        _AnimController.SetBool("HasDown",true);
        _AnimController.SetBool("IsDefend", false);
        _AnimController.Play("Panda_Down", 0, 0);

        //攻撃判定のオブジェクトを消す
        _BlackBody.SetActive(false);
        _WhiteBody.SetActive(false);
        
        //攻撃を受けたらレバーの位置をリセット
        _LeverSwitch?.ResetBarPos();

        this._IsDead = true;
    }

    /// <summary> 防御 </summary>
    /// <param name="defendTime">防御する時間</param>
    public void Defend(float defendTime)
    {
        //時間を設定してタスクを追加
        _DefendTime = defendTime;
        _TaskList.AddTask(TaskEnum.Defend);
    }

    public void Charge(float chargeTime)
    {
        //時間を設定してタスクを追加
        _ChargeTime = chargeTime;
        _TaskList.AddTask(TaskEnum.Charge);
    }


    /// <summary> 背景に合わせて身体を有効/無効化 </summary>
    void CheckBackColorAndControlBody()
    {
        //背景の色によって実体が消える
        bool isWhite = false;
        bool isBlack = false;
        if(this._BackColorScript?.colorType == ColorObjectVer3.OBJECT_COLOR3.WHITE)
            isWhite = true;

        if(this._BackColorScript?.colorType == ColorObjectVer3.OBJECT_COLOR3.BLACK)
            isBlack = true;

        //身体を有効/無効化する
        this._WhiteBody.SetActive(!isWhite);
        this._BlackBody.SetActive(!isBlack);
    }

    /// <summary> 背景色を変更する </summary>
    void ChangeBackColor()
    {
        _LeverSwitch?.ChangeBarPos();
    }

    #region Task function


    #region Task Idle function
    void TaskIdleEnter()
    {
        _AnimController.SetBool("CanAttack", false);
        _AnimController.SetBool("CanKick", false);
        _AnimController.SetBool("IsIdle", true);
        _AnimController.SetBool("IsBattle", false);
    }

    bool TaskIdleUpdate()
    {
        return true;
    }

    void TaskIdleExit()
    {
        // 特になし
    }
    #endregion

    #region Task Walk function

    void TaskWalkEnter()
    {
        
    }

    bool TaskWalkUpdate()
    {
        //向いてる方向に進む
        transform.Translate(_Dir * _MoveSpeed, 0, 0);
        return true;
    }

    void TaskWalkExit()
    {

    }
    #endregion


    #region Task Kick function
    bool _HasFloat;
    void TaskKickEnter()
    {
        TurnTo(_Player);
        _AnimController.SetBool("IsKick", true);
        _AnimController.Play("Panda_Kick", 0, 0);
        var vel = GetComponent<Rigidbody2D>().velocity;
        vel = _kickVel;
        vel.x *= _Dir;
        GetComponent<Rigidbody2D>().velocity = vel;

        _HasFloat = false;
    }

    bool TaskKickUpdate()
    {
        //1回でも地面から離れれば記憶する
        if(!_HasFloat && !OnGround)
        {
            _HasFloat = true;
        }
        return OnGround && _HasFloat;
    }

    void TaskKickExit()
    {
        _AnimController.SetBool("IsKick", false);
        TurnTo(_Player);
    }
    #endregion

    #region Task SuperKick function
    void TaskSuperKickEnter()
    {
        TurnTo(_Player);
        _AnimController.SetBool("IsKick", true);
        _AnimController.Play("Panda_Kick", 0, 0);

        var vel = GetComponent<Rigidbody2D>().velocity;
        vel = _kickVel*1.25f;
        vel.x = 0;
        GetComponent<Rigidbody2D>().velocity = vel;

        _HasFloat = false;
    }

    bool TaskSuperKickUpdate()
    {
        var desObj = _RightSetPos;
        if(_Dir == -1)
        {
            desObj = _LeftSetPos;
        }
        var des = desObj.transform.position.x;

        //端に着くまで移動する
        transform.Translate(_Dir * 0.8f, 0, 0);
        
        var diff = Mathf.Abs(transform.position.x - des);
        return diff < 1;
    }

    void TaskSuperKickExit()
    {
        _AnimController.SetBool("IsKick", false);
        TurnTo(_Player);
    }
    #endregion

    #region Task Charge function

    int _FrameNumber;
    float _Xpos;

    void TaskChargeEnter()
    {
        //現X座標と時間をセット
        _Timer.ResetTimer(_ChargeTime);
        _FrameNumber = 0;
        _Xpos = transform.position.x;
    }

    bool TaskChargeUpdate()
    {
        var pos = transform.position;
        var dir = 1;

        //フレームの数に応じて振動する
        if((_FrameNumber/4)%2 == 0)
        {
            dir = -1;
        }
        pos.x = _Xpos + dir * 0.25f;
        transform.position = pos;

        _FrameNumber++;
        _Timer.UpdateTimer();

        return _Timer.IsTimeUp;
    }

    void TaskChargeExit()
    {
        _AnimController.SetBool("IsDefend", false);
    }
    #endregion

    #region Task SwingUp function
    void TaskSwingUpEnter()
    {
        TurnTo(_Player);
        _Timer.ResetTimer(_SwingUpTime);
        _AnimController.Play("Panda_SwingUp", 0, 0);
    }

    bool TaskSwingUpUpdate()
    {
        _Timer.UpdateTimer();
        return _Timer.IsTimeUp;
    }

    void TaskSwingUpExit()
    {
    }
    #endregion

    #region Task SwingDown function

    
    void TaskSwingDownEnter()
    {
         //TurnTo(_Player);
        _Timer.ResetTimer(_SwingDownTime);
        _AnimController.SetBool("IsAttack", true);
        _AnimController.Play("Panda_SwingDown", 0, 0 );
    }

    bool TaskSwingDownUpdate()
    {
        _Timer.UpdateTimer();
        return _Timer.IsTimeUp;
    }

    void TaskSwingDownExit()
    {
        _AttackIntervalTimer.ResetTimer(_AttackInterval);
        _AnimController.SetBool("IsAttack", false);
    }
    #endregion

    #region Task Return function
    void TaskReturnEnter()
    {
        TurnTo(_Player);
        
        var rb = GetComponent<Rigidbody2D>();
        var vel = rb.velocity;
        var diff = 0f;

        //右向きなら左端へ、左向きなら右端へ跳ぶ
        if (_Dir == 1)
        {
            diff = _LeftSetPos.transform.position.x - transform.position.x;
        }
        else
        {
            diff = _RightSetPos.transform.position.x - transform.position.x;
        }
        
        //Debug.Log("diff::" + diff);
        vel.x = diff *(4f/5f);
        vel.y = 30;

        _HasFloat = true;

        //遠くにいたら飛ぶ
        if (Mathf.Abs(diff) > 2f)
        {
            _AnimController.SetBool("IsKick", true);
            _AnimController.Play("Panda_Kick", 0, 0);
            rb.velocity = vel;
            _HasFloat = false;
        }
    }

    bool TaskReturnUpdate()
    {
        if(!OnGround)
        {
            _HasFloat = true;
        }
        return OnGround && _HasFloat;
    }

    void TaskReturnExit()
    {
        //背景色を変更
        if(_AnimController.GetBool("IsKick"))
        {
            ChangeBackColor();
        }
        _AnimController.SetBool("IsKick", false);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _IsUnableBeAttacked = false;

         
    }
    #endregion

    #region Task Damage function
    void TaskDamageEnter()
    {
        _IsUnableBeAttacked = true;
        _Timer.ResetTimer(1.0f);
        _AnimController.SetBool("HasDamage", true);
        _AnimController.Play("Panda_Damage", 0, 0);
        _AnimController.SetBool("IsDefend", false);
    }

    bool TaskDamageUpdate()
    {
        _Timer.UpdateTimer();
        return _Timer.IsTimeUp;
    }

    void TaskDamageExit()
    {
        _AnimController.SetBool("HasDamage", false);
        _Timer.ResetTimer(_DamageTimeInterval);
    }

    #endregion

    #region Task Defend function

    void TaskDefendEnter()
    {
        _Timer.ResetTimer(_DefendTime);
        _AnimController.SetBool("IsDefend", true);
        //防御の間は無敵にする
        _IsUnableBeAttacked = true;
        
    }

    bool TaskDefendUpdate()
    {
        //ダメージは受けないけど踏める
        _WhiteBody.SetActive(true);
        _Timer.UpdateTimer();
        return Distance > 2 && _Timer.IsTimeUp;
    }

    void TaskDefendExit()
    {
        _AnimController.SetBool("IsDefend", false);
        _IsUnableBeAttacked = false;
        //背景色に合わせる
        CheckBackColorAndControlBody();
    }
    #endregion

    #region Task Wait function
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
        _IsUnableBeAttacked = false;
    }
    #endregion

    #endregion
}