using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRollManager : MonoBehaviour
{
    //上に動くまでの待機時間
    [SerializeField] private float _StartTime = 3;
    //流れ終わってからの待機時間
    [SerializeField] private float _EndTime = 3;
    //時間測定用
    private float _Time = 0;
    //上に動くスピード
    [SerializeField] private float _MoveSpeed = 20;
    //レクトトランスフォーム
    private RectTransform _RectTransform;
    //最終位置
    private float _EndPosY;
    private const float SCREEN_HEIGHT = 1080;

    public enum StateEnum
    {
        FIRST_WAIT,
        MOVE,
        END_WAIT,
        END
    }
    private StateEnum _State = StateEnum.FIRST_WAIT;

    public StateEnum State
    {
        get
        {
            return _State;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _Time = 0;
        _RectTransform = GetComponent<RectTransform>();
        _EndPosY = _RectTransform.anchoredPosition.y + SCREEN_HEIGHT + _RectTransform.sizeDelta.y;

    }

    // Update is called once per frame
    void Update()
    {
        switch (_State)
        {
            case StateEnum.FIRST_WAIT:
                _Time += Time.deltaTime;
                if (_Time >= _StartTime)
                {
                    _State = StateEnum.MOVE;
                    _Time = 0;
                }
                break;
            case StateEnum.MOVE:
                Vector3 pos = _RectTransform.anchoredPosition;
                pos.y += Time.deltaTime * _MoveSpeed;
                _RectTransform.anchoredPosition = pos;
                if(_RectTransform.anchoredPosition.y >= _EndPosY)
                {
                    _State = StateEnum.END_WAIT;
                }
                break;
            case StateEnum.END_WAIT:
                _Time += Time.deltaTime;
                if (_Time >= _EndTime)
                {
                    _State = StateEnum.END;
                    _Time = 0;
                }
                break;
        }
    }
}
