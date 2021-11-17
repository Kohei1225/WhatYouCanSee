using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

//影になってる部分を判定するスクリプト
public class LightRayScriptVer2 : MonoBehaviour
{
    float rayLength;            //Rayの最長距離
    bool hasTouchedBody = false;//既に実体があるオブジェクトに当たったとき用。(もしかして影の判定できるのでは？)
    float rootObjectTheta;      //親の大元のオブジェクトの角度
    float LightTheta;           //光が出る角度の範囲
    int layerMask;              //Rayを飛ばすLayer
    GameObject vecPointObject;  //角度の基準になる方向になるオブジェクト

    GameManagerScript gameManagerScript;   
    Light2D light2D;
    private const float RAY_DISPLAY_TIME = 0.01f;//Rayの表示時間(Sceneでしか見えないヤツ)

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Managers").GetComponent<GameManagerScript>();
        vecPointObject = transform.Find("VectorPoint").gameObject;
        rayLength = 30 * transform.parent.gameObject.transform.localScale.x * 5;
        rootObjectTheta = transform.parent.gameObject.transform.rotation.z;

        light2D = GetComponent<Light2D>();
        LightTheta = light2D.pointLightInnerAngle;
        LightTheta = light2D.pointLightInnerAngle;
        light2D.shadowIntensity = System.Convert.ToInt32(gameManagerScript.existShadow);
        
        layerMask = (1 << (LayerMask.NameToLayer("Ray")));
    }

    // Update is called once per frame
    void Update()
    {
        rootObjectTheta = transform.parent.gameObject.transform.localEulerAngles.z;
        LightTheta = GetComponent<Light2D>().pointLightInnerAngle;
        rayLength = GetComponent<Light2D>().pointLightInnerRadius;

        //オブジェクトごと
        for(int i = 0;i < gameManagerScript.Get_colorObjectList().Length;i++)
        {
            if(gameManagerScript.Get_colorObjectList()[i] && gameManagerScript.existRay)
            {  
                RayJudgeScript colliderListScript = gameManagerScript.Get_colorObjectList()[i].GetComponent<RayJudgeScript>();
                float xPos;
                float yPos;

                //オブジェクトの中に定義されてるそれぞれのコライダーに向かって飛ばす
                for(int j = 0;j < colliderListScript.colliderList.Length;j++)
                {
                    GameObject vartex = colliderListScript.colliderList[j];
                    if(!vartex)break;
                    
                    //角度の基準になる単位ベクトルを用意
                    xPos = vecPointObject.transform.position.x - transform.position.x;
                    yPos = vecPointObject.transform.position.y - transform.position.y;
                    Vector2 baseUnitVec = new Vector2(xPos,yPos);
                    baseUnitVec = UnitVector(baseUnitVec);
                    //Debug.DrawRay(transform.position, new Vector2(xPos,yPos), Color.red, RAY_DISPLAY_TIME, false);
                    
                    //対象の方向を向いてる単位ベクトルを用意
                    xPos = vartex.transform.position.x - transform.position.x;
                    yPos = vartex.transform.position.y - transform.position.y;
                    Vector2 vec = new Vector2(xPos,yPos);
                    Vector2 objUnitVec = UnitVector(vec);
                    
                    //光の範囲内であればRayを飛ばす処理をする。(この１行で２つの単位ベクトルの角度を計算してる)
                    if(Mathf.Acos((baseUnitVec.x*objUnitVec.x + baseUnitVec.y*objUnitVec.y))*180f/Mathf.PI < LightTheta/2)
                    {

                        hasTouchedBody = false;//新しいRayを飛ばす度にリセット
                        Vector2 firstHitPos = (Vector2)transform.position;

                        foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position,vec,rayLength,layerMask))
                        {
                            GameObject hitObject = hit.collider.gameObject;

                            //同じ親をもつオブジェクトに当たった時
                            if(hitObject.transform.parent.gameObject == gameManagerScript.Get_colorObjectList()[i])
                            {
                                //小さいコライダーに当たったら
                                if(hitObject.GetComponent<RayColliderScript>())
                                {
                                    //とりあえずRayに当たった判定はする
                                    RayColliderScript hitCollider = hitObject.GetComponent<RayColliderScript>();
                                    hitCollider.Set_onRay();

                                    //当たったコライダーのスクリプトの値を変更する。
                                    //着地時に地面にめり込んでRayが当たらなくなることがあったので、そこを改善するためにif文をつけました。
                                    //オブジェクトの速度を元に判定する距離を計算して、RayがStageに当たった部分とColliderの距離よりも長かったら
                                    //Colliderは前の状態を維持するみたいな感じです。
                                    float distanceIgnoreRay =  0.1f;
                                    if(gameManagerScript.existShadow && Mathf.Abs(vartex.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity.y) > 0)
                                        distanceIgnoreRay = Mathf.Abs(vartex.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity.y) * 0.013f;

                                    //実際の距離と速度を元に計算した距離を比較する
                                    if(CalcDistance2Point(hit.point,firstHitPos) > distanceIgnoreRay)
                                        hitCollider.SetLightVariable(!hasTouchedBody);

                                    

                                    //Rayが目的に到着したらもう次に行く
                                    if(hitObject == vartex)
                                    {
                                        //if(hitObject.tag == "Player")
                                        //if(hasTouchedBody)Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                                        //else Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.green, RAY_DISPLAY_TIME, false);
                                        break;
                                    }
                                }
                            }
                            //違う親を持つオブジェクトに当たったかつ影が存在する場合
                            else if(gameManagerScript.existShadow)
                            {
                                //取得できたコンポーネント
                                if(hitObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>())
                                {
                                    //当たった他のオブジェクトに実体と影があれば印をつける。
                                    if(hitObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>().hasShadow && !hitObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>().Get_noBody())
                                    {
                                        //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.blue, RAY_DISPLAY_TIME, false);
                                        hasTouchedBody = true;
                                    }
                                }
                                if(hitObject.tag == "Stage" )
                                {
                                    firstHitPos = hit.point;
                                    //if(vartex.tag == "Player")Debug.Log("hitPoint:"+firstHitPos);
                                    //Debug.Log("ステージ:" + hitObject.name);
                                    //Debug.DrawRay(transform.position, hit.point - new Vector2(transform.position.x,transform.position.y), Color.red, RAY_DISPLAY_TIME, false);
                                    hasTouchedBody = true;
                                }
                            }
                        }
                    }
                }
            }
        }
            
        hasTouchedBody = false;
    }

    //単位ベクトルを返す返すメソッド
    Vector2 UnitVector(Vector2 vec)
    {
        float size = Mathf.Sqrt(vec.x*vec.x + vec.y*vec.y);
        return vec/size;
    }

    float CalcDistance2Point(Vector2 pos1,Vector2 pos2)
    {
        
        return Mathf.Sqrt((pos1.x - pos2.x)*(pos1.x-pos2.x) + (pos1.y-pos2.y)*(pos1.y-pos2.y));
    }
}