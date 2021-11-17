using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//オブジェクト同士の重なりを判定するためだけのクラス
public class TouchingObjectScript : MonoBehaviour
{
    EdgeCheckerScript parentChecker;
    public GameObject nextEdge;
    public int EdgeNum;
    int objectNum;

    // Start is called before the first frame update
    void Start()
    {
        parentChecker = transform.parent.gameObject.GetComponent<EdgeCheckerScript>();
        if(nextEdge == null)
        {
            Debug.Log(gameObject.name + "の次の角が指定されてないよ");
        }
        objectNum = transform.parent.gameObject.GetComponent<ColorObjectScript>().objectNumber;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float rayDistance = DistanceXY(nextEdge.transform.position,transform.position);
        Vector2 vec = new Vector2(nextEdge.transform.position.x - transform.position.x,nextEdge.transform.position.y - transform.position.y);
        int touchCounter = 0;
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, vec, rayDistance);
        foreach(RaycastHit2D hit in Physics2D.RaycastAll(transform.position, vec, rayDistance))
        {
            //Debug.Log("発射");
            //Debug.DrawRay(transform.position,vec, Color.red, 0.1f, false);
            if(hit.collider)
            {
                if(hit.collider.gameObject.GetComponent<ColorObjectScript>())
                {
                    //Debug.Log(gameObject.transform.parent.gameObject.name + ":" + hit.collider.gameObject.name);
                    if(hit.collider.gameObject.GetComponent<ColorObjectScript>().objectNumber != objectNum)
                    {
                        //Debug.Log(gameObject.transform.parent.gameObject.name + "::" + hit.collider.gameObject.name);
                        ColorObjectScript otherColorObject = hit.collider.gameObject.GetComponent<ColorObjectScript>();
                        if(otherColorObject.isObject && !otherColorObject.noBody)
                        {
                            //Debug.Log(gameObject.transform.parent.gameObject.name + ":::" + hit.collider.gameObject.name);
                            parentChecker.edgeObjectList[EdgeNum] = true;
                            touchCounter++;
                            break;
                        }
                    }
                }
                //Debug.Log("何にも当たらない");
                
            }
            parentChecker.edgeObjectList[EdgeNum] = false;    
        }
        if(touchCounter == 0)parentChecker.edgeObjectList[EdgeNum] = false;    
    }

    float DistanceXY(Vector3 vec1,Vector3 vec2)
    {
        return Mathf.Sqrt((vec1.x - vec2.x)*(vec1.x - vec2.x) + (vec1.y - vec2.y)*(vec1.y - vec2.y));
    }
    
    /*
    void OnTriggerStay2D(Collider2D otherObject)
    {
        //Debug.Log("何かしらには当たってる");
        if(otherObject.gameObject.GetComponent<ColorObjectScript>())
        {
            if(otherObject.gameObject.GetComponent<ColorObjectScript>().isObject)
            {
            
                Debug.Log(otherObject.gameObject.name);
                Debug.Log("isObject:" + otherObject.gameObject.GetComponent<ColorObjectScript>().isObject);
                Debug.Log("noBody:" + otherObject.gameObject.GetComponent<ColorObjectScript>().noBody);
                if(//transform.parent.gameObject.GetComponent<ColorObjectScript>().noBody 
                !otherObject.gameObject.GetComponent<ColorObjectScript>().noBody)
                {
                    Debug.Log("タッチング！！！");
                    parentChecker.touchingObject = true;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D otherObject)
    {
        if(otherObject.gameObject.GetComponent<ColorObjectScript>())
        {
            if(//transform.parent.gameObject.GetComponent<ColorObjectScript>().noBody 
             (otherObject.gameObject.GetComponent<ColorObjectScript>().isObject 
            && !otherObject.gameObject.GetComponent<ColorObjectScript>().noBody))
            {
                parentChecker.touchingObject = false;
            }
        }
    }
    */
}
