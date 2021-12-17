using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChameleonScript : BossBase
{
    #region define
    /// <summary> タスクの定義 </summary>
    public enum TaskEnum
    {
        WALK,
        TONGUE_ATTACK,
        TRANSLATE,
        TRANSPARENT,
        CHANGE_COLOR,
        TRANSPARENT_END,
        RETURN_POS,
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
    /// <summary> 何段階目か </summary>
    [SerializeField] private int _Phase;
    /// <summary> 何段階目まであるか </summary>
    private int _MaxPhase = 2;
    /// <summary> フェーズごとのカメレオンの色のEnum </summary>
    [SerializeField] private ColorObjectVer3.OBJECT_COLOR3[] _PhaseColorEnums;
    /// <summary> フェーズごとの背景の色のEnum </summary>
    [SerializeField] private ColorObjectVer3.OBJECT_COLOR3[] _PhaseBackColorEnums;

    /// <summary> プレイヤー </summary>
    [SerializeField] private GameObject _Player = null;

    /// <summary> タスク管理 </summary>
    private TaskList<TaskEnum> _TaskList = new TaskList<TaskEnum>();

    /// <summary> 時間計測用 </summary>
    private TimerScript _TimerScript = new TimerScript();

    /// <summary> 近づいて攻撃する距離 </summary>
    [SerializeField] private float _AttackDistance = 20;
    /// <summary> 攻撃前の待機時間 </summary>
    [SerializeField] private float _WaitBeforeAttack = 2f;
    /// <summary> ダメージ後の硬直時間 </summary>
    [SerializeField] private float _DamagedStandTime = 2;
    /// <summary> 怯える追加硬直時間 </summary>
    [SerializeField] private float _ScaredTime = 3;
    /// <summary> キャラアップデート回数 </summary>
    private int _CharaUpdateCount = 0;

    /// <summary> カメレオンが移動する足場の親オブジェクト </summary>
    [SerializeField] private GameObject _AsibaParentObj;
    /// <summary> ダメージ後の移動する場所(2箇所) </summary>
    [SerializeField] private GameObject[] _ReturnPosObjs;
    //蜂のプレハブ
    [SerializeField] private GameObject _BeePrefab;
    //蜂のオブジェクトたち
    private List<GameObject> _BeeObjs = new List<GameObject>();
    //蜂生成いちオブジェクト(フェーズごと)
    [SerializeField] private GameObject[] _BeePosObj = new GameObject[3];
    /// <summary> 背景オブジェクトのカラースクリプト </summary>
    [SerializeField] private ColorObjectVer3 _BackGroundColorObjectVer3;


    //舌の動き制御
    private TongueScript _TongueScript;
    //透明化制御
    private TransparentScript _TransparentScript;
    //雷制御
    public LightningScript _LightningScript;
    //色切り替え制御
    //private SpriteRenderer _SpriteRenderer;
    private ColorObjectVer3 _ColorObjectVer3;

    //アニメーション
    private Animator _Animator;

    #endregion

    public int Phase
    {
        get
        {
            return _Phase;
        }
    }

    public int MaxPhase
    {
        get
        {
            return _MaxPhase;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _CurrentHP = _MaxHP;
        _BossSize = transform.localScale.x;
        _TongueScript = transform.Find("Tongue").GetComponent<TongueScript>();
        _DamageTimeInterval = 3.0f;
        //フェーズ更新
        _Phase = (_MaxHP - _CurrentHP) / (_MaxHP / (_MaxPhase + 1));
        if (_TongueScript == null)
            Debug.LogError("_TongueScript is null");
        _TransparentScript = GetComponent<TransparentScript>();
        //_ChangeBodyColor = GetComponent<ChangeBodyColor>();
        _ColorObjectVer3 = GetComponent<ColorObjectVer3>();
        _Animator = GetComponent<Animator>();
        _PhaseColorEnums = new ColorObjectVer3.OBJECT_COLOR3[]
        {
            ColorObjectVer3.OBJECT_COLOR3.GREEN,
            ColorObjectVer3.OBJECT_COLOR3.ORRANGE,
            ColorObjectVer3.OBJECT_COLOR3.GRAY
        };
        _PhaseBackColorEnums = new ColorObjectVer3.OBJECT_COLOR3[]
        {
            ColorObjectVer3.OBJECT_COLOR3.CYAN,
            ColorObjectVer3.OBJECT_COLOR3.ORRANGE,
            ColorObjectVer3.OBJECT_COLOR3.GRAY
        };
        //_SpriteRenderer = GetComponent<SpriteRenderer>();
        //色の設定
        _ColorObjectVer3.colorType = _PhaseColorEnums[_Phase];
        //判定も設定
        DamageOrNot(true);
        //背景色の設定
        _BackGroundColorObjectVer3.colorType = _PhaseBackColorEnums[_Phase];

        //タスクの登録
        _TaskList.DefineTask(TaskEnum.WALK, TaskWalkEnter, TaskWalkUpdate, TaskWalkExit);
        _TaskList.DefineTask(TaskEnum.TONGUE_ATTACK, TaskTongueAttackEnter, TaskTongueAttackUpdate, TaskTongueAttackExit);
        _TaskList.DefineTask(TaskEnum.TRANSPARENT, TaskTransparentEnter, TaskTransparentUpdate, TaskTransparentExit);
        _TaskList.DefineTask(TaskEnum.CHANGE_COLOR, TaskChangeColorEnter, TaskChangeColorUpdate, TaskChangeColorExit);
        _TaskList.DefineTask(TaskEnum.TRANSPARENT_END, TaskTransparentEndEnter, TaskTransparentEndUpdate, TaskTransparentEndExit);
        _TaskList.DefineTask(TaskEnum.TRANSLATE, TaskTranslateEnter, TaskTranslateUpdate, TaskTranslateExit);
        _TaskList.DefineTask(TaskEnum.RETURN_POS, TaskReturnPosEnter, TaskReturnPosUpdate, TaskReturnPosExit);
        _TaskList.DefineTask(TaskEnum.SCARED, TaskScaredEnter, TaskScaredUpdate, TaskScaredExit);
        _TaskList.DefineTask(TaskEnum.DAMAGE, TaskDamageEnter, TaskDamageUpdate, TaskDamageExit);
        _TaskList.DefineTask(TaskEnum.DOWN, TaskDownEnter, TaskDownUpdate, TaskDownExit);
        _TaskList.DefineTask(TaskEnum.WAIT, TaskWaitEnter, TaskWaitUpdate, TaskWaitExit);

        //_State = StateEnum.Move;

        //プレイヤーを向く
        TurnTo(_Player);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_State)
        {
            case StateEnum.None:
                float distance = (_Player.transform.position - transform.position).magnitude;
                if(distance <= BATTLE_START_DISTANCE)
                {
                    _State = StateEnum.Move;
                    //蜂1体追加
                    GameObject bee = Instantiate(_BeePrefab, _BeePosObj[_Phase].transform.position, Quaternion.identity);
                    bee.GetComponent<BeeScript>().Player = _Player;
                    _BeeObjs.Add(bee);
                }
                break;
            case StateEnum.Move:
                if (_TaskList.IsEnd)
                    CharaUpdate();
                _TaskList.UpdateTask();
                break;
            case StateEnum.Dead:
                break;
        }
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
                    Attack1();
                }
                else
                {
                    _TaskList.AddTask(TaskEnum.WALK);
                }
                break;
            case 2:
                //交互にやる
                if (_CharaUpdateCount % 4 == 0)
                {
                    _TaskList.AddTask(TaskEnum.TRANSPARENT);
                    _TaskList.AddTask(TaskEnum.TRANSLATE);
                    Wait(0.5f);
                    _TaskList.AddTask(TaskEnum.TRANSPARENT_END);
                }
                else
                {
                    Attack2();
                }
                break;
        }
        _CharaUpdateCount++;
    }

    #region タスク関数
    /// <summary> 歩き処理
    private void TaskWalkEnter()
    {
        //アニメーションは歩くに
        _Animator.SetBool("IsWalk", true);
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
        _Animator.SetBool("IsWalk", false);
    }

    /// <summary> 舌攻撃処理
    private void TaskTongueAttackEnter()
    {
        //アニメーション舌を出す
        _Animator.SetBool("IsAttack", true);
        //舌を動かし始める
        _TongueScript.StartStretch();
        //Debug.Log("Attack");
        //音ならす
        SoundManager.Instance.PlaySE("Strength");
    }

    private bool TaskTongueAttackUpdate()
    {
        return _TongueScript.IsFin;
    }

    private void TaskTongueAttackExit()
    {
        //アニメーションを止める
        _Animator.SetBool("IsAttack", false);
        //Debug.Log("Attack End");
        //舌関係全てリセット
        _TongueScript.ResetTongue();
    }

    /// <summary> 瞬間移動処理
    private void TaskTranslateEnter()
    {
        //待機に
        //移動
        int no = Random.Range(0, 3);
        Transform asibaTransform = _AsibaParentObj.transform.GetChild(no);
        Vector3 pos = new Vector3(asibaTransform.position.x, asibaTransform.position.y + asibaTransform.localScale.y + transform.localScale.y, 0);
        transform.position = pos;
    }

    private bool TaskTranslateUpdate()
    {
        return true;
    }

    private void TaskTranslateExit()
    {
        //アニメーションを止める
        //プレイヤーを向く
        TurnTo(_Player);
    }

    /// <summary> 透明化処理
    private void TaskTransparentEnter()
    {
        //待機に
        //透明化を始める
        _TransparentScript.StartTransparent(true);
        //判定が消える
        DamageOrNot(false);
        //音ならす
        SoundManager.Instance.PlaySE("FadeOut");
    }

    private bool TaskTransparentUpdate()
    {
        return _TransparentScript.IsFin;
    }

    private void TaskTransparentExit()
    {
        //アニメーションを止める
        //Debug.Log(_SpriteRenderer.color);
    }

    /// <summary> 色切り替え処理
    private void TaskChangeColorEnter()
    {
        //色変える
        _ColorObjectVer3.colorType = _PhaseColorEnums[_Phase];
        //色によって判定が消えるよ
        DamageOrNot(false);
    }

    private bool TaskChangeColorUpdate()
    {
        return true;
    }

    private void TaskChangeColorExit()
    {
        //Debug.Log(_SpriteRenderer.color);
    }

    /// <summary> 透明化終了処理
    private void TaskTransparentEndEnter()
    {
        //待機に
        //見えるようにする
        _TransparentScript.StartTransparent(false);
        //音ならす
        SoundManager.Instance.PlaySE("FadeIn");
    }

    private bool TaskTransparentEndUpdate()
    {
        return _TransparentScript.IsFin;
    }

    private void TaskTransparentEndExit()
    {
        //アニメーションを止める
        //背景と同じではないなら有効になるよ
        DamageOrNot(true);
    }

    /// <summary> ダメージ後の定位置に戻る処理
    private void TaskReturnPosEnter()
    {
        int no = Random.Range(0,_ReturnPosObjs.Length);
        transform.position = new Vector3(_ReturnPosObjs[no].transform.position.x, _ReturnPosObjs[no].transform.position.y, 0);
    }

    private bool TaskReturnPosUpdate()
    {
        return true;
    }

    private void TaskReturnPosExit()
    {
        //プレイヤーを向く
        TurnTo(_Player);
    }

    /// <summary> 怯える処理
    private void TaskScaredEnter()
    {
        //アニメーションは待機に
        _Animator.SetBool("IsScared", true);
        //雷の時は以下の処理をする
        //舌をしまう
        _TongueScript.ResetTongue();
        //怖がらせる
        foreach (GameObject bee in _BeeObjs)
        {
            bee.GetComponent<BeeScript>().Scared();
        }
        //判定あり
        DamageOrNot(true);
        //色を緑に
        _ColorObjectVer3.colorType = _PhaseColorEnums[0];

    }

    private bool TaskScaredUpdate()
    {
        return _LightningScript.IsFin;
    }

    private void TaskScaredExit()
    {
        ResetBoss();
        Wait(0.5f);
        //透明になって
        _TaskList.AddTask(TaskEnum.TRANSPARENT);
        //色を変える
        _TaskList.AddTask(TaskEnum.CHANGE_COLOR);
        //現れる
        _TaskList.AddTask(TaskEnum.TRANSPARENT_END);
        //アニメーションを止める
        _Animator.SetBool("IsScared", false);
    }

    /// <summary> ダメージ処理
    private void TaskDamageEnter()
    {
        //アニメーション
        _Animator.SetBool("IsDamage", true);
        //時間設定
        _TimerScript.ResetTimer(_DamageTimeInterval);
        //ダメージを与えられない
        DamageOrNot(false);
        //Debug.Log("Damage");
        //音
        SoundManager.Instance.PlaySE("Damage_2");
    }

    private bool TaskDamageUpdate()
    {
        _TimerScript.UpdateTimer();
        return _TimerScript.IsTimeUp;
    }

    private void TaskDamageExit()
    {
        //アニメーションを止める
        _Animator.SetBool("IsDamage", false);
        //Debug.Log("Damage End");
    }

    /// <summary> 死んだ処理
    private void TaskDownEnter()
    {
        //アニメーション
        _Animator.SetBool("IsDown", true);
        //ダメージ判定を消す
        DamageOrNot(false);
        ResetBoss();
        //雷関係リセット
        _LightningScript.FinishLightning();
        //蜂削除
        foreach (GameObject bee in _BeeObjs)
        {
            Destroy(bee);
        }
        _BeeObjs.Clear();
        //音
        SoundManager.Instance.PlaySE("Damage_1");
        //状態を死んだにする
        _State = StateEnum.Dead;
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
        //プレイヤーを向く
        TurnTo(_Player);
        //スクリプトの初期化
        _TongueScript.SetStretch();
        //リスト追加
        _TaskList.AddTask(TaskEnum.TONGUE_ATTACK);
    }

    public override void Attack3()
    {
        throw new System.NotImplementedException();
    }

    public override void Damage()
    {
        if (!_IsUnableBeAttacked)
        {
            //タスクを消して
            _TaskList.CancelAllTask();
            //ダメージにする
            _TaskList.AddTask(TaskEnum.DAMAGE);
            int beforePhase = _Phase;
            //フェーズ更新
            _Phase = (_MaxHP - _CurrentHP) / (_MaxHP / (_MaxPhase + 1));
            ResetBoss();
            //透明になって
            _TaskList.AddTask(TaskEnum.TRANSPARENT);
            Wait(0.5f);
            //色を変える
            _TaskList.AddTask(TaskEnum.CHANGE_COLOR);
            //もし新しいフェーズになっていたら
            if (_Phase != beforePhase)
            {
                //蜂1体追加
                GameObject bee = Instantiate(_BeePrefab, _BeePosObj[_Phase].transform.position, Quaternion.identity);
                bee.GetComponent<BeeScript>().Player = _Player;
                _BeeObjs.Add(bee);
                //ラストフェーズ
                if (_Phase == _MaxPhase)
                {
                    //したが高速に
                    _TongueScript.StrongerTongue();
                }

            }
            if (_Phase != 2)
            {
                //端に移動
                _TaskList.AddTask(TaskEnum.RETURN_POS);
                Wait(0.5f);
                //現れる
                _TaskList.AddTask(TaskEnum.TRANSPARENT_END);
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

    //ボスにダメージが入るか?
    public void DamageOrNot(bool isDamage)
    {
        //背景と同じだったら
        if (_ColorObjectVer3.colorType == _BackGroundColorObjectVer3.colorType)
        {
            //判定が消える
            isDamage = false;
        }
        //頭のスクリプトを切り替え
        transform.Find("Head").gameObject.GetComponent<Collider2D>().enabled = isDamage;
        //体のスクリプトを切り替え
        transform.Find("Body").gameObject.GetComponent<Collider2D>().enabled = isDamage;
    }

    /// <summary>
    /// 怯えさせる
    /// </summary>
    public void Scared()
    {
        _TaskList.CancelAllTask();
        //今透明だったら
        if(GetComponent<SpriteRenderer>().color.a != 1)
        {
            //見えるようになって
            _TaskList.AddTask(TaskEnum.TRANSPARENT_END);
        }
        //怯える
        _TaskList.AddTask(TaskEnum.SCARED);
    }

    //いったんの初期化に呼ぶ
    public void ResetBoss()
    {
        //舌関係リセット
        _TongueScript.ResetTongue();
        //雷関係リセット
        _LightningScript.FinishLightning();
        if (_Phase > _MaxPhase)
            _Phase = _MaxPhase;
        //雷準備
        _LightningScript.PrepareLightning(_Phase == 2);
        //キャラアップデートリセット
        _CharaUpdateCount = 0;
        //背景色の設定
        _BackGroundColorObjectVer3.colorType = _PhaseBackColorEnums[_Phase];
        //蜂リセット
        foreach (GameObject bee in _BeeObjs)
        {
            bee.GetComponent<BeeScript>().ResetBee();
        }
    }
}
