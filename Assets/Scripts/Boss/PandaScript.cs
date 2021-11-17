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
        
        Jump,
        MiniJump,
        ClawAttack,
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
    [SerializeField] private int firstHp = 3;       //パンダの体力
    [SerializeField] private float firstMoveSpeed;  //パンダのスピード
    [SerializeField] GameObject backGroundObject;
    [SerializeField] private GameObject leverObject; //レバーのオブジェクト
    [SerializeField] private GameObject _Player = null;
    [SerializeField] private GameObject _RightSetPos = null;
    [SerializeField] private GameObject _LeftSetPos = null;
    /// <summary> 飛び蹴りベクトル </summary>
    [SerializeField] private Vector2 kickVel = new Vector2(25f, 20f);
    #endregion

    #region field
    /// <summary> バトルを開始する距離 </summary>
    private const float BATTLE_START_DISTANCE = 40f; 
    /// <summary> 攻撃１をする距離 </summary>
    private const float DISTANCE1 = 7.5f;             
    /// <summary> 攻撃２をする距離 </summary>
    private const float DISTANCE2 = 25f;             
    /// <summary>  </summary>
    private bool hasKick = false;
    /// <summary> 身体の黒い部分(主に攻撃判定) </summary>
    private GameObject blackBody = null;                   
    /// <summary> 身体の白い部分 </summary>
    private GameObject whiteBody = null;                   
    /// <summary> 背景のスクリプト </summary>
    private ColorObjectVer3 backColorScript = null;        

    /// <summary> タスクリストのインスタンス </summary>
    private TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();

    /// <summary> 攻撃した回数 </summary>
    private int _AttackCounter = 0;
    
    
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
        _CurrentHP = firstHp;
        _MoveSpeed = firstMoveSpeed;
        _AttackTimeInterval = 3;
        _AttackTime = _AttackTimeInterval;
        _DamageTimeInterval = 1;
        _DamageTime = _DamageTimeInterval;
        _AnimController = GetComponent<Animator>();
        _BossSize = transform.localScale.x;
        blackBody = transform.Find("Black").gameObject;
        whiteBody = transform.Find("White").gameObject;
        blackBody.SetActive(false);
        backColorScript = backGroundObject?.GetComponent<ColorObjectVer3>();
        //Debug.Log("attackTime:" + attackTime + " inter:" + attackTimeInterval);

        //タスクの登録
        _TaskList.DefineTask(TaskEnum.Idle, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
        _TaskList.DefineTask(TaskEnum.Walk, TaskWalkEnter, TaskWalkUpdate, TaskWalkExit);
        _TaskList.DefineTask(TaskEnum.Jump, TaskJumpEnter, TaskJumpUpdate, TaskJumpExit);
        _TaskList.DefineTask(TaskEnum.MiniJump, TaskMiniJumpEnter, TaskMiniJumpUpdate, TaskMiniJumpExit);
        _TaskList.DefineTask(TaskEnum.ClawAttack, TaskClawEnter, TaskClawUpdate, TaskClawExit);
        _TaskList.DefineTask(TaskEnum.Kick,TaskKickEnter,TaskKickUpdate,TaskKickExit);
        _TaskList.DefineTask(TaskEnum.SuperKick, TaskSuperKickEnter, TaskSuperKickUpdate, TaskSuperKickExit);
        _TaskList.DefineTask(TaskEnum.ReturnPostion, TaskReturnEnter, TaskReturnUpdate, TaskReturnExit);
        _TaskList.DefineTask(TaskEnum.Wait, TaskWaitEnter, TaskWaitUpdate, TaskWaitExit);
        _TaskList.DefineTask(TaskEnum.Damage, TaskDamageEnter, TaskDamageUpdate, TaskDamageExit);
        _TaskList.DefineTask(TaskEnum.Defend, TaskDefendEnter, TaskDefendUpdate, TaskDefendExit);

        TurnTo(_Player);
    }

    // Update is called once per frame
    void Update()
    {
        //バトルが始まってたら処理する
        if(_HasStartBattle)
        {
            UpdateState();

            //戦える状態だったら戦う
            if(!_IsDead && !_Player.GetComponent<PlayerController>().damage )
            {
                //Debug.Log(GameObject.Find("Player").transform.Find("Body").gameObject);
                _AnimController.SetBool("IsIdle",false);
                //if(animController.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Panda_Damage")
                  //  animController.SetBool("HasDamage",false);

                //身体の処理
                CheckBackColorAndControlBody();

                //攻撃にインターバルをつける。一定の時間が経つまでは攻撃できない。
                //if(this.attackTime < attackTimeInterval)
                //{
                //    this.attackTime++;
                //    Debug.Log("足してるはず");
                //    animController.SetBool("CanKick",false);
                //    animController.SetBool("CanAttack",false);
                //}
                //else this.canAttack = true;

                
                if(_DamageTime < _DamageTimeInterval)_DamageTime++;

                //キックしたら印をつける
                if(_AnimController.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Panda_Kick"
                    && !transform.Find("Foot").gameObject.GetComponent<FootAreaScript>().touchingStage)
                    hasKick = true;

                //キックした後に着地したらアニメーションを終わらせる
                //if(hasKick && transform.Find("Foot").gameObject.GetComponent<FootAreaScript>().touchingStage)
                //{
                //    hasKick = false;
                //    animController.SetBool("CanKick",false);
                //    GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                //}

                //if(animController.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Panda_Move")
                //    TurnToPlayer();

                //ある程度プレイヤーに近づくまでジリジリ近づく
                //if( (this.hp == 3 && Distance > DISTANCE1) 
                //|| (this.hp <= 2 && Distance > DISTANCE2))
                //    Move();

                //else if(this.canAttack)
                //{
                    //引っ掻き攻撃
                //    if(Distance < DISTANCE1)
                //        Attack1();

                    //飛び蹴り攻撃
                //    else if(this.hp <= 2 && Distance < DISTANCE2)
                //        Attack2();
                
                //    this.canAttack = false;
                //    attackTime = 0;
                    //Debug.Log("attackTime:" + attackTime);
                //}
            }
            else 
            {
                //Idle();
            }
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
                    if (Distance < BATTLE_START_DISTANCE)
                    {  
                        if(ply.damage)
                        {
                            break;
                        }

                        //一回でもある程度近づいてきたらバトル開始
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
                    }

                    //登録された一連のタスクの処理がすべて完了したら、次の一連のタスクを登録する
                    if (_TaskList.IsEnd)
                    {
                        if(IsNoPlayer || ply.damage)
                        {
                            _State = StateEnum.None;
                        }
                        else
                        {
                            // 一連のタスクを登録する
                            SelectTasks();
                        }
                        
                    }

                    // UnityのUpdate関数の中で _TaskList.UpdateTask()を呼ぶ
                    // 登録されているタスクが処理され,タスクが終われば次のタスクがセットされる. 次のタスクが無い時は空になる.
                    _TaskList.UpdateTask();
                }
                break;
            case StateEnum.Dead:
                // Dead時に毎フレーム呼ばれる処理
                {
                }
                break;
        }
    }

    private void SelectTasks()
    {
        switch (_CurrentHP)
        {
            //状態１
            case 3:
                {
                    //Debug.Log(Distance);
                    // 攻撃範囲に入ってきたら攻撃
                    if (Distance < DISTANCE1)
                    {
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.Wait);
                    }
                    else _TaskList.AddTask(TaskEnum.Walk);
                    _TaskList.AddTask(TaskEnum.Walk);
                }
                break;
            //状態２
            case 2:
                {
                    _TaskList.AddTask(TaskEnum.Walk);

                }
                break;
            //状態３
            case 1:
                {
                    //プレイヤーがジャンプしたら防御
                    if(!_Player.GetComponent<PlayerController>().onStage)
                    {
                        _TaskList.AddTask(TaskEnum.Defend);
                    }

                    //
                    if(Distance > DISTANCE2)
                    {
                        _TaskList.AddTask(TaskEnum.Walk);
                    }

                    //
                    if (Distance < DISTANCE1)
                    {
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _AttackCounter++;
                    }

                    //
                    else if (Distance < DISTANCE2)
                    {
                        _TaskList.AddTask(TaskEnum.Kick);
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.ClawAttack);
                        _TaskList.AddTask(TaskEnum.Wait);
                        _TaskList.AddTask(TaskEnum.Defend);
                        _AttackCounter++;

                    }

                    //
                    if (_AttackCounter > 2)
                    {
                        _TaskList.AddTask(TaskEnum.ReturnPostion);
                        _TaskList.AddTask(TaskEnum.Wait);
                        _TaskList.AddTask(TaskEnum.SuperKick);
                        _TaskList.AddTask(TaskEnum.Wait);
                        _Timer.ResetTimer(3.0f);
                        _AttackCounter = 0;
                    }
                    Debug.Log("Attack::" + _AttackCounter);
                }
                break;
        }

    }

    //通常状態
    public override void Idle()
    {
        //通常状態の処理

    }

    //移動
    public override void Move()
    {
        //Debug.Log("Move()");
        //移動の処理        
        //animController.SetBool("",true);
        if(_AnimController.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Panda_Move")
            return;

        //移動する
        transform.Translate(-_MoveSpeed*_Dir,0f,0f);
    }

    //攻撃1(引っ掻きのみ)
    public override void Attack1()
    {
        _AnimController.SetBool("CanKick",false);
        _AnimController.SetBool("CanAttack",true);
    }

    //攻撃2(飛び蹴り追加)
    public override void Attack2()
    {
        if(!transform.Find("Foot").gameObject.GetComponent<FootAreaScript>().touchingStage)
            return;

        //攻撃２の処理
        _AnimController.SetBool("CanKick",true);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 force = new Vector2(-_Dir*600f*rb.mass,500f*rb.mass);
        
        rb.AddForce(force);
        this._CanAttack = true;
        this._AttackTime = this._AttackTimeInterval;
    }

    //攻撃3(連続攻撃)
    public override void Attack3()
    {
        //攻撃３の処理
    }

    //ダメージ処理
    public override void Damage()
    {
        _TaskList.CancelAllTask();
        _TaskList.AddTask(TaskEnum.Damage);
        _TaskList.AddTask(TaskEnum.ReturnPostion);
        _TaskList.AddTask(TaskEnum.Wait);


        //ダメージを受けた時の処理(アニメーション再生とか)
        Debug.Log("ダメージ受けた！！残り残機:" + this._CurrentHP);
        
        //
        //animController.SetBool("HasDamage",true);

        //攻撃を受けたらレバーの位置をリセット
        if(leverObject)
            leverObject.GetComponent<LeverScript>().ResetBarPos();
        else Debug.Log("パンダにレバーが設定されてないよ！！");

        //ラストは攻撃間隔を短くする
        if (_CurrentHP == 1)
        {
            _AttackTimeInterval = 0;
        }
        //animController.SetBool("HasDamage", false);
    }

    //倒れる処理
    public override void Down()
    {
        //死ぬ処理
        Debug.Log("ダメージ。あ！死んだ！！");
        _AnimController.SetBool("HasDown",true);

        //攻撃判定のオブジェクトを消す
        blackBody.SetActive(false);
        whiteBody.SetActive(false);
        
        //攻撃を受けたらレバーの位置をリセット
        if(leverObject)
            leverObject.GetComponent<LeverScript>().ResetBarPos();

        this._IsDead = true;
    }

    /// <summary> 背景に合わせて身体を有効/無効化 </summary>
    void CheckBackColorAndControlBody()
    {
        //背景の色によって実体が消える
        bool isWhite = false;
        bool isBlack = false;
        if(this.backColorScript?.colorType == ColorObjectVer3.OBJECT_COLOR3.WHITE)
            isWhite = true;

        if(this.backColorScript?.colorType == ColorObjectVer3.OBJECT_COLOR3.BLACK)
            isBlack = true;

        //身体を有効/無効化する
        this.whiteBody.SetActive(!isWhite);
        this.blackBody.SetActive(!isBlack);
    }


    #region Task function
    TimerScript _Timer = new TimerScript();

    #region Task Idle function
    void TaskIdleEnter()
    {
        _AnimController.SetBool("CanAttack", false);
        _AnimController.SetBool("CanKick", false);
        _AnimController.SetBool("IsIdle", true);
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
        TurnTo(_Player);
    }

    bool TaskWalkUpdate()
    {
        //Debug.Log("dir::" + dir);
        transform.Translate(_Dir * _MoveSpeed, 0, 0);
        return true;
    }

    void TaskWalkExit()
    {

    }
    #endregion

    #region Task Jump function

    void TaskJumpEnter()
    {
    }

    bool TaskJumpUpdate()
    {
        return true;
    }

    void TaskJumpExit()
    {

    }
    #endregion

    #region Task Kick function
    bool _HasFloat;
    void TaskKickEnter()
    {
        TurnTo(_Player);
        _AnimController.SetBool("CanKick", true);
        var vel = GetComponent<Rigidbody2D>().velocity;
        vel = kickVel;
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
        _AnimController.SetBool("CanKick", false);
    }
    #endregion

    #region Task SuperKick function
    void TaskSuperKickEnter()
    {
        TurnTo(_Player);
        _AnimController.SetBool("CanKick", true);

        var vel = GetComponent<Rigidbody2D>().velocity;
        vel = kickVel;
        vel.x *= _Dir;
        //GetComponent<Rigidbody2D>().velocity = vel;

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

        transform.Translate(_Dir * 0.5f, 0, 0);
        

        var diff = Mathf.Abs(transform.position.x - des);
        return diff < 1;
    }

    void TaskSuperKickExit()
    {
        _AnimController.SetBool("CanKick", false);
        _Timer.ResetTimer(1.0f);
    }
    #endregion

    #region Task MiniJump function
    void TaskMiniJumpEnter()
    {

    }

    bool TaskMiniJumpUpdate()
    {
        return true;
    }

    void TaskMiniJumpExit()
    {

    }
    #endregion

    #region Task Claw function
    void TaskClawEnter()
    {
        TurnTo(_Player);
        _Timer.ResetTimer(0.4f);
        _AnimController.SetBool("CanAttack", true);
        _AnimController.Play("Panda_Attack",0,0);
    }

    bool TaskClawUpdate()
    {
        _Timer.UpdateTimer();
        return _Timer.IsTimeUp;
    }

    void TaskClawExit()
    {
        _AnimController.SetBool("CanAttack", false);
        _Timer.ResetTimer(_AttackTimeInterval);
    }
    #endregion

    #region Task Return function
    void TaskReturnEnter()
    {
        TurnTo(_Player);
        _AnimController.SetBool("CanKick", true);
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
        
        Debug.Log("diff::" + diff);
        vel.x = diff *(4f/5f);
        vel.y = 30;

        _HasFloat = true;

        if (Mathf.Abs(diff) > 2f)
        {
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
        _AnimController.SetBool("CanKick", false);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _Timer.ResetTimer(2.0f);
    }
    #endregion

    #region Task Damage function
    void TaskDamageEnter()
    {
        _IsNotAttacked = true;
        _Timer.ResetTimer(1.0f);
        _AnimController.SetBool("HasDamage", true);
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
        _AnimController.SetBool("IsDefend", true);
    }

    bool TaskDefendUpdate()
    {
        return Distance > 2;
    }

    void TaskDefendExit()
    {
        _AnimController.SetBool("IsDefend", false);
    }
    #endregion

    #region Task Wait function
    void TaskWaitEnter()
    {
    }

    bool TaskWaitUpdate()
    {
        _Timer.UpdateTimer();
        return _Timer.IsTimeUp;
    }

    void TaskWaitExit()
    {
        _IsNotAttacked = false;
    }
    #endregion
    #endregion
}