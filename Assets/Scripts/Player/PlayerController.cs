using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーを操作するためのクラス
public class PlayerController : MonoBehaviour
{
    public float vel;           //歩く速度
    public float vForce;        //ジャンプする時に加える力

    bool active;
    bool walk;                  //歩ける判定用  
    [SerializeField] bool jump;                  //ジャンプできる判定用
    int vec;                    //向いてる方向を示す変数
    [SerializeField] bool onStage;               //何かの上に乗ってるかを判定する用
    bool damage;                //ダメージを受けたかを判定する用
    float scale;           
    float throwPower = 1500;        //投げるときに加える力 
    bool isHoldingObject;           //今オブジェクトを運んでるかどうかの判定
    GameObject objectBeingHolden;   //持ってるオブジェクト
    GameObject objectToHold;        //持ち上げる時に使う自身の子オブジェクト
    HoldObjectScript holdScript;    
    Animator animController;        //アニメーター
    GameObject topOfHead;           //頭のてっぺんに置いてあるオブジェクト

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
    }

    // Update is called once per frame
    void Update()
    {
        
        walk = false;
        if(Input.GetKey(KeyCode.Z))active = true;

        GetComponent<ColorObjectVer3>().Set_active(Input.GetKey(KeyCode.Z));

        if(!GetComponent<ColorObjectVer3>().Get_active() && !damage)
        {
            //キーが押されたかを保存
            jump = Input.GetKeyDown(KeyCode.UpArrow);

            if (jump && onStage)
            {
                if (isHoldingObject)
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, vForce * 0.75f));
                else
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, vForce));
                jump = false;
            }

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
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        }
        
    }

    void FixedUpdate()
    {
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
}
