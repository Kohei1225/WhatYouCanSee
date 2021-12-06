using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaInObj : MonoBehaviour
{
    [SerializeField] private GameObject obj = null;
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

    public GameObject Obj
    {
        get
        {
            return obj;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnter || isStay)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(obj == null)
        {
            isEnter = true;
            obj = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (obj == null)
            return;
        if (obj.Equals(collision.gameObject))
        {
            isStay = true;
        }
        //Debug.Log(collision.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (obj == null)
            return;
        if (obj.Equals(collision.gameObject))
        {
            isExit = true;
            obj = null;
        }
    }
}
