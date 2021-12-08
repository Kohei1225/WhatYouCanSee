using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private bool canMove = true;
    private bool _IsFin = false;
    [SerializeField] private bool isShrink = false;
    public float speed = 100;
    public float maxScale = 200;
    public GameObject blackBoard;

    public bool IsFin
    {
        get
        {
            return _IsFin;
        }
        set
        {
            this._IsFin = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        blackBoard.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove || _IsFin)
            return;

        if (isShrink)
        {
            //円が小さくなる
            transform.localScale = transform.localScale - (Vector3.right + Vector3.up) * speed * Time.deltaTime;
            if(transform.localScale.x <= 0 && transform.localScale.y <= 0)
            {
                canMove = false;
                _IsFin = true;
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
                _IsFin = true;
                //x,yの大きさを最大にする
                transform.localScale = (Vector3.right + Vector3.up) * maxScale + Vector3.forward;
            }
        }
    }

    public void StartMask(bool isShrink)
    {
        this.isShrink = isShrink;
        _IsFin = false;
        canMove = true;
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
