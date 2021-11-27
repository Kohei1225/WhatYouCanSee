using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//色を持ってるオブジェクトにアタッチするクラス。消え得るオブジェクトを覆っていたら印を付けてあげる。
public class ColorObjectVer3 : MonoBehaviour
{
    //色情報のenum
    public enum OBJECT_COLOR3
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
    public OBJECT_COLOR3 colorType; //色のタイプ
    public bool isObject;           //ステージ上にあるかどうか(背景ならfalse)
    public bool hasShadow;          //このオブジェクトに影ができるか
    public bool canHold;            //プレイヤーが持てるかの判定

    bool noBody;        //実体が消えるかの判定値
    bool active;        //この変数がTrueじゃないと実体は消えない。Playerのみ切り替え可能
    bool onLightRay;    //光に当たってる判定
    bool onShadowRay;   //影に当たってる判定
    bool inSameColor;   //同じ色に入っているかの判定(FrameRayOfObjectスクリプトから操作)
    bool onThePlayer;   //プレイヤーに持ち上げられてる判定
    float gravity;      //重力加速度を記憶しておく用

    GameManagerScript gameManagerScript;
    GameObject BodyObject;  //SetActiveで切り替える対象(実体に当たる部分)


    // Start is called before the first frame update
    void Start()
    {
        noBody = false;
        active = true;
        if(gameObject.tag == "BackGround")gameObject.layer = LayerMask.NameToLayer("BACKGROUND");
        else if(gameObject.tag == "ColorObject")gameObject.layer = LayerMask.NameToLayer("Color");
        gameManagerScript = GameObject.Find("Managers").GetComponent<GameManagerScript>();
        if(isObject)
        {
            if(!transform.Find("Body"))Debug.Log(gameObject.name + "にはBodyなし");
            else BodyObject = transform.Find("Body").gameObject;
            gravity = GetComponent<Rigidbody2D>().gravityScale;

            //if(!gameManagerScript.existShadow)hasShadow = false;
            if(BodyObject.transform.Find("ShadowCaster"))
                BodyObject.transform.Find("ShadowCaster").gameObject.SetActive(this.hasShadow);
        }

        //ゴールの場合
        if(gameObject.name == "Goal")
        {
            gravity = 0;
            this.hasShadow = false;
            this.canHold = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    void FixedUpdate()
    {
        //if(gameObject.tag != "BackGround" && gameObject.tag != "Player")gameObject.layer = LayerMask.NameToLayer("Color");

        //背景じゃなくてステージ上にあるオブジェクトの場合の処理
        if(isObject && active)
        {   
            //オブジェクトが黒色なら
            if(this.colorType == OBJECT_COLOR3.BLACK)
            {
                //黒い背景に重なるか、影の中にいれば当たり判定が消える
                if(inSameColor || !onLightRay)
                    noBody = true;
                else noBody = false;
            }
            //黒以外の色なら
            else
            {
                //同色背景に重なってて、影が当たってなければ当たり判定が消える。
                if(inSameColor && !onShadowRay)
                    noBody = true;
                else noBody = false;
            }
        }
        else noBody = false;

        GetComponent<SpriteRenderer>().color = ChangeColorByType(colorType);//colorTypeによって変色

        if(isObject)ControlBody(noBody);//オブジェクトの状態によって処理する
    }

    //Bodyの処理
    void ControlBody(bool flag)
    {
        BodyObject.SetActive(!flag);

        //もしTrue(実体なし)だったら
        Vector3 pos = transform.position;
        if(flag)
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

    //オブジェクトの色を変えるメソッド
    public Color ChangeColorByType(OBJECT_COLOR3 colorType)
    {
        Color color = new Color(0,0,0);
        switch(this.colorType)
        {
            case OBJECT_COLOR3.BLACK:
                color = new Color(0,0,0);
                break;

            case OBJECT_COLOR3.WHITE:
                color = new Color(1,1,1);
                break;

            case OBJECT_COLOR3.RED:
                color = new Color(1,0,0);
                break;

            case OBJECT_COLOR3.GREEN:
                color = new Color(59f/255f,175f/255f,117f/255f);
                break;
            
            case OBJECT_COLOR3.BLUE:
                color = new Color(0,0,1);
                break;

            case OBJECT_COLOR3.CYAN:
                color = new Color(0,156f/255f,209f/255f);
                break;
            
            case OBJECT_COLOR3.MAGENTA:
                color = new Color(225f/255f,0,255f/255f);
                break;

            case OBJECT_COLOR3.YELLOW:
                color = new Color(1,1,0);
                break;

            case OBJECT_COLOR3.PURPLE:
                color = new Color(167f/255f,87f/255f,168f/255f);

                break;
            case OBJECT_COLOR3.ORRANGE:
                color = new Color(1,165f/255f,0);
                break;

            case OBJECT_COLOR3.LIME:
                color = new Color(0,1,0);
                break;
            case OBJECT_COLOR3.GRAY:
                color =  new Color(118f/255,118f/255f,118f/255f);
                break;

            default:break;
        }
        return color;
    }

    public bool Get_noBody()
    {
        return this.noBody;
    }

    public void Set_active(bool value)
    {
        this.active = value;
    }

    public bool Get_active()
    {
        return this.active;
    }

    public void Set_onLightRay(bool value)
    {
        this.onLightRay = value;
    }

    public void Set_onShadowRay(bool value)
    {
        this.onShadowRay = value;
    }

    public void Set_inSameColor(bool value)
    {
        this.inSameColor = value;
    }

    public void Set_onThePlayer(bool value)
    {
        this.onThePlayer = value;
    }
}