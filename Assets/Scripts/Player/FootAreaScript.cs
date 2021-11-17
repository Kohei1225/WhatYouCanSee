using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//足の当たり判定のクラス
public class FootAreaScript : MonoBehaviour
{
    public bool touchingStage{get; private set;}

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        touchingStage = false;
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Stage"
        || (other.gameObject.tag == "ColorObject"
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>() 
        && !other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>().Get_noBody()
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>().isObject)
        || (other.gameObject.tag == "ColorObject"
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>() 
        && !other.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>().noBody 
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>().isObject))
        {
            touchingStage = true;
            //Debug.Log("ステージに当たった");
            //player.GetComponent<PlayerController>().onStage = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Stage"
        || (other.gameObject.tag == "ColorObject"
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>() 
        && !other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>().Get_noBody()
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectVer3>().isObject))
        {

            touchingStage = false;
            //player.GetComponent<PlayerController>().onStage = false;
            //Debug.Log("離れた");
        }
    }
}
