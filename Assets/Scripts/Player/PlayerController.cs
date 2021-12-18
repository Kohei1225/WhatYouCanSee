using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//プレイヤーを操作するためのクラス
public class PlayerController : MonoBehaviour
{
    /// <summary> 移動速度 </summary>
    public float moveVel = 17;

    /// <summary> ジャンプスピード </summary>
    public float jumpSpeed = 35f;

    public bool active{get;private set;}

    /// <summary> ジャンプできる判定用 </summary>
    public bool jump;
    /// <summary> ダメージを受けたかを判定する用 </summary>
    public bool damage{get; private set;}
    /// <summary> 向いてる方向を示す変数 </summary>
    int vec;
    /// <summary> 何かの上に乗ってるかを判定する用 </summary>
    public bool onStage;
    /// <summary>  </summary>
    private bool canCtrl = true;
    
    float scaleX;
    /// <summary> 投げるときに加える力  </summary>
    float throwPower = 1500;
    /// <summary> 今オブジェクトを運んでるかどうかの判定 </summary>
    [HideInInspector] public bool isHoldingObject;
    /// <summary> 持ってるオブジェクト </summary>
    [HideInInspector] public GameObject objectBeingHolden;
    /// <summary> 持ち上げる時に使う自身の子オブジェクト </summary>
    GameObject objectToHold;        
    HoldObjectScript holdScript;    
    Animator animController;        //アニメーター
    GameObject topOfHead;           //頭のてっぺんに置いてあるオブジェクト
    Rigidbody2D rigidBody;
    [SerializeField] GameObject _ParentEnptyPrefab;

    //体力
    [SerializeField] private int _Life = 3;
    //ダメージを受けるか
    private bool _CanDamage = true;
    //ダメージ後の無敵時間
    [SerializeField] private float _MutekiTime = 3;
    //ダメージ時間測定用
    private float _DamageTime = 0;
    //スプライトレンダラー
    private SpriteRenderer _SpriteRenderer;
    //点滅の回数
    [SerializeField] private int _ChangeAlphaNum = 15;
    //一回のアルファ値を変えるのにどのくらいの時間を使うのか
    private float _TimePerChangeAlpha;
    //何回点滅したか
    private int _NowChangeCount = 0;

    //移動範囲X
    [SerializeField] private Vector2 _MoveRangeX = new Vector2(-50, 50);
    //死ぬ高さ
    [SerializeField] private float _DeathY = -30;
    //死んでからマスクが小さくなるまでの時間
    [SerializeField] private float _DeathWaitTime = 4;
    //時間測定用
    private TimerScript _TimerScript = new TimerScript();
    //マスクマネージャー
    private MaskManager _MaskManager;

