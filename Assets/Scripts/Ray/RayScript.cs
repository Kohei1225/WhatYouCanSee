using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//影になってる部分を判定するスクリプト
public class RayScript : MonoBehaviour
{
    float yPos;
    float xPos;
    public float rayLength;
    public float firstTheta;
    bool hasTouchedBody = false;//既に実体があるオブジェクトに当たったとき用。(もしかして影の判定できるのでは？)
    bool hasTouchedEdge = false;
    public float rootObjectTheta;//親の大元のオブジェクトの角度
    public float parentLightTheta;//光が出る角度の範囲
    GameObject TouchingObject;
    int layerMask;


    //public GameObject gameManager;
    GameManagerScript gameManagerScript;
    Light2D parent2DLight;

    //Rayの表示時間
    private const float RAY_DISPLAY_TIME = 0.1f;

    //int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Managers").transform.Find("GameManager").gameObject.GetComponent<GameManagerScript>();
        rayLength = 30 * transform.parent.gameObject.transform.localScale.x * 5;
        rootObjectTheta = transform.root.gameObject.transform.rotation.z;

        parent2DLight = transform.parent.gameObject.GetComponent<Light2D>();
        parentLightTheta = parent2DLight.pointLightInnerAngle;
        parentLightTheta = parent2DLight.pointLightInnerAngle;
        parent2DLight.shadowIntensity = System.Convert.ToInt32(gameManagerScript.existShadow);

