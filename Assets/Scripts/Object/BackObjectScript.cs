using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//背景の手前にある背景の当たり判定用(四角のみ)
public class BackObjectScript : MonoBehaviour
{
    public float xScale;
    public float yScale;
    //GameObject Target;
    GameManagerScript gameManagerScript;
    const float PLAYERX = 0.3099501f;
    const float PLAYERTOP = 0.1456f;

    // Start is called before the first frame update
    void Start()
    {
        xScale = transform.localScale.x;
        yScale = transform.localScale.y;
        gameManagerScript = GameObject.Find("Managers").transform.Find("GameManager").gameObject.GetComponent<GameManagerScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for(int i = 0;i < gameManagerScript.objectList.Length;i++)
        {
            GameObject target = gameManagerScript.objectList[i];

            //自身に入ってたら
            float right = target.transform.position.x + Mathf.Abs(target.transform.localScale.x)/2f;
            float left = target.transform.position.x - Mathf.Abs(target.transform.localScale.x)/2f;
            float top = target.transform.position.y + target.transform.localScale.y/2f;
            float bottom = target.transform.position.y - target.transform.localScale.y/2f;

            if(i == 0)
            {
                right = target.transform.position.x + Mathf.Abs(target.transform.localScale.x * PLAYERX);
                left = target.transform.position.x - Mathf.Abs(target.transform.localScale.x) * PLAYERX;
                top = target.transform.position.y + (target.transform.localScale.y * PLAYERTOP);
            }

            if(i == -1)
            {
                Debug.Log("target.right:" + right + " me:" + (transform.position.x + xScale/2f));
                Debug.Log("target.left:" + left + " me:" + (transform.position.x - xScale/2f));
                Debug.Log("target.top:" + top + " me:" + (transform.position.y + yScale/2f));
                Debug.Log("target.bottom:" + bottom + " me:" + (transform.position.y - yScale/2f));
            }

            if(right <= transform.position.x + xScale/2f
            && left >= transform.position.x - xScale/2f
            && top <= transform.position.y + yScale/2f
            && bottom >= transform.position.y - yScale/2f)
            {
                //Debug.Log(target.name + "は完全に入ったね");
                if(GetComponent<ColorObjectScript>().colorType == target.GetComponent<ColorObjectScript>().colorType)
                {
                    target.GetComponent<EdgeCheckerScript>().sameColor = true;
                    //Debug.Log(gameObject.name + "と" + target.name + "は同じ色");
                }
                else 
                {
                    target.GetComponent<EdgeCheckerScript>().sameColor = false;
                    target.GetComponent<ColorObjectScript>().inSameColor = false;
                }
                    
            }
            //もし完全に外に出てない場合
            else if(!(right < transform.position.x - xScale/2f
                || left > transform.position.x + xScale/2f
                || top < transform.position.y - yScale/2f
                || bottom > transform.position.y + yScale/2f))
            {
                target.GetComponent<ColorObjectScript>().inSameColor = false;
            }
            //else target.GetComponent<EdgeCheckerScript>().sameColor = false;
        }
        
    }
}
