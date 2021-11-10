using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager_Map : MonoBehaviour
{
    private bool isShrink = false;
    public float shrinkSpeed = 100;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isShrink)
        {
            //円が小さくなる
            transform.localScale = transform.localScale - (Vector3.right + Vector3.up) * shrinkSpeed * Time.deltaTime;
            if(transform.localScale.x <= 0 && transform.localScale.y <= 0)
            {
                isShrink = false;
                //x,yの大きさを0にする
                transform.localScale = Vector3.zero + Vector3.forward;
            }
        }
    }

    public void Set_isShrink(bool isShrink)
    {
        this.isShrink = isShrink;
    }

    public bool Get_isShrink()
    {
        return isShrink;
    }
}
