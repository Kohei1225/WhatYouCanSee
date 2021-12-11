using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOnObj : MonoBehaviour
{
    [SerializeField] private bool isIn = false;

    private bool isEnter = false;
    private bool isStay = false;
    private bool isExit = false;

    public bool IsIn
    {
        get
        {
            return isIn;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isEnter || isStay)
        {
            isIn = true;
        }
        else if (isExit)
        {
            isIn = false;
        }
        isEnter = false;
        isExit = false;
        isStay = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isEnter = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isStay = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isExit = true;
    }
}
