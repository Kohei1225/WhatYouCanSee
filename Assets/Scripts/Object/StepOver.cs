using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepOver : MonoBehaviour
{
    //up,downの順
    public GameObject[] _AreaInObjsParent;
    private AreaInObj[][] _AreaInObjs;
    private Rigidbody2D _Rigidbody2D;
    public float height = 0.8f;
    private Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        _Rigidbody2D = GetComponent<Rigidbody2D>();
        _AreaInObjs = new AreaInObj[_AreaInObjsParent.Length][];
        for(int i = 0; i < _AreaInObjs.Length; i++)
        {
            _AreaInObjs[i] = new AreaInObj[2];
            for(int j = 0; j < _AreaInObjs[i].Length; j++)
            {
                GameObject obj = _AreaInObjsParent[i].transform.GetChild(j).gameObject;
                _AreaInObjs[i][j] = obj.GetComponent<AreaInObj>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            Vector3 beforePos = pos;
            pos = transform.position;
            if (Mathf.Abs((pos - beforePos).x) != 0)
            {
                bool canOver = false;
                for (int i = 0; i < _AreaInObjs.Length; i++)
                {
                    //down && !up
                    canOver |= _AreaInObjs[i][0].IsIn && !_AreaInObjs[i][1].IsIn;
                }
                if (canOver)
                {
                    transform.Translate(Vector3.up * height);
                    Debug.Log("Object up");
                }
            }
        }
    }
}