    public int Life
    {
        get
        {
            return _Life;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        jump = false;
        vec = 1;
        active = false;
        isHoldingObject = false;
        onStage = false;
        scaleX = Mathf.Abs(transform.localScale.x);
        animController = GetComponent<Animator>();
        objectToHold = transform.Find("Body").transform.Find("CatchArea").gameObject;
        holdScript = objectToHold.GetComponent<HoldObjectScript>();
        topOfHead = transform.Find("TopOfHead").gameObject;
        rigidBody = GetComponent<Rigidbody2D>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _TimePerChangeAlpha = _MutekiTime / _ChangeAlphaNum;
        _MaskManager = transform.Find("CircleMask")?.GetComponent<MaskManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canCtrl)
            return;

        bool walk = false;

        if(Input.GetKey(KeyCode.Z))active = true;

        GetComponent<ColorObjectVer3>().Set_active(Input.GetKey(KeyCode.Z));

        if(!GetComponent<ColorObjectVer3>().Get_active() && !damage)
        {
            //キーが押されたかを保存
            jump = Input.GetKeyDown(KeyCode.UpArrow);


            walk = Input.GetKey(KeyCode.RightArrow)|Input.GetKey(KeyCode.LeftArrow);
            if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                if(Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))walk = false;
                else if(Input.GetKey(KeyCode.RightArrow))vec = 1;//右を向く
                else vec = -1;
            }
            //左右どちらかが入力されてればその方向に移動
            if (walk)
            {
                //移動範囲内なら
                if (transform.position.x >= _MoveRangeX.x && transform.position.x <= _MoveRangeX.y)
                {
                    transform.Translate(moveVel * vec * Time.deltaTime, 0, 0);
                }
                else
                {
                    //強制的に位置を戻す
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x, _MoveRangeX.x, _MoveRangeX.y), transform.position.y, transform.position.z);
                }
            }

            //持ち上げたり投げたりする処理
            if (Input.GetKeyDown(KeyCode.X))
            {
                //目の前にブロックがあって何も持ち上げてない時
                if(holdScript.Get_objectFrontMe() && !isHoldingObject)
                {
                    //目の前のブロックを持ち上げる
                    isHoldingObject = true;
                    objectBeingHolden = holdScript.Get_objectFrontMe();
                    objectBeingHolden.GetComponent<ColorObjectVer3>().Set_onThePlayer(true);
                    //音ならす
                    SoundManager.Instance.PlaySE("Bring");
                }
                //ブロックを持ち上げてる時
                else if(isHoldingObject)
                {
                    //投げる向いてる方向に投げる
                    Vector3 throwVec = new Vector3(vec*throwPower, throwPower*1.5f, 0);
                    objectBeingHolden.GetComponent<Rigidbody2D>().AddForce(throwVec);

                    //もしこれがダメージを与えるブロックだったら
                    DamageBlockScript damageBlockScript = objectBeingHolden.GetComponent<DamageBlockScript>();
                    if (damageBlockScript != null)
                    {
                        //ダメージを与えられる状態にする
                        damageBlockScript._CanDamage = true;
                    }

                    //何も持ってない状態にする
                    isHoldingObject = false;
                    objectBeingHolden.GetComponent<ColorObjectVer3>().Set_onThePlayer(false);
                    objectBeingHolden = null;

                    //音ならす
                    SoundManager.Instance.PlaySE("Throw");
                }
            }
        }
        else 
        {
            if(isHoldingObject)objectBeingHolden.GetComponent<ColorObjectVer3>().Set_onThePlayer(false);
            isHoldingObject = false;
            objectBeingHolden = null;
        }

        if(damage)
        {
            rigidBody.velocity = new Vector2(0,0);
            if (_TimerScript.IsTimeUp && !_MaskManager.IsFin)
            {
                //マスクマネージャーを開始
                _MaskManager.StartMask(true);
            }
            else
            {
                //タイマー計測
                _TimerScript.UpdateTimer();
                if (_MaskManager.IsFin)
                    //シーン読み込み
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        animController.SetBool("Walk", walk);

        //地面にいるかを取得
        onStage = transform.Find("Body").gameObject.transform.Find("Foot").gameObject.GetComponent<FootAreaScript>().touchingStage;

        //地面にいる状態ならジャンプできる
        if(jump && onStage)
        {
            var vel = rigidBody.velocity;

            if(isHoldingObject)
                vel.y = jumpSpeed*0.75f;
            else
                vel.y = jumpSpeed;

            rigidBody.velocity = vel;
            jump = false;
            //音ならす
            SoundManager.Instance.PlaySE("Player_Jump");
        }

        //無敵時間中の処理
        if (!_CanDamage && !damage)
        {
            _DamageTime += Time.deltaTime;
            Color color = _SpriteRenderer.color;
            //点滅する時間なら
            if (_DamageTime / (_TimePerChangeAlpha * (_NowChangeCount + 1)) >= 1)
            {
                //アルファ値を反転
                int newAlpha = (int)color.a ^ 1;
                _SpriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
                _NowChangeCount++;
            }
            if (_DamageTime >= _MutekiTime)
            {
                _CanDamage = true;
                //透明度を1に
                _SpriteRenderer.color = new Color(color.r, color.g, color.b, 1);
            }
        }

        //Zキーが押された時はタグをカラーオブジェクトにする
        if (GetComponent<ColorObjectVer3>().Get_active())
        {
            gameObject.tag = "ColorObject";
        }
        else
        {
            gameObject.tag = "Player";
        }
    }

    void FixedUpdate()
    {
        if (!canCtrl)
            return;

        //落ちたなら
        if(transform.position.y < _DeathY && !damage)
        {
            //死ぬ
            Death();
        }

        //何か持ってる時
        if(isHoldingObject)
        {
            //持ってるオブジェクトが見えてる時              //元々はColorObjectScript
            if(!objectBeingHolden.GetComponent<ColorObjectVer3>().Get_noBody())
            {
                Vector3 pos = topOfHead.transform.position;
                pos.y += objectBeingHolden.transform.localScale.y/2;
                objectBeingHolden.transform.position = pos;
                                                        //元々はColorObjectScript
                objectBeingHolden.GetComponent<ColorObjectVer3>().Set_onThePlayer(true);
            }
            //持ってるオブジェクトが見えなくなった時
            else 
            {                                           //元々はColorObjectScript
                objectBeingHolden.GetComponent<ColorObjectVer3>().Set_onThePlayer(false);
                isHoldingObject = false;
                objectBeingHolden = null;     
            }       
        }

        GameObject obj = transform.Find("Body").gameObject?.transform.Find("Foot")?.gameObject.GetComponent<AreaInObj>().Obj;
        if (obj != null)
        {
            if(obj.transform.parent?.GetComponent<ColorObjectVer3>() && transform.parent == null && obj.transform.parent?.GetComponent<Rigidbody2D>())
            {
                //そのオブジェクトが親になってくれるなら
                if (obj.transform.parent.GetComponent<ColorObjectVer3>().isPlayersParent && obj.transform.parent.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
                {
                    transform.SetParent(obj.transform.parent);
                    scaleX = Mathf.Abs(transform.localScale.x);
                }
            }
        }
        else
        {
            if (!GetComponent<ColorObjectVer3>().Get_active())
            {
                transform.SetParent(null);
                scaleX = Mathf.Abs(transform.localScale.x);
            }
        }

        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(vec * scaleX,scale.y,scale.z);
        animController.SetBool("Nothing",GetComponent<ColorObjectVer3>().Get_active());
        animController.SetBool("Damage",damage);
        animController.SetFloat("Abs_V_Vel",Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));

        animController.SetBool("OnStage",onStage);
    }

    public void Damage()
    {
        //死んでいたら無視
        if (damage)
            return;
        if (!_CanDamage)
            return;
        _DamageTime = 0;
        _NowChangeCount = 0;
        _Life--;
        _CanDamage = false;
        if(_Life == 0)
        {
            //死んだ
            Death();
        }
        else
        {
            //音ならす
            SoundManager.Instance.PlaySE("PlayerDamage");
        }
    }

    public void Death()
    {
        damage = true;
        //音ならす
        SoundManager.Instance.PlaySE("AfterDeath");
        //タイマーセット
        _TimerScript.ResetTimer(_DeathWaitTime);
        //マスクをリセット
        _MaskManager.IsFin = false;
        //BGMを止める
        SoundManager.Instance.StopBGM();
        //死んだら動かなくする
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void Set_onStage(bool value)
    {
        this.onStage = value;
    }

    public bool Get_isHoldingObject()
    {
        return this.isHoldingObject;
    }

    public void Set_canCtrl(bool canCtrl)
    {
        this.canCtrl = canCtrl;
    }

    public bool Get_canCtrl()
    {
        return canCtrl;
    }
}
