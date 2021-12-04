using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOChatcherScript : MonoBehaviour
{
    private bool _InChatchArea = false;
    [SerializeField] private GameObject _ChatchArea;
    [SerializeField] private float _MoveX = 50;
    private float[] _RangeX;
    private int _Dir = 1;
    private float _FirstPosY;
    private Vector3 _FromPos, _ToPos;
    [SerializeField] private float _MoveSpeed = 2;
    private float _Time = 0;
    [SerializeField] private GameObject _CatchedObj = null;
    private float _ObjGravityScale;
    [SerializeField] private float _WaitTime = 2;

    public enum StateEnum
    {
        MOVE,
        FIND_MOVE,
        DOWN,
        CATCH,
        UP,
    }
    [SerializeField] private StateEnum _State;
    // Start is called before the first frame update
    void Start()
    {
        _RangeX = new float[] {transform.position.x - _MoveX, transform.position.x + _MoveX };
        _State = StateEnum.MOVE;
        SetToFromPosX(_RangeX[_Dir]);
    }

    // Update is called once per frame
    void Update()
    {
        ChatchAreaScript chatchAreaScript = _ChatchArea?.GetComponent<ChatchAreaScript>();
        GameObject objInArea = chatchAreaScript?.ObjInArea;

        switch (_State)
        {
            case StateEnum.MOVE:
                if (Move())
                {
                    //入れ替え
                    _Dir ^= 1;
                    SetToFromPosX(_RangeX[_Dir]);
                }
                if(objInArea != null)
                {
                    _State = StateEnum.FIND_MOVE;
                    SetToFromPosX(objInArea.transform.position.x);
                }
                break;
            case StateEnum.FIND_MOVE:
                if (Move())
                {
                    _State = StateEnum.DOWN;
                    SetToFromPosY(chatchAreaScript.ObjInArea.transform.position.y);
                }
                if(objInArea == null)
                {
                    _State = StateEnum.MOVE;
                    SetToFromPosX(_RangeX[_Dir]);
                }
                break;
            case StateEnum.DOWN:
                if (Move())
                {
                    _State = StateEnum.CATCH;
                    _Time = 0;
                }
                if (objInArea == null)
                {
                    _State = StateEnum.UP;
                    SetToFromPosY(_FirstPosY);
                }
                break;
            case StateEnum.CATCH:
                _Time += Time.deltaTime;
                if(_Time <= _WaitTime)
                {
                    _State = StateEnum.UP;
                    SetToFromPosY(_FirstPosY);
                    //捕まえたオブジェクトを記憶
                    _CatchedObj = objInArea;
                    //objを子オブジェクトにする
                    _CatchedObj.transform.parent = transform;
                    //重力の影響を受けないようにする
                    _CatchedObj.GetComponent<Rigidbody2D>().gravityScale = 0;
                }
                if (objInArea == null)
                {
                    _State = StateEnum.UP;
                    SetToFromPosY(_FirstPosY);
                }
                break;
            case StateEnum.UP:
                if (Move())
                {
                    _State = StateEnum.MOVE;
                    SetToFromPosX(_RangeX[_Dir]);
                }
                if (_CatchedObj != null)
                {
                    if (!_CatchedObj.CompareTag("ColorObject"))
                    {
                        //重力の影響を受けるようにする
                        _CatchedObj.GetComponent<Rigidbody2D>().isKinematic = false;
                        //オブジェクトとUFOキャッチャーの親子関係解除
                        _CatchedObj.transform.parent = null;
                        _CatchedObj = null;
                    }
                }
                break;
        }
    }

    private bool Move()
    {
        _Time += Time.deltaTime;
        float t = _Time * _MoveSpeed / (_ToPos - _FromPos).magnitude;
        transform.position = Vector3.Lerp(_FromPos, _ToPos, t);
        if(t >= 1)
        {
            return true;
        }
        return false;
    }

    private void SetToFromPosX(float toPosX)
    {
        _FromPos = transform.position;
        _ToPos = new Vector3(toPosX, transform.position.y, transform.position.z);
        _Time = 0;
    }

    private void SetToFromPosY(float toPosY)
    {
        _FromPos = transform.position;
        _ToPos = new Vector3(transform.position.x, toPosY, transform.position.z);
        _Time = 0;
    }
}
