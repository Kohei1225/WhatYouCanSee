using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSwitchScript : MonoBehaviour
{
    public bool isPushed;
    float changeSizeSpeed = 0.04f;
    const float HEIGHT_NOT_PUSH = 0.2f;
    const float HEIGHT_ON_PUSH = 0;

    public GameObject pushButtonObject;

    // Start is called before the first frame update
    void Start()
    {
        isPushed = false;
    }

    // Update is called once per frame
    void Update()
    {
        //バーの傾きを調整
        Vector3 buttonSize = pushButtonObject.transform.localScale;
        float heightDifference = HEIGHT_NOT_PUSH - buttonSize.y;
        if(isPushed)heightDifference = HEIGHT_ON_PUSH - buttonSize.y;

        //十分傾いてたら傾ける操作をしない
        if(Mathf.Abs(heightDifference) > 0.001f)
        {
            //Debug.Log("じゅうぶんかが");
            if(heightDifference > 0)buttonSize.y += changeSizeSpeed;
            if(heightDifference < 0)buttonSize.y -= changeSizeSpeed;
            pushButtonObject.transform.localScale = buttonSize;
        }
        //isPushed = false;
    }

    /*
    void OnCollisionStay2D(Collision2D other)
    {
        isPushed = true;
    }

    void OnCollisionExit2D(Collision2D other)
    {
        isPushed = false;
    }
    */

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<ColorObjectScript>())
        {

        }
        //Debug.Log(other.gameObject.name);
        isPushed = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        isPushed = false;
    }
}
