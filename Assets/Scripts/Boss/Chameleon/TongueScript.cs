using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TongueScript : MonoBehaviour
{
    public enum StateEnum
    {
        WAIT,
        START_STRETCH,
        STRETCHING,
        END_STRETCH,
        NUM
    }

    #region field
    /// <summary> 舌の状態
    [SerializeField] private StateEnum _State;
    /// <summary> 終わったか
    [SerializeField] private bool _IsFin = false;
    /// <summary> 舌の伸びる最大の長さ
    [SerializeField] private float _MaxTongueLength = 40;
    /// <summary> 舌を伸ばす長さ
    private float _TongueLength;
    [SerializeField] private Vector3 _FirstPos;
    [SerializeField] private Vector3 _FromPos;
    [SerializeField] private Vector3 _ToPos;
    /// <summary> 舌の伸びるスピード
    [SerializeField] private float _StretchSpeed = 10;
    /// <summary> 舌の伸び切った時間
    [SerializeField] private float _StretchingTime = 0.8f;
    [SerializeField] private GameObject _Player;
    /// <summary> ターゲットマーカープレハブ </summary>
    [SerializeField] private GameObject _TargetMarkerPrefab;
    private GameObject _TargetMarkerObj;
    public bool isFetch { private get; set; }
    private Collider2D _Collider2D;

    private float time = 0;

    //[SerializeField] private bool _IsReset = false;

    #endregion

    #region property
    public bool IsFin
    {
        get
        {
            return _IsFin;
        }
    }
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        _State = StateEnum.WAIT;
        _FirstPos = transform.localPosition;
        _Collider2D = GetComponent<Collider2D>();
        //当たり判定なし
        _Collider2D.enabled = false;
        //見えなくする
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (_IsReset)
        //{
        //    transform.localPosition = _FirstPos;
        //}
        time += Time.deltaTime;
        switch (_State) {
            case StateEnum.START_STRETCH:
                if (StretchTongue())
                {
                    _State = StateEnum.STRETCHING;
                    //時間初期化
                    time = 0;
                }
                break;
            case StateEnum.STRETCHING:
                if(time >= _StretchingTime)
                {
                    _State = StateEnum.END_STRETCH;
                    //マーカーを削除
                    if (_TargetMarkerObj != null)
                        Destroy(_TargetMarkerObj);
                    //時間初期化
                    time = 0;
                    //位置を変更
                    _ToPos = _FromPos;
                    _FromPos = transform.position;
                    //当たり判定なし
                    _Collider2D.enabled = false;
                }
                break;
            case StateEnum.END_STRETCH:
                if (StretchTongue())
                {
                    _State = StateEnum.WAIT;
                    //時間初期化
                    time = 0;
                    //終わり
                    _IsFin = true;
                    //見えなくする
                    gameObject.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    public void SetStretch()
    {
        //終わったかのをリセット
        _IsFin = false;
        //舌セット
        _FromPos = transform.position;
        _ToPos = _Player.transform.position;
        _TongueLength = Mathf.Clamp((_ToPos - _FromPos).magnitude, 0, _MaxTongueLength);
        //舌が見えるように
        gameObject.SetActive(true);
        //マーカーを作成
        _TargetMarkerObj = Instantiate(_TargetMarkerPrefab, _FromPos + (_ToPos - _FromPos).normalized * _TongueLength, Quaternion.identity);
        //マーカの描画順設定
        _TargetMarkerObj.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
    }

    public void StartStretch()
    {
        if (!_IsFin)
        {
            //時間セット
            time = 0;
            //当たり判定ありに
            gameObject.GetComponent<Collider2D>().enabled = true;
            //状態変更
            _State = StateEnum.START_STRETCH;
        }
    }

    private bool StretchTongue()
    {
        float t = time * _StretchSpeed / _TongueLength;
        //Debug.Log("t:" + t);
        transform.position = Vector3.Lerp(_FromPos, _FromPos + (_ToPos - _FromPos).normalized * _TongueLength, t);
        //Debug.DrawRay(_FirstPos, (_ToPos - _FromPos).normalized * _TongueLength);
        if(t >= 1)
        {
            //終わり
            return true;
        }
        return false;
    }

    public void ResetTongue()
    {
        transform.localPosition = _FirstPos;
        _IsFin = false;
        _State = StateEnum.WAIT;
        //見えなくする
        gameObject.SetActive(false);
        //マーカーを削除
        if (_TargetMarkerObj != null)
            Destroy(_TargetMarkerObj);
    }
}
