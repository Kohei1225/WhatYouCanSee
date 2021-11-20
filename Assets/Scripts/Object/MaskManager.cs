using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool isShrink = false;
    public float speed = 100;
    public float maxScale = 200;
    public GameObject blackBoard;
    // Start is called before the first frame update
    void Start()
    {
        blackBoard.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
            return;

        if (isShrink)
        {
            //円が小さくなる
            transform.localScale = transform.localScale - (Vector3.right + Vector3.up) * speed * Time.deltaTime;
            if(transform.localScale.x <= 0 && transform.localScale.y <= 0)
            {
                canMove = false;
                //x,yの大きさを0にする
                transform.localScale = Vector3.zero + Vector3.forward;
            }
        }
        else
        {
            //円が大きくなる
            transform.localScale = transform.localScale + (Vector3.right + Vector3.up) * speed * Time.deltaTime;
            if (transform.localScale.x >= maxScale && transform.localScale.y >= maxScale)
            {
                canMove = false;
                //x,yの大きさを最大にする
                transform.localScale = (Vector3.right + Vector3.up) * maxScale + Vector3.forward;
            }
        }
    }

    public void Set_canMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public bool Get_canMove()
    {
        return canMove;
    }

    public void Set_isShrink(bool isShrink)
    {
        this.isShrink = isShrink;
    }

    public float Get_speed()
    {
        return speed;
    }

    public void Set_speed(float speed)
    {
        this.speed = speed;
    }
}