        layerMask = (1 << (LayerMask.NameToLayer("Ray"))/* | 1 << (LayerMask.NameToLayer("BACKGROUND"))*/);
        
        
        //Debug.Log(transform.root.gameObject.name);
        //Debug.Log(hasTouchedBody + ":" + System.Convert.ToInt32(hasTouchedBody));
    }

    // Update is called once per frame
    void Update()
    {
        //360°回転させて光線を飛ばす
        //for(int j = 0;j < 2;j++)
        //{
        rootObjectTheta = transform.root.gameObject.transform.localEulerAngles.z;
        parentLightTheta = transform.parent.gameObject.GetComponent<Light2D>().pointLightInnerAngle;
        rayLength = transform.parent.gameObject.GetComponent<Light2D>().pointLightInnerRadius;

            for(float i = 0;i < 10;i+=0.0625f)
            {
                float angle = i + firstTheta - rootObjectTheta - parentLightTheta/2f;
                if(this.firstTheta + i > parentLightTheta)break;
                //角度を更新する(前もって決められた自身の角度と一番親のオブジェクトの角度を考慮)
                xPos = Mathf.Sin((angle)/180f*Mathf.PI);// + transform.position.x;
                yPos = Mathf.Cos((angle)/180f*Mathf.PI);// - transform.position.y;]

                Vector2 vec = new Vector2(xPos,yPos);
                vec *= rayLength;

            
                //RaycastHit2D hit = Physics2D.Raycast(transform.position,vec/*,hit/*,rayLength*/);
                /*
                
                */

                //Debug.DrawRay(transform.position, vec, Color.red, rayLength);

                //RaycastHit2D hit = RaycastAndDraw(transform.position,vec,rayLength);

                int ObjectNum = 0;
                /*
                foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, vec, rayLength))
                {
                    if(hit.collider)
                    {
                        if(hit.collider.gameObject.GetComponent<ColorObjectScript>())
                        {
                            //実態があるオブジェクトに当たる度にカウントしていく。
                            if(!hit.collider.gameObject.GetComponent<ColorObjectScript>().noBody)ObjectNum++;
                        }
                    }  
                }
                */
                
                int ObjectCounter = 0;
                hasTouchedBody = false;
                hasTouchedEdge = false;
                int objectNum = -11;
                //Debug.Log("光です");
                foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, vec, rayLength, layerMask))
                {
                    //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                    //何かしらに当たった時
                    if(hit.collider)
                    {
                        //Debug.Log("当たってる");
                        //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                        //オブジェクトに当たった時
                        if(hit.collider.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>())
                        {
                            ColorObjectScript colorObject = hit.collider.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>();
                            //もしも影が必要なかったら
                            /*
                            if(!gameManagerScript.existShadow)
                            {
                                colorObject.touchingShadow = false;
                                colorObject.outOfShadow = true;
                            }
                            
                            else */
                            if(colorObject.isObject)
                            {
                                //当たったオブジェクトに実体がある時
                                if(!colorObject.noBody)
                                {
                                    //ObjectNum++;
                                    //オブジェクトの後ろのオブジェクトには光が当たるはずがないので
                                    //既に実体があるオブジェクトに当たってる場合はfalseにする。
                                    if(hasTouchedBody)
                                    {
                                        //colorObject.onRay = false;
                                        //Debug.Log("番号：" + objectNum);
                                    }
                                    //初めて当たるオブジェクトの時
                                    else
                                    {
                                        //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.red, RAY_DISPLAY_TIME, false);
                                        //colorObject.onRay = true;
                                        //if(colorObject.colorType == ColorObjectScript.OBJECT_COLOR.BLACK)Debug.Log("黒だけど当たった");
                                        objectNum = colorObject.objectNumber;
                                        //実体に当たっているので印をつけておく(次に当たった時に判定する用)
                                        hasTouchedBody = true;

                                        //↓このif文を有効にするとPlayerは無視される
                                        //if(objectNum == -1)hasTouchedBody = false;
                                        //自身が影をもってないオブジェクトは無視する。
                                        if(!colorObject.hasShadow)hasTouchedBody = false;

                                        //Debug.Log(objectNum + "に最初に当たった！！");
                                    }
                                    
                                }
                                //当たったオブジェクトに実体がない時
                                else 
                                {
                                    //影の部分に入ってる場合
                                    if(hasTouchedBody)
                                    {
                                        //colorObject.onRay = false;
                                    }
                                    //影の部分から出てる場合
                                    else 
                                    {
                                        //colorObject.onRay = true;
                                    }
                                    //オブジェクトの色が黒以外で同色背景の中に入ってない場合。
                                    /*
                                    if(colorObject.colorType != ColorObjectScript.OBJECT_COLOR.BLACK && !colorObject.inSameColor)
                                    {
                                        //colorObject.noBody = false;
                                        colorObject.onRay = true;
                                        Debug.Log("オブジェクト" + colorObject.colorType);
                                    }     
                                    */
                                    //if(ColorObject,colorType == ColorObjectScript.OBJECT_COLOR.BLACK) 
                                }
                            }
                            
                        }
                        else if(hit.collider.gameObject.tag == "ColorObject" || hit.collider.gameObject.tag == "Stage")
                        {
                            hasTouchedBody = true;
                            //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.red, RAY_DISPLAY_TIME, false);
                        }

                        
                        //角のオブジェクトに当たった時
                        if(hit.collider.gameObject.GetComponent<EdgeScript>())
                        {
                            EdgeScript edgeObject = hit.collider.gameObject.GetComponent<EdgeScript>();
                            //Debug.Log(hit.collider.gameObject.name + "です");
                            //当たった角のタイプがSHADOWだった時しか処理しない
                            if(edgeObject.edgeType == EdgeScript.EDGE_TYPE.SHADOW)
                            {
                                //if(hasTouchedBody)edgeObject.onRay = false;
                                //初めて当たる場合
                                //Debug.Log("Num:" + objectNum + " hasTouchedBody is " + hasTouchedBody);

                                //影が存在しなければオブジェクトにあたり次第Trueにする
                                if(!gameManagerScript.existShadow)
                                {
                                    edgeObject.onRay = true;
                                }

                                //この角が初めて当たるオブジェクトの場合
                                else if(!hasTouchedBody)//else 
                                {
                                    //hit.collider.gameObject.transfrom.parent.gameObject.GetComponent<ColorObjectScript>().objectNumber
                                    //ちゃんと光が当たってるのでTrueにする
                                    edgeObject.onRay = true;
                                    //if(objectNum == 18)Debug.Log("18は影に入っってない");

                                    //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                                    //Debug.Log("この角(" + hit.collider.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>().objectNumber + ")は初めて当たるやつだ！！");
                                }
                                //既にオブジェクトに当たってる場合(影の部分のお話)
                                else 
                                {
                                    //初めて光に当たったオブジェクトが角の親オブジェクトだったら気にせずTrueにする
                                    if(hit.collider.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>().objectNumber == objectNum)
                                    {
                                        edgeObject.onRay = true;
                                        //if(objectNum == 18)Debug.Log("光には当たってるよ");
                                    }
                                        
                                    //当たったオブジェクトが他のオブジェクト(ガチの影)だったらFalse(当たってない判定)にする
                                    else 
                                    {
                                        //edgeObject.onRay = false;   
                                    } 
                                }
                                
                                hasTouchedEdge = true;
                                //hasTouchedBody  = true;
                                //Debug.Log("あれ？");
                            }
                            
                        }

                        if(hit.collider.gameObject.tag == "Stage")
                        {
                            hasTouchedBody = true;
                        }
                        
                        ObjectCounter++;

                        /*
                        //Debug.Log(hit.collider.gameObject.name);
                        Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                        //Debug.Log(hit.collider.gameObject.name);
                        string hitName = hit.collider.gameObject.name;
                        if(hitName == "Square")
                        {
                            //Debug.Log("資格に当たった！！");
                        }
                        else if(hitName == "Shikaku")
                        {
                            Debug.Log("びよおおおおおおおおおんwwww");
                        }


                        if(hit.collider.gameObject.GetComponent<ColorObjectScript>())
                        {
                            
                            if(hitName == "Square (10)")Debug.Log(hitName + "'s noBody is " + hit.collider.gameObject.GetComponent<ColorObjectScript>().noBody);
                            //当たったオブジェクトに実態がある場合はもう処理しない
                            if(!hit.collider.gameObject.GetComponent<ColorObjectScript>().noBody)break;
                            //|| hit.collider.gameObject.GetComponent<ColorObjectScript>().noBody)break;
                            
                            
                            hit.collider.gameObject.GetComponent<ColorObjectScript>().onRay = true;
                            Debug.Log(hitName + "のonRay:" + hit.collider.gameObject.GetComponent<ColorObjectScript>().onRay);//hit.collider.gameObject.name + "は日陰の外にいるぞ！！");
                        }
                        if()
                        */
                    }
                    //else Debug.DrawRay(transform.position, vec * rayLength, Color.green, RAY_DISPLAY_TIME, false);
                }

                
            }
        //}
        

        //i++;

        //if(i == 360)i = 0;
    }

    public static RaycastHit2D RaycastAndDraw(Vector2 origin, Vector2 direction, float maxDistance)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance);

        //衝突時のRayを画面に表示
        if (hit.collider)
        {
            Debug.DrawRay(origin, hit.point - origin, Color.blue, RAY_DISPLAY_TIME, false);
        }
        //非衝突時のRayを画面に表示
        else
        {
            Debug.DrawRay(origin, direction * maxDistance, Color.green, RAY_DISPLAY_TIME, false);
        }

        return hit;
    }
}
