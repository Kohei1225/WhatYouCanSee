using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーを操作するためのクラス
public class PlayerController : MonoBehaviour
{
    /// <summary>
    ///  移動速度
    /// </summary>
    public float vel;

    /// <summary>
    /// ジャンプスピード
    /// </summary>
    public float jumpSpeed = 35f;        //ジャンプする時に加える力

    public bool active{get;private set;}
    bool walk;                  //歩ける判定用  
    public bool jump;                  //ジャンプできる判定用
    public bool damage{get; private set;}  //ダメージを受けたかを判定する用
    int vec;                    //向いてる方向を示す変数
    public bool onStage;               //何かの上に乗ってるかを判定する用
    private bool canCtrl = true;
    
    float scale;           
    float throwPower = 1500;        //投げるときに加える力 
    bool isHoldingObject;           //今オブジェクトを運んでるかどうかの判定
    GameObject objectBeingHolden;   //持ってるオブジェクト
    GameObject objectToHold;        //持ち上げる時に使う自身の子オブジェクト
    HoldObjectScript holdScript;    
    Animator animController;        //アニメーター
    GameObject topOfHead;           //頭のてっぺんに置いてあるオブジェクト
    Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        walk = false;
        jump = false;
        vec = 1;
        active = false;
        isHoldingObject = false;
        onStage = false;
        scale = transform.localScale.y;
        animController = GetComponent<Animator>();
        objectToHold = transform.Find("Body").transform.Find("CatchArea").gameObject;
        holdScript = objectToHold.GetComponent<HoldObjectScript>();
        topOfHead = transform.Find("TopOfHead").gameObject;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canCtrl)
            return;

        walk = false;
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
            
            //持ち上げたり投げたりする処理
            if(Input.GetKeyDown(KeyCode.X))
            {
                //目の前にブロックがあって何も持ち上げてない時
                if(holdScript.Get_objectFrontMe() && !isHoldingObject)
                {
                    //目の前のブロックを持ち上げる
                    isHoldingObject = true;
                    objectBeingHolden = holdScript.Get_objectFrontMe();
                    objectBeingHolden.GetComponent<ColorObjectVer3>().Set_onThePlayer(true);
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
        }

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
        }
    }

    void FixedUpdate()
    {
        if (!canCtrl)
            return;

        //左右どちらかが入力されてればその方向に移動
        if(walk)
        {
            transform.Translate(vel * vec,0,0);
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

        transform.localScale = new Vector3(vec * scale,scale,1);
        animController.SetBool("Nothing",GetComponent<ColorObjectVer3>().Get_active());
        animController.SetBool("Damage",damage);
        animController.SetFloat("Abs_V_Vel",Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
        animController.SetBool("Walk",walk);
        animController.SetBool("OnStage",onStage);
    }

    public void Set_onStage(bool value)
    {
        this.onStage = value;
    }

    public void Set_damage(bool value)
    {
        this.damage = value;
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
