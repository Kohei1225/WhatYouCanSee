using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//プレイヤーを操作するためのクラス
public class PlayerController : MonoBehaviour
{
    public float vel;
    public float vForce;
    bool walk;
    public bool jump;
    int vec;                    //向いてる方向を示す
    public bool onStage;
    public bool damage;
    float scale;
    Animator animController;
    public bool isHoldingObject;//今オブジェクトを運んでるかどうかの判定
    public GameObject objectBeingHolden;
    const float PLAYERX = 0.3099501f;
    const float PLAYERTOP = 0.1456f;
    public GameObject objectToHold;
    HoldObjectScript holdObject;
    float throwPower = 1500;

    // Start is called before the first frame update
    void Start()
    {
        walk = false;
        jump = false;
        vec = 1;
        isHoldingObject = false;
        onStage = false;
        scale = transform.localScale.y;
        animController = GetComponent<Animator>();
        holdObject = objectToHold.GetComponent<HoldObjectScript>();
    }

    // Update is called once per frame
    void Update()
    {
        walk = false;

        GetComponent<ColorObjectScript>().active = Input.GetKey(KeyCode.Z);

        if(!GetComponent<ColorObjectScript>().active && !damage)
        {
            //キーが押されたかを保存
            jump = Input.GetKeyDown(KeyCode.UpArrow);

            //右を向く
            if(Input.GetKey(KeyCode.RightArrow))
            {
                vec = 1;
                walk = true;
            }
            
            //左を向く
            if(Input.GetKey(KeyCode.LeftArrow))
            {
                vec = -1;
                walk = true;
            }

            //地面にいる状態ならジャンプできる
            if(jump && onStage)
            {
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0,vForce));
            }

            if(Input.GetKeyDown(KeyCode.X))
            {
                //目の前にブロックがあって何も持ち上げてない時
                if(holdObject.objectFrontMe && !isHoldingObject)
                {
                    //目の前のブロックを持ち上げる
                    isHoldingObject = true;
                    objectBeingHolden = holdObject.objectFrontMe;
                    objectBeingHolden.transform.root.GetComponent<ColorObject2>().onThePlayer = true;
                }
                //ブロックを持ち上げてる時
                else if(isHoldingObject)
                {
                    //投げる向いてる方向に投げる
                    Vector3 throwVec = new Vector3(vec*throwPower, throwPower*1.5f, 0);
                    objectBeingHolden.transform.root.GetComponent<Rigidbody2D>().AddForce(throwVec);

                    //何も持ってない状態にする
                    isHoldingObject = false;
                    objectBeingHolden.transform.root.GetComponent<ColorObject2>().onThePlayer = false;
                    objectBeingHolden = null;
                }
            }
        }
        else 
        {
            if(isHoldingObject)objectBeingHolden.transform.root.gameObject.GetComponent<ColorObject2>().onThePlayer = false;
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
            if(!objectBeingHolden.transform.root.GetComponent<ColorObject2>().noBody)
            {
                Vector3 pos = transform.position;
                pos.y = transform.position.y + (transform.localScale.y * PLAYERTOP) + (objectBeingHolden.transform.localScale.y) + 0.6f;
                objectBeingHolden.transform.root.position = pos;
                                                        //元々はColorObjectScript
                objectBeingHolden.transform.root.GetComponent<ColorObject2>().onThePlayer = true;
            }
            //持ってるオブジェクトが見えなくなった時
            else 
            {                                           //元々はColorObjectScript
                objectBeingHolden.transform.root.GetComponent<ColorObject2>().onThePlayer = false;
                isHoldingObject = false;
                objectBeingHolden = null;     
            }       
        }

        transform.localScale = new Vector3(vec * scale,scale,1);
        animController.SetBool("Nothing",GetComponent<ColorObjectScript>().active);
        animController.SetBool("Damage",damage);
        animController.SetFloat("Abs_V_Vel",Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
        animController.SetBool("Walk",walk);
        animController.SetBool("OnStage",onStage);
    }
}
