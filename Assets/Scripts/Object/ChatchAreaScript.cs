using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatchAreaScript : MonoBehaviour
{
    [SerializeField] private GameObject _ObjInArea = null;

    public GameObject ObjInArea
    {
        get
        {
            return _ObjInArea;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckHitColorObj(collision.gameObject, true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckHitColorObj(collision.gameObject, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckHitColorObj(collision.gameObject, false);
    }

    private void CheckHitColorObj(GameObject hitObj, bool isEnter)
    {
        if (hitObj.name != "Body")
            return;
        //当たったオブジェクト(おそらくBody)の親のタグがColorObjctだったら
        if (hitObj.transform.root.gameObject.CompareTag("ColorObject"))
        {
            if (isEnter)
                //登録
                _ObjInArea = hitObj.transform.root.gameObject;
            else
                _ObjInArea = null;
        }
        else
        {
            _ObjInArea = null;
        }
    }
}
