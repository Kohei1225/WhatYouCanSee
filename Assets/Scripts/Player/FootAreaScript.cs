using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//足の当たり判定のクラス
public class FootAreaScript : MonoBehaviour
{
    GameObject player;
    public bool touchingStage;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        player.GetComponent<PlayerController>().onStage = touchingStage;
        touchingStage = false;
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == "Stage"
        || (other.gameObject.tag == "ColorObject"
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>() 
        && !other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>().noBody 
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>().isObject)
        || (other.gameObject.tag == "ColorObject"
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>() 
        && !other.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>().noBody 
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObject2>().isObject))
        {
            touchingStage = true;
            //player.GetComponent<PlayerController>().onStage = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Stage"
        || (other.gameObject.tag == "ColorObject"
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>() 
        && !other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>().noBody 
        && other.gameObject.transform.parent.gameObject.GetComponent<ColorObjectScript>().isObject))
        {
            touchingStage = false;
            //player.GetComponent<PlayerController>().onStage = false;
            //Debug.Log("離れた");
        }
    }
}
