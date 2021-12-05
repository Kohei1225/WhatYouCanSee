using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOChatcherScript : MonoBehaviour
{
    private bool _InChatchArea = false;
    [SerializeField] private GameObject _ChatchArea;
    [SerializeField] private GameObject _CatchObj = null;
    [SerializeField] private float _MoveX = 50;
    private float[] _RangeX;
    private int _Dir = 1;
    private float _FirstPosY;
    private Vector3 _FromPos, _ToPos;
    [SerializeField] private float _MoveSpeed = 2;
    private float _Time = 0;
    [SerializeField] private GameObject _AreaObj = null;
    //private bool isCatch = false;
    private float _ObjGravityScale;
    [SerializeField] private float _WaitTime = 2;
    private LineRenderer _LineRenderer;
    [SerializeField] private float _DeltaCatchY = -5;

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
        _LineRenderer = GetComponent<LineRenderer>();
        Vector3[] poses = new Vector3[] { transform.position - Vector3.right * _MoveX, transform.position + Vector3.right * _MoveX};
        _LineRenderer.positionCount = poses.Length;
        _LineRenderer.SetPositions(poses);
        _State = StateEnum.MOVE;
        SetToFromPosX(_RangeX[_Dir]);
    }

    // Update is called once per frame
    void Update()
    {
        InAreaScript inAreaScript = _ChatchArea?.GetComponent<InAreaScript>();
        GameObject beforeAreaObj = _AreaObj;
        _AreaObj = inAreaScript?.ObjInArea;

        switch (_State)
        {
            case StateEnum.MOVE:
                if (Move())
                {
                    //入れ替え
                    _Dir ^= 1;
                    SetToFromPosX(_RangeX[_Dir]);
                }
                else
                {
                    if (_AreaObj != null && _CatchObj == null)
                    {
                        _State = StateEnum.FIND_MOVE;
                        SetToFromPosX(_AreaObj.transform.position.x);
                    }
                }
                break;
            case StateEnum.FIND_MOVE:
                if (Move())
                {
                    _State = StateEnum.DOWN;
                    SetToFromPosY(inAreaScript.ObjInArea.transform.position.y - _DeltaCatchY);
                }
                break;
            case StateEnum.DOWN:
                if (Move())
                {
                    _State = StateEnum.CATCH;
                    _Time = 0;
                }
                break;
            case StateEnum.CATCH:
                _Time += Time.deltaTime;
                if(_Time >= _WaitTime)
                {
                    _State = StateEnum.UP;
                    SetToFromPosY(_FirstPosY);
                    //objを子オブジェクトにする
                    _AreaObj.transform.parent = transform;
                    //重力をなしに
                    _AreaObj.GetComponent<Rigidbody2D>().isKinematic = true;
                    ////位置をずらす
                    //_AreaObj.transform.Translate(Vector3.up * _DeltaCatchY);
                    //捕まえたobj記憶
                    _CatchObj = _AreaObj;
                }
                break;
            case StateEnum.UP:
                if (Move())
                {
                    _State = StateEnum.MOVE;
                    SetToFromPosX(_RangeX[_Dir]);
                }
                break;
        }
        if (_CatchObj != null)
        {
            if (!_CatchObj.CompareTag("ColorObject") || !_CatchObj.transform.Find("Body").gameObject.activeSelf)
            {
                //重力の影響を受けるようにする
                //_TargetObj.GetComponent<Rigidbody2D>().gravityScale = _ObjGravityScale;
                _CatchObj.GetComponent<Rigidbody2D>().isKinematic = false;
                //オブジェクトとUFOキャッチャーの親子関係解除
                _CatchObj.transform.parent = null;
                _CatchObj = null;
            }
        }
        else
        {
            if(beforeAreaObj != null && _AreaObj == null)
            {
                switch (_State)
                {
                    case StateEnum.FIND_MOVE:
                        _State = StateEnum.MOVE;
                        SetToFromPosX(_RangeX[_Dir]);
                        break;
                    case StateEnum.DOWN:
                        _State = StateEnum.UP;
                        SetToFromPosY(_FirstPosY);
                        break;
                    case StateEnum.CATCH:
                        _State = StateEnum.UP;
                        SetToFromPosY(_FirstPosY);
                        break;
                }
            }
            else if (beforeAreaObj == null && _AreaObj != null)
            {
                if (_State == StateEnum.UP)
                {
                    _State = StateEnum.DOWN;
                    SetToFromPosY(_AreaObj.transform.position.y - _DeltaCatchY);
                }
            }
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
