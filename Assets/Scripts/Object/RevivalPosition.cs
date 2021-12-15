using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> カメラの範囲外に出た際に特定の座標に移動する </summary>
public class RevivalPosition : MonoBehaviour
{
    /// <summary> 復活する座標 </summary>
    [SerializeField] private Vector3 _RevivalPos;

    private bool _IsOutofCamera = false;
    private Rigidbody2D _RigidBody2 = null;
    private ColorObjectVer3 _ColorObjVer3 = null;

    // Start is called before the first frame update 
    void Start()
    {
        _RigidBody2 = GetComponent<Rigidbody2D>();
        _ColorObjVer3 = GetComponent<ColorObjectVer3>() ?? null;
    }

    // Update is called once per frame
    void Update()
    {
        if(_IsOutofCamera)
        {
            _IsOutofCamera = false;
            _RigidBody2.velocity = Vector2.zero;
            transform.position = _RevivalPos;

            if (_ColorObjVer3 != null)
            {
                _ColorObjVer3.canHold = true;
            }
        }
    }

    private void OnBecameInvisible()
    {
        _IsOutofCamera = true;
    }
}
