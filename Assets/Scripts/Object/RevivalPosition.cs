using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> カメラの範囲外に出た際に特定の座標に移動する </summary>
public class RevivalPosition : MonoBehaviour
{
    /// <summary> 復活する座標 </summary>
    [SerializeField] private Vector3 _RevivalPos;
    /// <summary> レバー </summary>
    [SerializeField] private LeverScript _Lever = null;
    /// <summary> カメラ外に出たか </summary>
    private bool _IsOutofCamera = false;
    /// <summary> 記録するレバーの位置 </summary>
    private int _CurrentLeverState = 0;

    private Rigidbody2D _RigidBody2 = null;
    private ColorObjectVer3 _ColorObjVer3 = null;


    // Start is called before the first frame update 
    void Start()
    {
        _RigidBody2 = GetComponent<Rigidbody2D>();
        _ColorObjVer3 = GetComponent<ColorObjectVer3>() ?? null;
        _CurrentLeverState = _Lever.barPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //レバーが更新されたら
        if(_CurrentLeverState != _Lever?.barPosition)
        {
            ResetPos();
            _CurrentLeverState = (int)(_Lever?.barPosition);
        }

        //カメラ外に出たら
        if(_IsOutofCamera)
        {
            _IsOutofCamera = false;
            ResetPos();
        }
    }

    private void OnBecameInvisible()
    {
        _IsOutofCamera = true;
    }

    /// <summary> 場所のリセット </summary>
    public void ResetPos()
    {
        _RigidBody2.velocity = Vector2.zero;
        transform.position = _RevivalPos;

        if (_ColorObjVer3 != null)
        {
            _ColorObjVer3.canHold = true;
        }
    }
}
