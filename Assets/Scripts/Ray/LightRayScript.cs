using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//影になってる部分を判定するスクリプト
public class LightRayScript : MonoBehaviour
{
    float yPos;
    float xPos;
    public float rayLength;
    public float firstTheta;    //こいつ自体が持ってる角度。この角度にプラスしていく
    bool hasTouchedBody = false;//既に実体があるオブジェクトに当たったとき用。(もしかして影の判定できるのでは？)
    public float rootObjectTheta;//親の大元のオブジェクトの角度
    public float parentLightTheta;//光が出る角度の範囲
    int layerMask;
    public bool forDebug;

    GameManagerScript gameManagerScript;
    Light2D parent2DLight;

    //Rayの表示時間
    private const float RAY_DISPLAY_TIME = 0.01f;

    //int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Managers").GetComponent<GameManagerScript>();
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
        rootObjectTheta = transform.root.gameObject.transform.localEulerAngles.z;
        parentLightTheta = transform.parent.gameObject.GetComponent<Light2D>().pointLightInnerAngle;
        rayLength = transform.parent.gameObject.GetComponent<Light2D>().pointLightInnerRadius;

        bool finishFlag = false;

        if(firstTheta > parentLightTheta)finishFlag = true;;
            for(float i = 0;i < 10;i+=0.03125f)
            {
                if(finishFlag)break;
                hasTouchedBody = false;

                float angle = i + firstTheta - rootObjectTheta - parentLightTheta/2f;
                if(this.firstTheta + i > parentLightTheta)
                {
                    finishFlag = true;
                    hasTouchedBody = true;
                }

                //角度を更新する(前もって決められた自身の角度と一番親のオブジェクトの角度を考慮)
                xPos = Mathf.Sin((angle)/180f*Mathf.PI);// + transform.position.x;
                yPos = Mathf.Cos((angle)/180f*Mathf.PI);// - transform.position.y;]

                Vector2 vec = new Vector2(xPos,yPos);
                vec *= rayLength;

                int ObjectCounter = 0;
                
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

                        //色を持ってるオブジェクトに当たった時
                        if(hit.collider.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>())
                        {
                            ColorObject2 colorObject = hit.collider.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>();
                            //if(forDebug)Debug.Log("hit:" + hit.collider.gameObject.transform.parent.gameObject.name);

                            //ステージにあるオブジェクトに当たった時
                            if(colorObject.isObject)
                            {
                                colorObject.onRay = true;
                                //既に何かしらのオブジェクトに当たってる時
                                if(hasTouchedBody)
                                {
                                    //if(hit.collider.gameObject.transform.parent.gameObject.name == "SquareBlockVer2 (3)")Debug.Log("影判定されてる");
                                    if(colorObject.hasShadow)colorObject.onShadowRay = true;
                                    else colorObject.onLightRay = true;
                                }
                                //初めてオブジェクトに当たる時
                                else
                                {
                                    //
                                    colorObject.onLightRay = true;
                                    if(!colorObject.noBody)
                                    {
                                        if(colorObject.hasShadow)break;
                                        //if(forDebug)Debug.Log(hit.collider.gameObject.transform.parent.gameObject.name);
                                        this.hasTouchedBody = true;
                                        Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                                    }
                                    //↓このif文を有効にするとPlayerは無視される
                                    //if(objectNum == -1)hasTouchedBody = false;
                                    //自身が影をもってないオブジェクトは無視する。
                                    //if(!colorObject.hasShadow)hasTouchedBody = false;
                                }
                            }
                            
                        }
                        else if(hit.collider.gameObject.tag == "ColorObject" || hit.collider.gameObject.tag == "Stage")
                        {
                            hasTouchedBody = true;
                            //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.red, RAY_DISPLAY_TIME, false);
                        }

                        if(hit.collider.gameObject.tag == "Stage")
                        {
                            hasTouchedBody = true;
                        }
                    }
                    //else Debug.DrawRay(transform.position, vec * rayLength, Color.green, RAY_DISPLAY_TIME, false);
                }
            }
        //Debug.Log("1cool:" + gameObject.name);
    }
}
