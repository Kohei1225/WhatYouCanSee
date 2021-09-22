using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRayOfObject : MonoBehaviour
{
    public GameObject[] edgeList;
    int edgeNum;
    int colorSum = 0;
    public bool[] hitColorList;
    public float distance;
    public int maxLayer;
    public string colorName;
    ColorObject2 colorObject;
    public int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        edgeNum = edgeList.Length;
        hitColorList = new bool[edgeNum];
        colorObject = GetComponent<ColorObject2>();
        layerMask = (1 << (LayerMask.NameToLayer("BACKGROUND")));
        //Debug.Log(layerMask);
        //layerMask = 20;
    }

    // Update is called once per frame
    void Update()
    {
        colorSum = 0;
        maxLayer = -20;
        for(int i = 0;i < edgeNum;i++)
        {
            hitColorList[i] = false;
            int nextObject = (i+1)%edgeNum;

            //int distance = Mathf.Abs(edgeList.transform.position.x);
            //Debug.DrawRay((Vector2)edgeList[i].transform.position,edgeList[nextObject].transform.position - edgeList[i].transform.position, Color.blue, 0.01f, false);
            //Debug.Log("Ray:" + i);
            distance = Mathf.Sqrt(Mathf.Pow(edgeList[i].transform.position.x - edgeList[nextObject].transform.position.x,2) + Mathf.Pow(edgeList[i].transform.position.y - edgeList[nextObject].transform.position.y,2));

            
            //一番レイヤーの優先度が高い値を探す
            foreach(RaycastHit2D hit in Physics2D.RaycastAll(edgeList[i].transform.position,edgeList[nextObject].transform.position - edgeList[i].transform.position,distance,layerMask))
            {
                if(hit.collider)
                {
                    int nowOrderInLayer = hit.collider.gameObject.transform.root.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
                    if(nowOrderInLayer > maxLayer)maxLayer = nowOrderInLayer;
                }
            }

            //同色背景に入ってるか調べる
            foreach(RaycastHit2D hit in Physics2D.RaycastAll(edgeList[i].transform.position,edgeList[nextObject].transform.position - edgeList[i].transform.position,distance,layerMask))
            {
                //Rayが何かに当たる
                if(hit.collider)
                {
                    //レイヤーの優先度が一番高いやつだったら
                    if(hit.collider.gameObject.transform.root.gameObject.GetComponent<SpriteRenderer>().sortingOrder == maxLayer)
                    {
                        //同色の背景とRayがぶつかった(触れている)
                        if(hit.collider.gameObject.transform.root.gameObject.GetComponent<ColorObject2>().colorType == colorObject.colorType)
                        {
                            //角から当たった場所までの距離を測って、少しでも距離があったらダメ
                            float hitDistance = Mathf.Sqrt(Mathf.Pow(edgeList[i].transform.position.x - hit.point.x,2) + Mathf.Pow(edgeList[i].transform.position.y - hit.point.y,2));
                            if(hitDistance > 0)
                            {
                                //if(!hitColorList[i])break;

                                hitColorList[i] = false;
                                break;
                            }

                            hitColorList[i] = true;
                            //Debug.DrawRay((Vector2)edgeList[i].transform.position,hit.point - (Vector2)edgeList[i].transform.position, Color.blue, 0.01f, false);
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
        if(colorSum == edgeNum)
        {
            colorObject.inSameColor = true;
            //Debug.Log("inSameColor!!!!(" + colorName + ")");
        }
        else colorObject.inSameColor = false;
    }
}
