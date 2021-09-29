using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//色を持ってるオブジェクトにアタッチするクラス。消え得るオブジェクトを覆っていたら印を付けてあげる。
public class ColorObject2 : MonoBehaviour
{
    //いらない変数多すぎ、

    //色情報のenum
    public enum OBJECT_COLOR2
    {
        BLACK,      //黒(影の色)
        WHITE,      //白
        RED,        //赤色
        GREEN,      //緑色
        BLUE,       //青色
        CYAN,       //シアン
        MAGENTA,    //マゼンタ
        YELLOW,     //黄色
        PURPLE,     //紫
        ORRANGE,    //オレンジ色
        LIME,       //黄緑色
        GRAY,       //灰色
        BACKGROUND, //背景の色
    }
    public OBJECT_COLOR2 colorType;//色のタイプ

    
    public bool onRay;
    
    
    public bool noBody;
    int time = 0;
    
    
    
    public int objectNumber;
    public bool touchingObject;
    bool stopFlag;
    Vector3 lastPos;
    float lastRotZ;
    public GameObject BodyObject;
    float gravity;
    public bool active;     //プレイヤー専用の変数
    GameManagerScript gameManagerScript;
    
    public bool isChild;

    public bool onLightRay;         //光に当たってる判定
    public bool onShadowRay = true;        //影に当たってる判定
    bool touchingShadow;            //影に触れてる判定
    bool outOfShadow;               //影の外にいる判定

    public bool inSameColor;        //同じ色に入っているかの判定(FrameRayOfObjectスクリプトから操作)

    public bool isObject;           //ブロックとかのオブジェクトかどうか
    public bool forDebug = false;   //デバッグするならTrueにする
    public bool hasShadow;          //このオブジェクトに影ができるか
    public bool canHold;            //プレイヤーが持てるかの判定
    public bool onThePlayer;        //プレイヤーに持ち上げられてる判定


