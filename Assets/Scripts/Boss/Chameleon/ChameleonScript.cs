using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonScript : BossBase
{
    #region define
    /// <summary> タスクの定義 </summary>
    public enum TaskEnum
    {
        IDLE,
        WALK,
        TONGUE_ATTACK,
        TRANSLATE,
        TRANSPARENT,
        TRANSPARENT_END,
        SCARED,
        DAMAGE,
        DOWN,
        WAIT
    }
    #endregion

    #region field
    /// <summary> 最大HP </summary>
    [SerializeField] private int _MaxHP = 9;
    /// <summary> 歩くスピード </summary>
    [SerializeField] private float _WalkSpeed = 1;
    /// <summary> フェーズ内の攻撃回数 </summary>
    [SerializeField] private int _AttackCount = 0;
    /// <summary> 雷が落ちるまでの攻撃回数 </summary>
    [SerializeField] private int _MaxAttackCount = 3;
    /// <summary> 何段階目か </summary>
    [SerializeField] private int _Phase;
    /// <summary> 何段階目まであるか </summary>
    [SerializeField] private int _MaxPhase = 3;
    /// <summary> 何段階目まであるか </summary>
    private int _CharaUpdateCount = 0;

    /// <summary> プレイヤー </summary>
    [SerializeField] private GameObject _Player = null;

    /// <summary> タスク管理 </summary>
    private TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();

    /// <summary> 時間計測用 </summary>
    private TimerScript _TimerScript = new TimerScript();
    ///// <summary> ボスの経過時間 </summary>
    //private TimerScript _ManageTimerScript = new TimerScript();

    /// <summary> 近づいて攻撃する距離 </summary>
    [SerializeField] private float _AttackDistance = 20;
    /// <summary> 攻撃前の待機時間 </summary>
    [SerializeField] private float _WaitBeforeAttack = 1.5f;
    /// <summary> ダメージ後の硬直時間 </summary>
    [SerializeField] private float _DamagedStandTime = 2;
    /// <summary> 怯える追加硬直時間 </summary>
    [SerializeField] private float _ScaredTime = 3;

    /// <summary> カメレオンが移動する足場の親オブジェクト </summary>
    [SerializeField] private GameObject _AsibaParentObj;

    //舌の動き制御
    private TongueScript _TongueScript;
    //透明化制御
    private TransparentScript _TransparentScript;
    //雷制御
    public LightningScript _LightningScript;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _CurrentHP = _MaxHP;
        _BossSize = transform.localScale.x;
        _TongueScript = transform.Find("Tongue").GetComponent<TongueScript>();
        _DamageTimeInterval = 3.0f;
        _Phase = (_MaxHP - _CurrentHP) / _MaxPhase;
        if(_TongueScript == null)
            Debug.LogError("_TongueScript is null");
        _TransparentScript = GetComponent<TransparentScript>();

        //タスクの登録
        _TaskList.DefineTask(TaskEnum.IDLE, TaskIdleEnter, TaskIdleUpdate, TaskIdleExit);
        _TaskList.DefineTask(TaskEnum.WALK, TaskWalkEnter, TaskWalkUpdate, TaskWalkExit);
        _TaskList.DefineTask(TaskEnum.TONGUE_ATTACK, TaskTongueAttackEnter, TaskTongueAttackUpdate, TaskTongueAttackExit);
        _TaskList.DefineTask(TaskEnum.TRANSPARENT, TaskTransparentEnter, TaskTransparentUpdate, TaskTransparentExit);
        _TaskList.DefineTask(TaskEnum.TRANSPARENT_END, TaskTransparentEndEnter, TaskTransparentEndUpdate, TaskTransparentEndExit);
        _TaskList.DefineTask(TaskEnum.TRANSLATE, TaskTranslateEnter, TaskTranslateUpdate, TaskTranslateExit);
        _TaskList.DefineTask(TaskEnum.SCARED, TaskScaredEnter, TaskScaredUpdate, TaskScaredExit);
        _TaskList.DefineTask(TaskEnum.DAMAGE, TaskDamageEnter, TaskDamageUpdate, TaskDamageExit);
        _TaskList.DefineTask(TaskEnum.DOWN, TaskDownEnter, TaskDownUpdate, TaskDownExit);
        _TaskList.DefineTask(TaskEnum.WAIT, TaskWaitEnter, TaskWaitUpdate, TaskWaitExit);

        _State = StateEnum.Move;

        //プレイヤーを向く
        TurnTo(_Player);
    }

    // Update is called once per frame
    void Update()
    {
        if (_TaskList.IsEnd)
            CharaUpdate();
        _TaskList.UpdateTask();
    }

    private void CharaUpdate()
    {
        //switch (_State)
        //{
        //    case StateEnum.None:
        //        break;
        //    case StateEnum.Move:
        //        Move();
        //        break;
        //    case StateEnum.Dead:
        //        break;
        //}
        //距離測る
        float TongueToPlayerLength = Mathf.Abs(_TongueScript.gameObject.transform.position.x - _Player.transform.position.x);
        switch (_Phase)
        {
            case 0:
                if (TongueToPlayerLength <= _AttackDistance)
                {
                    Wait(0.5f);
                    Attack1();
                }
                else
                {
                    _TaskList.AddTask(TaskEnum.WALK);
                }
                break;
            case 1:
                if (TongueToPlayerLength <= _AttackDistance)
                {
                    Wait(0.5f);
                    Attack1();
                }
                else
                {
                    _TaskList.AddTask(TaskEnum.WALK);
                }
                break;
            case 2:
                //交互にやる
                if (_CharaUpdateCount % 2 == 0)
                {
                    _TaskList.AddTask(TaskEnum.TRANSLATE);
                    Wait(0.5f);
                }
                else
                {
                    Attack1();
                }
                break;
        }
        _CharaUpdateCount++;
    }

    #region タスク関数
    /// <summary> 待機処理
    private void TaskIdleEnter()
    {
        //アニメーションは待機に
    }

    private bool TaskIdleUpdate()
    {
        return true;
    }

    private void TaskIdleExit()
    {
        //アニメーションを止める
    }

    /// <summary> 歩き処理
    private void TaskWalkEnter()
    {
        //アニメーションは歩くに
        //Debug.Log("Walk");
    }

    private bool TaskWalkUpdate()
    {
        _TimerScript.UpdateTimer();
        //プレイヤーを向く
        TurnTo(_Player);
        //プレイヤーの方向へ少し移動
        transform.Translate(_Dir * _WalkSpeed * Time.deltaTime * Vector3.right);
        float TongueToPlayerLength = Mathf.Abs(_TongueScript.gameObject.transform.position.x - _Player.transform.position.x);
        return TongueToPlayerLength <= _AttackDistance;
    }

    private void TaskWalkExit()
    {
        //Debug.Log("Walk End");
    }

    /// <summary> 舌攻撃処理
    private void TaskTongueAttackEnter()
    {
        //アニメーション舌を出す
        //攻撃回数++
        _AttackCount++;
        //舌を動かし始める
        _TongueScript.StartStretch();
        //Debug.Log("Attack");
    }

    private bool TaskTongueAttackUpdate()
    {
        return _TongueScript.IsFin;
    }

    private void TaskTongueAttackExit()
    {
        //アニメーションを止める
        //Debug.Log("Attack End");
        //舌関係全てリセット
        _TongueScript.ResetTongue();
    }

    /// <summary> 瞬間移動処理
    private void TaskTranslateEnter()
    {
        //待機に
        //移動
        int no = Random.Range(0, 2);
        Transform asibaTransform = _AsibaParentObj.transform.GetChild(no);
        Vector3 pos = new Vector3(asibaTransform.position.x, asibaTransform.position.y + asibaTransform.localScale.y/2 + transform.localScale.y/2, 0);
        transform.position = pos;
    }

    private bool TaskTranslateUpdate()
    {
        return true;
    }

    private void TaskTranslateExit()
    {
        //アニメーションを止める
    }

    /// <summary> 透明化処理
    private void TaskTransparentEnter()
    {
        //待機に
        //透明化を始める
        _TransparentScript.IsTransparent = true;
        _TransparentScript.StartTransparent();
    }

    private bool TaskTransparentUpdate()
    {
        return _TransparentScript.IsFin;
    }

    private void TaskTransparentExit()
    {
        //アニメーションを止める
    }

    /// <summary> 透明化終了処理
    private void TaskTransparentEndEnter()
    {
        //待機に
        //見えるようにする
        _TransparentScript.IsTransparent = false;
        _TransparentScript.StartTransparent();
    }

    private bool TaskTransparentEndUpdate()
    {
        return _TransparentScript.IsFin;
    }

    private void TaskTransparentEndExit()
    {
        //アニメーションを止める
    }

    /// <summary> 怯える処理
    private void TaskScaredEnter()
    {
        //アニメーションは待機に
        //雷の時は以下の処理をする
        //舌をしまう
        _TongueScript.ResetTongue();
        //ボディーのダメージを消す
        //プレイヤーのダメージを有効にする
    }

    private bool TaskScaredUpdate()
    {
        return _LightningScript.IsFin;
    }

    private void TaskScaredExit()
    {
        //アニメーションを止める
        ResetBoss();
    }

    /// <summary> ダメージ処理
    private void TaskDamageEnter()
    {
        //アニメーションは待機に
        //時間設定
        _TimerScript.ResetTimer(_DamageTimeInterval);
        //ダメージを与えられなく、食らわない
        AllDamageOrNot(false);
        Debug.Log("Damage");
    }

    private bool TaskDamageUpdate()
    {
        _TimerScript.UpdateTimer();
        return _TimerScript.IsTimeUp;
    }

    private void TaskDamageExit()
    {
        //アニメーションを止める

        //_IsUnableBeAttacked = false;
        //ダメージを与えられる、食らう
        AllDamageOrNot(true);
        Debug.Log("Damage End");
    }

    /// <summary> 死んだ処理
    private void TaskDownEnter()
    {
        //アニメーション
        //ダメージ判定を消す
        _TongueScript.gameObject.GetComponent<DamageObjectScript>().enabled = false;
    }

    private bool TaskDownUpdate()
    {
        return false;
    }

    private void TaskDownExit()
    {

    }

    /// <summary> 待機処理
    private void TaskWaitEnter()
    {
        //時間設定
        _TimerScript.ResetTimer(_WaitTime);
    }

    private bool TaskWaitUpdate()
    {
        _TimerScript.UpdateTimer();
        return _TimerScript.IsTimeUp;
    }

    private void TaskWaitExit()
    {
        
    }
    #endregion

    #region BoseBaseからのOverride関数
    //通常舌攻撃
    public override void Attack1()
    {
        //プレイヤーを向く
        TurnTo(_Player);
        //スクリプトの初期化
        _TongueScript.SetStretch();
        //攻撃まで待機
        Wait(_WaitBeforeAttack);
        //リスト追加
        _TaskList.AddTask(TaskEnum.TONGUE_ATTACK);
    }

    public override void Attack2()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack3()
    {
        throw new System.NotImplementedException();
    }

    public override void Damage()
    {
        if (!_IsUnableBeAttacked)
        {
            ////無敵
            //_IsUnableBeAttacked = true;
            ResetBoss();
            //タスクを消して
            _TaskList.CancelAllTask();
            //ダメージにする
            _TaskList.AddTask(TaskEnum.DAMAGE);
            //フェーズが1なら透明になる
            if(_Phase == 1)
            {
                _TaskList.AddTask(TaskEnum.TRANSPARENT);
            }
        }
    }

    public override void Down()
    {
        //タスクを消して
        _TaskList.CancelAllTask();
        //死んだ判定
        _IsDead = true;
        //タスク
        _TaskList.AddTask(TaskEnum.DOWN);
    }

    public override void Idle()
    {

    }

    public override void Move()
    {

    }

    //waitTime分待つ
    public override void Wait(float waitTime)
    {
        _WaitTime = waitTime;
        _TaskList.AddTask(TaskEnum.WAIT);
    }
    #endregion

    //ボスを無力化
    public void AllDamageOrNot(bool isDamage)
    {
        transform.Find("Tongue").gameObject.GetComponent<Collider2D>().enabled = isDamage;
        transform.Find("Head").gameObject.GetComponent<Collider2D>().enabled = isDamage;
        transform.Find("Body").gameObject.GetComponent<Collider2D>().enabled = isDamage;
    }

    /// <summary>
    /// 怯えさせる
    /// </summary>
    public void Scared()
    {
        ResetBoss();
        _TaskList.CancelAllTask();
        //見えるようになって
        _TaskList.AddTask(TaskEnum.TRANSPARENT_END);
        //怯える
        _TaskList.AddTask(TaskEnum.SCARED);
        //硬直時間追加
        Wait(_ScaredTime);
    }

    //いったんの初期化に呼ぶ
    public void ResetBoss()
    {
        //舌関係リセット
        _TongueScript.ResetTongue();
        //雷関係リセット
        _LightningScript.ResetLightning();
        //攻撃回数リセット
        _AttackCount = 0;
        //キャラアップデートリセット
        _CharaUpdateCount = 0;
        //フェーズ更新
        _Phase = (_MaxHP - _CurrentHP) / _MaxPhase;
    }
}
