using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//色を持ってるオブジェクトにアタッチするクラス。消え得るオブジェクトを覆っていたら印を付けてあげる。
public class ColorObjectScript : MonoBehaviour
{
    public enum OBJECT_COLOR
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
    public OBJECT_COLOR colorType;//色のタイプ

    
    public bool onRay;
    public bool outOfShadow;//影の外にいるかどうかの判定(RayScriptで操作)
    public bool inSameColor;//同じ色に入っているかの判定(EdgeCheckerで操作する)
    public bool noBody;
    public int rayTime;
    int time = 0;
    public bool isObject;
    public bool forDebug = false;
    public bool touchingShadow;//影に触れてるかの判定
    public int objectNumber;
    public bool touchingObject;
    bool stopFlag;
    Vector3 lastPos;
    float lastRotZ;
    public GameObject BodyObject;
    public float gravity;
    public bool active;     //プレイヤー専用の変数
    GameManagerScript gameManagerScript;
    public bool hasShadow;
    public bool canHold;
    public bool onThePlayer;
    public bool isChild;

    // Start is called before the first frame update
    void Start()
    {
        rayTime = 0;
        outOfShadow = true;
        noBody = false;
        touchingShadow = false;
        touchingObject = false;
        stopFlag = false;
        active = true;
        if(gameObject.tag == "BackGround")gameObject.layer = LayerMask.NameToLayer("BACKGROUND");
        if(gameObject.tag == "Player")gameObject.layer = LayerMask.NameToLayer("Player");
        gameManagerScript = GameObject.Find("Managers").GetComponent<GameManagerScript>();
        if(isObject)
        {
            if(gameObject.tag != "Player")BodyObject = transform.Find("Body").gameObject;
            //Debug.Log("あれ？？");
            gravity = GetComponent<Rigidbody2D>().gravityScale;

            //this.hasShadow = true;
            BodyObject.transform.Find("ShadowCaster").gameObject.SetActive(this.hasShadow);
            //if(BodyObject.transform.Find("ShadowCaster") == null)Debug.Log(gameObject.name);
        }

        switch(this.colorType)
        {
            case OBJECT_COLOR.BLACK:
                GetComponent<SpriteRenderer>().color = new Color(0,0,0);
                //Debug.Log("黒wwww");
                break;

            case OBJECT_COLOR.WHITE:
                GetComponent<SpriteRenderer>().color = new Color(1,1,1);
                break;

            case OBJECT_COLOR.RED:
                GetComponent<SpriteRenderer>().color = new Color(1,0,0);
                //Debug.Log("黒wwww");
                break;

            case OBJECT_COLOR.GREEN:
                GetComponent<SpriteRenderer>().color = new Color(59f/255f,175f/255f,117f/255f);
                break;
            
            case OBJECT_COLOR.BLUE:
                GetComponent<SpriteRenderer>().color = new Color(0,0,1);
                //Debug.Log("黒wwww");
                break;

            case OBJECT_COLOR.CYAN:
                GetComponent<SpriteRenderer>().color = new Color(0,156f/255f,209f/255f);
                break;
            
            case OBJECT_COLOR.MAGENTA:
                GetComponent<SpriteRenderer>().color = new Color(228f/255f,0,127f/255f);
                //Debug.Log("黒wwww");
                break;

            case OBJECT_COLOR.YELLOW:
                GetComponent<SpriteRenderer>().color = new Color(1,1,0);
                break;

            case OBJECT_COLOR.PURPLE:
                GetComponent<SpriteRenderer>().color = new Color(167f/255f,87f/255f,168f/255f);

                //Debug.Log("黒wwww");
                break;
            case OBJECT_COLOR.ORRANGE:
                GetComponent<SpriteRenderer>().color = new Color(1,165f/255f,0);
                break;

            case OBJECT_COLOR.LIME:
                GetComponent<SpriteRenderer>().color = new Color(0,1,0);
                //Debug.Log("黒wwww");
                break;
            case OBJECT_COLOR.GRAY:
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
        if(isChild)this.colorType = transform.root.GetComponent<ColorParentScript>().colorType;
        if(gameObject.tag != "BackGround" && gameObject.tag != "Player")gameObject.layer = LayerMask.NameToLayer("Color");
        //if(forDebug)Debug.Log(gameObject.name + ":" + gameObject.layer);

        //背景じゃなくてステージ上にあるオブジェクトの場合の処理
        if(isObject)
        {
            time++;
            if(time == 360)time = 0;
            if(rayTime < 10)rayTime++;

            /*
            if(onRay)
            {
                rayTime = 0;
                outOfShadow = true;
                //if(forDebug)Debug.Log("影の外！！");
            }
            else outOfShadow = false;
            */

            //if(rayTime == 10 && !onRay)outOfShadow = false;
            if(!active)noBody = false;

            //影が存在する場合としない場合で処理が若干変わる
            if(gameManagerScript.existShadow)
            {
                if(touchingObject && noBody)noBody = true;
            }
            else 
            {
                if(touchingObject && noBody && !touchingShadow)noBody = true;
            }
            
            if(!noBody || noBody)
            {
                switch(this.colorType)
                {
                    //色が黒色の場合
                    case OBJECT_COLOR.BLACK:
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
                    case OBJECT_COLOR.WHITE:
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

                    //背景の色と同色の場合
                    case OBJECT_COLOR.RED:
                        GetComponent<SpriteRenderer>().color = new Color(1,0,0);
                        //影の中にいる場合は実態が残る
                        if(!outOfShadow)noBody = false;
                        BodyObject.SetActive(!noBody);
                        break;

                    //色が緑色の場合
                    case OBJECT_COLOR.GREEN:
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
                    case OBJECT_COLOR.BLUE:
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
                    case OBJECT_COLOR.CYAN:
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
                    case OBJECT_COLOR.MAGENTA:
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
                    case OBJECT_COLOR.YELLOW:
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
                    case OBJECT_COLOR.PURPLE:
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
                    case OBJECT_COLOR.ORRANGE:
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
                    case OBJECT_COLOR.LIME:
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
                    case OBJECT_COLOR.GRAY:
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
                    case OBJECT_COLOR.BACKGROUND:
                        noBody = false;
                        //影の中にいる時は実体が消える
                        if(!outOfShadow)noBody = true;
                        //逆にそれ以外は実体は消えない
                        //else noBody = false;
                        //BodyObject.SetActive(!noBody);
                        break;

                    default:break;
                }
            }

            if(!active)noBody = false;
            //if(noBody && touchingObject)noBody = true;
            lastPos = transform.position;
            lastRotZ = transform.rotation.z;
            if(stopFlag)
            {

            }
            ControlBody(noBody);
            //inSameColor = false;
            //onRay = false;

            /*
            //影の外にいるなら()
            if(outOfShadow)
            {
                //if(time % 60 == 0)
                //Debug.Log("日向にいるよ");
                //GetComponent<SpriteRenderer>().color = new Color(255,0,0);
                if(this.colorType == OBJECT_COLOR.BLACK)
                {
                    noBody = true;
                }
            }
            else
            {
                //if(time % 60 == 0)
                //Debug.Log("影の中");
                //GetComponent<SpriteRenderer>().color = new Color(255,255,255);
                if(this.colorType == OBJECT_COLOR.BLACK)
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

        //実態がある場合
        if(!noBody)
        {
            switch(this.colorType)
            {
                case OBJECT_COLOR.BLACK:
                    GetComponent<SpriteRenderer>().color = new Color(0,0,0);
                    //Debug.Log("黒wwww");
                    break;

                case OBJECT_COLOR.WHITE:
                    GetComponent<SpriteRenderer>().color = new Color(1,1,1);
                    break;

                case OBJECT_COLOR.RED:
                    GetComponent<SpriteRenderer>().color = new Color(1,0,0);
                    //Debug.Log("黒wwww");
                    break;

                case OBJECT_COLOR.GREEN:
                    GetComponent<SpriteRenderer>().color = new Color(59f/255f,175f/255f,117f/255f);
                    break;
                
                case OBJECT_COLOR.BLUE:
                    GetComponent<SpriteRenderer>().color = new Color(0,0,1);
                    //Debug.Log("黒wwww");
                    break;

                case OBJECT_COLOR.CYAN:
                    GetComponent<SpriteRenderer>().color = new Color(0,156f/255f,209f/255f);
                    break;
                
                case OBJECT_COLOR.MAGENTA:
                    GetComponent<SpriteRenderer>().color = new Color(228f/255f,0,127f/255f);
                    //Debug.Log("黒wwww");
                    break;

                case OBJECT_COLOR.YELLOW:
                    GetComponent<SpriteRenderer>().color = new Color(1,1,0);
                    break;

                case OBJECT_COLOR.PURPLE:
                    GetComponent<SpriteRenderer>().color = new Color(167f/255f,87f/255f,168f/255f);

                    //Debug.Log("黒wwww");
                    break;
                case OBJECT_COLOR.ORRANGE:
                    GetComponent<SpriteRenderer>().color = new Color(1,165f/255f,0);
                    break;

                case OBJECT_COLOR.LIME:
                    GetComponent<SpriteRenderer>().color = new Color(0,1,0);
                    //Debug.Log("黒wwww");
                    break;
                case OBJECT_COLOR.GRAY:
                    GetComponent<SpriteRenderer>().color = new Color(118f/255,118f/255f,118f/255f);
                    break;

                default:break;
            }
        }
        
    }

    /*
    void FixedUpdate()
    {
        if(stopFlag)
        {
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
        }
        else 
        {

            GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }
    */

    //Bodyの処理
    void ControlBody(bool flag)
    {
        BodyObject.SetActive(!flag);
        //if(GetComponent<BoxCollider2D>())GetComponent<BoxCollider2D>().isTrigger = flag;
        //if(GetComponent<EdgeCollider2D>())GetComponent<EdgeCollider2D>().isTrigger = flag;
        //if(GetComponent<PolygonCollider2D>())GetComponent<PolygonCollider2D>().isTrigger = flag;
        //もしTrue(実体なし)だったら
        Vector3 pos = transform.position;
        if(flag || touchingObject)
        {
            //GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            //GetComponent<Rigidbody2D>().gravityScale = 0;
            //transform.position = lastPos;
            //transform.rotation = Quaternion.Euler(0,0,lastRotZ);
            //GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
           
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
        /*
        if(touchingObject)
        {
            stopFlag = true;
        }
        else
        {
            GetComponent<Rigidbody2D>().gravityScale = gravity;
            stopFlag = false;
        }
        */
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

    
    /*
    void OnTriggerStay2D(Collider2D other)
    {
        //もし当たってるオブジェクトにColorObjectScriptがアタッチされてたら
        if(other.gameObject.GetComponent<ColorObjectScript>())
        {
            //当たってるのが普通にオブジェクトだったら
            if(other.gameObject.GetComponent<ColorObjectScript>().isObject)
            {
                //動かなくする。
                if(noBody)
                {
                    //GetComponent<Rigidbody2D>().gravityScale = 0;
                    //GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
                    stopFlag = true;
                    
                }
                //もしくは
                //印をつけておく(実体化できなくする)
                touchingObject = true;

            }
            //動かなくする。

            if(noBody)
            {
                //GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            }
        }

        //もし当たってるオブジェクトがStageだったら
        if(other.gameObject.tag == "Stage")
        {
            if(noBody)
            {
                stopFlag = true;
                //動かなくする。
                //GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
            }
                        
        }
    }

    //
    void OnTriggerExit2D(Collider2D other)
    {
        //もし当たってるオブジェクトにColorObjectScriptがアタッチされてたら
        if(other.gameObject.GetComponent<ColorObjectScript>())
        {
            if(other.gameObject.GetComponent<ColorObjectScript>().isObject)
            {
                //
                touchingObject = false;
                GetComponent<Rigidbody2D>().gravityScale = gravity;
                stopFlag = false;
            }
            
        }
    }
    */
    

    //引数の値を最低値と最高値に修正するメソッド
    float bound(float min,float max,float value)
    {
        if(value < min)return min;
        if(max < value)return max;
        return value;
    }
}
