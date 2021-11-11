using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSignboardScript : MonoBehaviour
{
    //動くのか
    private bool isMove = false;
    //上がるのか
    public bool firstIsUp;
    public float minY, maxY;
    public float upSpeed = 1;
    private int moveVec;
    private bool isUp;
    private bool isFin = false;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(firstIsUp);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMove)
            return;

        float newY = rectTransform.anchoredPosition.y + upSpeed * moveVec * Time.deltaTime;
        if(newY <= minY || newY >= maxY)
        {
            isMove = false;
            isFin = true;
        }
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, Mathf.Clamp(newY, minY, maxY));
    }

    public void Set_isMove(bool isMove)
    {
        this.isMove = isMove;
    }

    public bool Get_isMove()
    {
        return isMove;
    }

    public bool Get_isUp()
    {
        return isUp;
    }

    public bool Get_isFin()
    {
        return isFin;
    }

    public void Init(bool isUp)
    {
        if (isUp)
        {
            //y = minY;
            moveVec = 1;
        }
        else
        {
            //y = maxY;
            moveVec = -1;
        }
        //rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, y);
        this.isUp = isUp;
        isFin = false;
    }
}
