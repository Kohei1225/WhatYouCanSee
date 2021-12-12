using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public enum StateEnum
    {
        BOARD_FADE_OUT,
        WAIT,
        CLEAR_FADE_IN,
        CLEAR_FADE_OUT,
        THANK_FADE_IN,
        THANK_FADE_OUT,
        ENDROLL,
        BOARD_FADE_IN,
        END
    }
    [SerializeField] private StateEnum _State;
    //Wait後に移るステートなど
    [SerializeField] private StateEnum _TmpState;

    private ChangeAlpha _ChangeAlphaClear;
    private ChangeAlpha _ChangeAlphaThank;
    private EndRollManager _EndRollManager;

    private float _WaitTime;
    [SerializeField] private float _StartWaitTime = 3;
    [SerializeField] private float _PrintTextTime = 5;
    [SerializeField] private float _PrintThankTime = 5;
    [SerializeField] private float _WaitTimeBeforeFadeIn = 2;
    [SerializeField] private float _EndWaitTime = 2;
    [SerializeField] private float _Time = 0;

    //ブラックボードのspriterenderer
    [SerializeField] private SpriteRenderer _SpriteRendererBoard;
    //ブラックボードが濃くなる時間
    [SerializeField] private float _FadeTime = 3;

    [SerializeField] private PlayerController _PlayerController;

    // Start is called before the first frame update
    void Start()
    {
        _ChangeAlphaClear = transform.Find("ClearText").GetComponent<ChangeAlpha>();
        _ChangeAlphaThank = transform.Find("ThankText").GetComponent<ChangeAlpha>();
        _EndRollManager = transform.Find("EndRollText").GetComponent<EndRollManager>();

        _State = StateEnum.BOARD_FADE_OUT;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_State)
        {
            case StateEnum.BOARD_FADE_OUT:
                _Time += Time.deltaTime;
                Color color = _SpriteRendererBoard.color;
                color.a = Mathf.Lerp(1, 0, _Time / _FadeTime);
                _SpriteRendererBoard.color = color;
                if (_Time >= _FadeTime)
                {
                    _Time = 0;
                    _TmpState = StateEnum.CLEAR_FADE_IN;
                    _State = StateEnum.WAIT;
                    _WaitTime = _StartWaitTime;
                }
                break;
            case StateEnum.CLEAR_FADE_IN:
                if (_ChangeAlphaClear.Get_isFin())
                {
                    _TmpState = StateEnum.CLEAR_FADE_OUT;
                    _State = StateEnum.WAIT;
                    _WaitTime = _PrintTextTime;
                }
                break;
            case StateEnum.WAIT:
                _Time += Time.deltaTime;
                if(_Time >= _WaitTime)
                {
                    if(_TmpState == StateEnum.CLEAR_FADE_IN)
                        //文字を浮かび上がらせる
                        _ChangeAlphaClear.Restart(true);
                    else if(_TmpState == StateEnum.CLEAR_FADE_OUT)
                        //文字を消す
                        _ChangeAlphaClear.Restart(false);
                    else if (_TmpState == StateEnum.THANK_FADE_OUT)
                        //文字を消す
                        _ChangeAlphaThank.Restart(false);
                    _Time = 0;
                    _State = _TmpState;
                }
                break;
            case StateEnum.CLEAR_FADE_OUT:
                if (_ChangeAlphaClear.Get_isFin())
                {
                    _State = StateEnum.ENDROLL;
                    _EndRollManager.enabled = true;
                }
                break;
            case StateEnum.ENDROLL:
                if(_EndRollManager.State == EndRollManager.StateEnum.END)
                {
                    _State = StateEnum.THANK_FADE_IN;
                    //文字を浮かび上がらせる
                    _ChangeAlphaThank.Restart(true);
                }
                break;
            case StateEnum.THANK_FADE_IN:
                if (_ChangeAlphaThank.Get_isFin())
                {
                    _TmpState = StateEnum.THANK_FADE_OUT;
                    _State = StateEnum.WAIT;
                    _WaitTime = _PrintTextTime;
                }
                break;
            case StateEnum.THANK_FADE_OUT:
                if (_ChangeAlphaThank.Get_isFin())
                {
                    _TmpState = StateEnum.BOARD_FADE_IN;
                    _State = StateEnum.WAIT;
                    _WaitTime = _WaitTimeBeforeFadeIn;
                }
                break;
            case StateEnum.BOARD_FADE_IN:
                _Time += Time.deltaTime;
                color = _SpriteRendererBoard.color;
                color.a = Mathf.Lerp(0, 1, _Time / _FadeTime);
                _SpriteRendererBoard.color = color;
                if (_Time >= _FadeTime)
                {
                    _Time = 0;
                    _TmpState = StateEnum.END;
                    _State = StateEnum.WAIT;
                    _WaitTime = _EndWaitTime;
                    //プレイヤーは動けなくなる
                    _PlayerController.Set_canCtrl(false);
                }
                break;
            case StateEnum.END:
                //シーン読み込み
                SceneManager.LoadScene("WorldSelect");
                break;
        }
    }
}
