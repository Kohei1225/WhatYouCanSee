using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクトの外枠に沿ってRayを飛ばして当たった判定をするクラス
public class FrameRayOfObject : MonoBehaviour
{
    public GameObject[] vartexList; //オブジェクトの頂点に配置したオブジェクトのリスト
    int vartexNum;                  //vartexListの要素数を記録するためだけの変数
    int colorSum = 0;               //同色背景に完全に重なってるRayの数
    bool[] hitColorList;     //同色背景に完全に重なってるかの結果を記録する配列
    float distance;                 //Rayを飛ばす距離
    int maxLayer;                   //一番見た目の優先度が高かったLayerの値
    ColorObjectVer3 colorObject;
    public int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        vartexNum = vartexList.Length;
        hitColorList = new bool[vartexNum];
        colorObject = GetComponent<ColorObjectVer3>();
        layerMask = (1 << (LayerMask.NameToLayer("BACKGROUND")));
    }

    // Update is called once per frame
    void Update()
    {
        colorSum = 0;
        maxLayer = -20;

        //頂点の数だけRayを飛ばす
        for(int i = 0;i < vartexNum;i++)
        {
            hitColorList[i] = false;
            int nextObject = (i+1)%vartexNum;

            //Debug.DrawRay((Vector2)vartexList[i].transform.position,vartexList[nextObject].transform.position - vartexList[i].transform.position, Color.blue, 0.01f, false);
            //distance = Mathf.Sqrt(Mathf.Pow(vartexList[i].transform.position.x - vartexList[nextObject].transform.position.x,2) + Mathf.Pow(vartexList[i].transform.position.y - vartexList[nextObject].transform.position.y,2));

            distance = CalcObjectDistance(vartexList[i],vartexList[nextObject]);//距離を測っておく

            //一番レイヤーの優先度が高い値を探す
            foreach(RaycastHit2D hit in Physics2D.RaycastAll(vartexList[i].transform.position,vartexList[nextObject].transform.position - vartexList[i].transform.position,distance,layerMask))
            {
                if(hit.collider)
                {
                    int nowOrderInLayer = hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
                    if(nowOrderInLayer > maxLayer)maxLayer = nowOrderInLayer;
                }
            }

            //同色背景に入ってるか調べる
            foreach(RaycastHit2D hit in Physics2D.RaycastAll(vartexList[i].transform.position,vartexList[nextObject].transform.position - vartexList[i].transform.position,distance,layerMask))
            {
                //Rayが何かに当たる
                if(hit.collider)
                {
                    //Debug.DrawRay((Vector2)vartexList[i].transform.position,hit.point - (Vector2)vartexList[i].transform.position, Color.blue, 0.01f, false);
                    GameObject hitObject = hit.collider.gameObject;

                    //レイヤーの優先度が一番高いやつだったら
                    if(hitObject.GetComponent<SpriteRenderer>().sortingOrder == maxLayer)
                    {
                        //同色の背景とRayがぶつかった(触れている)
                        if(hitObject.GetComponent<ColorObjectVer3>()) if(hitObject.GetComponent<ColorObjectVer3>().colorType == colorObject.colorType)
                        {
                            //角から当たった場所までの距離を測って、少しでも距離があったらダメ
                            float hitDistance = Mathf.Sqrt(Mathf.Pow(vartexList[i].transform.position.x - hit.point.x,2) + Mathf.Pow(vartexList[i].transform.position.y - hit.point.y,2));
                            if(hitDistance > 0)
                            {
                                //if(!hitColorList[i])break;

                                hitColorList[i] = false;
                                break;
                            }

                            hitColorList[i] = true;
                            //Debug.DrawRay((Vector2)vartexList[i].transform.position,hit.point - (Vector2)vartexList[i].transform.position, Color.blue, 0.01f, false);
                        }
                        //違う色の背景とRayがぶつかった
                        else
                        {
                            hitColorList[i] = false;
                            //Debug.Log("Ray"+ i + ":" + hit.collider.name);
                            break;
                        };
                        //Debug.Log(hit.collider.gameObject.name);
                    }
                }
                //else Debug.Log("Ray"+i+":Nothing");
            }
            if(hitColorList[i])colorSum++;
        }
        if(colorSum == vartexNum)
        {
            colorObject.Set_inSameColor(true);
        }
        else colorObject.Set_inSameColor(false);
    }

    //2D上の２点の距離を返すメソッド
    float CalcObjectDistance(GameObject obj1,GameObject obj2)
    {
        Vector3 vec1 = obj1.transform.position;
        Vector3 vec2 = obj2.transform.position;
        return Mathf.Sqrt((vec1.x-vec2.x)*(vec1.x-vec2.x) + (vec1.y-vec2.y)*(vec2.y-vec2.y));
    }
}
