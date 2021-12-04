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
        GameObject hitObj = collision.gameObject;
        if (hitObj.name != "Body")
            return;
        //当たったオブジェクト(おそらくBody)の親のタグがColorObjctだったら
        if (hitObj.transform.root.gameObject.CompareTag("ColorObject"))
        {
            //登録
            _ObjInArea = hitObj.transform.root.gameObject;
        }
        else
        {
            _ObjInArea = null;
        }
    }
}