    // Start is called before the first frame update
    void Start()
    {
        outOfShadow = true;
        noBody = false;
        touchingShadow = false;
        touchingObject = false;
        stopFlag = false;
        active = true;
        if(gameObject.tag == "BackGround")gameObject.layer = LayerMask.NameToLayer("BACKGROUND");
        if(gameObject.tag == "Player")gameObject.layer = LayerMask.NameToLayer("Player");
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        if(isObject)
        {
            if(gameObject.tag != "Player")BodyObject = transform.Find("Body").gameObject;
            //Debug.Log("あれ？？");
            gravity = GetComponent<Rigidbody2D>().gravityScale;

            //this.hasShadow = true;
            BodyObject.transform.Find("ShadowCaster").gameObject.SetActive(this.hasShadow);
        }

        //色のタイプによって変色する
        switch(this.colorType)
        {
            case OBJECT_COLOR2.BLACK:
                GetComponent<SpriteRenderer>().color = new Color(0,0,0);
                break;

            case OBJECT_COLOR2.WHITE:
                GetComponent<SpriteRenderer>().color = new Color(1,1,1);
                break;

            case OBJECT_COLOR2.RED:
                GetComponent<SpriteRenderer>().color = new Color(1,0,0);
                break;

            case OBJECT_COLOR2.GREEN:
                GetComponent<SpriteRenderer>().color = new Color(59f/255f,175f/255f,117f/255f);
                break;
            
            case OBJECT_COLOR2.BLUE:
                GetComponent<SpriteRenderer>().color = new Color(0,0,1);
                break;

            case OBJECT_COLOR2.CYAN:
                GetComponent<SpriteRenderer>().color = new Color(0,156f/255f,209f/255f);
                break;
            
            case OBJECT_COLOR2.MAGENTA:
                GetComponent<SpriteRenderer>().color = new Color(228f/255f,0,127f/255f);
                break;

            case OBJECT_COLOR2.YELLOW:
                GetComponent<SpriteRenderer>().color = new Color(1,1,0);
                break;

            case OBJECT_COLOR2.PURPLE:
                GetComponent<SpriteRenderer>().color = new Color(167f/255f,87f/255f,168f/255f);

                break;
            case OBJECT_COLOR2.ORRANGE:
                GetComponent<SpriteRenderer>().color = new Color(1,165f/255f,0);
                break;

            case OBJECT_COLOR2.LIME:
                GetComponent<SpriteRenderer>().color = new Color(0,1,0);
                break;
            case OBJECT_COLOR2.GRAY:
                GetComponent<SpriteRenderer>().color = new Color(118f/255,118f/255f,118f/255f);
                break;

            default:break;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    void FixedUpdate()
    {
        //if(isChild)this.colorType = transform.root.GetComponent<ColorParentScript>().colorType;
        if(gameObject.tag != "BackGround" && gameObject.tag != "Player")gameObject.layer = LayerMask.NameToLayer(this.colorType.ToString());
        //if(forDebug)Debug.Log(gameObject.name + ":" + gameObject.layer);

        //背景じゃなくてステージ上にあるオブジェクトの場合の処理
        if(isObject)
        {
            time++;
            if(time == 60)
            {
                //onLightRay = false;
                //onShadowRay = false;
                time = 0;
            }

            //影が存在する場合としない場合で処理が若干変わる
            if(gameManagerScript.existShadow)
            {
                //if(touchingObject && noBody)noBody = true;
            }
            else 
            {
                onLightRay = true;
            }
            
            if(onLightRay && onShadowRay)touchingShadow = true;
            if(!onShadowRay)outOfShadow = true;

            
            //長すぎ、enumの値とHashMapとか使えばいけると思う。
            //関数も使えるよね。
            switch(this.colorType)
            {
                    //色が黒色の場合
                    case OBJECT_COLOR2.BLACK:
                        GetComponent<SpriteRenderer>().color = new Color(0,0,0);
                        //影が存在する時
                        if(gameManagerScript.existShadow)
                        {
                            noBody = false;
                            //影の中にいる時は実体が消える
                            if(!outOfShadow)noBody = true;
                        }
                        //影が存在しない時
                        else
                        {
                            if(inSameColor)
                            {
                                if(!touchingShadow)
                                {
                                    noBody = true;
                                }
                                //Debug.Log("おんなじ色");
                            }
                            //被ってなかったら消えない
                            else 
                            {
                                noBody = false;
                            }
                            
                            if(touchingShadow)noBody = false;
                        }
                        
                        //逆にそれ以外は実体は消えない
                        //else noBody = false;
                        //BodyObject.SetActive(!noBody);
                        break;
                    
                    //色が白色の場合
                    case OBJECT_COLOR2.WHITE:
                        //Debug.Log("白色の処理");
                        GetComponent<SpriteRenderer>().color = new Color(1,1,1);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色が赤色の場合
                    case OBJECT_COLOR2.RED:
                        GetComponent<SpriteRenderer>().color = new Color(1,0,0);
                        //影の中にいる場合は実態が残る
                        if(!outOfShadow)noBody = false;
                        BodyObject.SetActive(!noBody);
                        break;

                    //色が緑色の場合
                    case OBJECT_COLOR2.GREEN:
                        GetComponent<SpriteRenderer>().color = new Color(59f/255f,175f/255f,117f/255f);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色が青色の場合
                    case OBJECT_COLOR2.BLUE:
                        GetComponent<SpriteRenderer>().color = new Color(0,0,1);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色がシアンの場合
                    case OBJECT_COLOR2.CYAN:
                        GetComponent<SpriteRenderer>().color = new Color(0,156f/255f,209f/255f);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色がマゼンタの場合
                    case OBJECT_COLOR2.MAGENTA:
                        GetComponent<SpriteRenderer>().color = new Color(228f/255f,0,127f/255f);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色が黄色の場合
                    case OBJECT_COLOR2.YELLOW:
                        GetComponent<SpriteRenderer>().color = new Color(1,1,0);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色が紫色の場合
                    case OBJECT_COLOR2.PURPLE:
                        GetComponent<SpriteRenderer>().color = new Color(167f/255f,87f/255f,168f/255f);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色がオレンジ色の場合
                    case OBJECT_COLOR2.ORRANGE:
                        GetComponent<SpriteRenderer>().color = new Color(1,165f/255f,0);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色が黄緑色の場合
                    case OBJECT_COLOR2.LIME:
                        GetComponent<SpriteRenderer>().color = new Color(0,1,0);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;

                    //色が灰色の場合
                    case OBJECT_COLOR2.GRAY:
                        GetComponent<SpriteRenderer>().color = new Color(118f/255,118f/255f,118f/255f);
                        //同色オブジェクトと被ってたら消える
                        if(inSameColor)
                        {
                            if(!touchingShadow)
                            {
                                noBody = true;
                            }
                            //Debug.Log("おんなじ色");
                        }
                        //被ってなかったら消えない
                        else 
                        {
                            noBody = false;
                        }
                        
                        if(touchingShadow)noBody = false;
                        break;
                    //色が黒色の場合
                    case OBJECT_COLOR2.BACKGROUND:
                        noBody = false;
                        //影の中にいる時は実体が消える
                        if(!outOfShadow)noBody = true;
                        //逆にそれ以外は実体は消えない
                        //else noBody = false;
                        //BodyObject.SetActive(!noBody);
                        break;

                    default:break;
            }
            
            

            if(!active)noBody = false;

            //影に関する処理
            /*
                //影の外にいるなら()
                if(outOfShadow)
                {
                    //if(time % 60 == 0)
                    //Debug.Log("日向にいるよ");
                    //GetComponent<SpriteRenderer>().color = new Color(255,0,0);
                    if(this.colorType == OBJECT_COLOR2.BLACK)
                    {
                        noBody = true;
                    }
                }
                else
                {
                    //if(time % 60 == 0)
                    //Debug.Log("影の中");
                    //GetComponent<SpriteRenderer>().color = new Color(255,255,255);
                    if(this.colorType == OBJECT_COLOR2.BLACK)
                    {
                        noBody = false;
                    }
                }

                //背景の同色オブジェクトと被ってたら
                if(inSameColor)
                {
                    //Debug.Log("同じ色");
                    BodyObject.SetActive(false);
                    noBody = true;
                }
                else 
                {
                    BodyObject.SetActive(true);
                    noBody = false;
                }
                //
            
            
            */

            //BodyObject.SetActive(noBody);

            //onRay = false;
            //inSameColor = false;
            //outOfShadow = false;
            
        }


        switch(this.colorType)
        {
            case OBJECT_COLOR2.BLACK:
                GetComponent<SpriteRenderer>().color = new Color(0,0,0);
                break;

            case OBJECT_COLOR2.WHITE:
                GetComponent<SpriteRenderer>().color = new Color(1,1,1);
                break;

            case OBJECT_COLOR2.RED:
                GetComponent<SpriteRenderer>().color = new Color(1,0,0);
                break;

            case OBJECT_COLOR2.GREEN:
                GetComponent<SpriteRenderer>().color = new Color(59f/255f,175f/255f,117f/255f);
                break;
                
            case OBJECT_COLOR2.BLUE:
                GetComponent<SpriteRenderer>().color = new Color(0,0,1);
                break;

            case OBJECT_COLOR2.CYAN:
                GetComponent<SpriteRenderer>().color = new Color(0,156f/255f,209f/255f);
                break;
                
            case OBJECT_COLOR2.MAGENTA:
                GetComponent<SpriteRenderer>().color = new Color(228f/255f,0,127f/255f);
                break;

            case OBJECT_COLOR2.YELLOW:
                GetComponent<SpriteRenderer>().color = new Color(1,1,0);
                break;

            case OBJECT_COLOR2.PURPLE:
                GetComponent<SpriteRenderer>().color = new Color(167f/255f,87f/255f,168f/255f);
                break;

            case OBJECT_COLOR2.ORRANGE:
                GetComponent<SpriteRenderer>().color = new Color(1,165f/255f,0);
                break;

            case OBJECT_COLOR2.LIME:
                GetComponent<SpriteRenderer>().color = new Color(0,1,0);
                break;
    
            case OBJECT_COLOR2.GRAY:
                GetComponent<SpriteRenderer>().color = new Color(118f/255,118f/255f,118f/255f);
                break;

            default:break;
        }
        
        if(isObject)
        {
            //光にしか当たってない。
            if(onLightRay && !onShadowRay)
            {
                if(inSameColor)noBody = true;
            }

            //光にも影にも当たってる
            if(onShadowRay)noBody = false;

            //光が当たってない
            if(!onLightRay)
            {
                if(colorType == OBJECT_COLOR2.BLACK)noBody = true;
                else noBody = false;
            }
            ControlBody(noBody);
        }
        
    }


    //Bodyの処理
    void ControlBody(bool flag)
    {
        BodyObject.SetActive(!flag);

        //もしTrue(実体なし)だったら
        Vector3 pos = transform.position;
        if(flag || touchingObject)
        {

           
            pos.z = 10;
        }
        else
        {
            //GetComponent<Rigidbody2D>().gravityScale = gravity;
            Color nowColor = GetComponent<SpriteRenderer>().color;
            nowColor.a = 1;
            //GetComponent<SpriteRenderer>().color = nowColor;
            pos.z = 0;
        }
        transform.position = pos;

        if(onThePlayer)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            GetComponent<Rigidbody2D>().gravityScale = 0;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            GetComponent<Rigidbody2D>().mass = 0;
        }
        else 
        {
            GetComponent<Rigidbody2D>().gravityScale = gravity;
            GetComponent<Rigidbody2D>().mass = 2;
        }
    }
    

    //引数の値を最低値と最高値に修正するメソッド
    float bound(float min,float max,float value)
    {
        if(value < min)return min;
        if(max < value)return max;
        return value;
    }

    /*
    関数
    */
}
