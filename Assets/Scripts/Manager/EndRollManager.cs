using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRollManager : MonoBehaviour
{
    ////元の文字列
    //[SerializeField, TextArea(10, 10)] private string _Str;
    ////整理された文字列
    //private string _NewString;
    ////列数
    //private int _RowNum = 0;
    ////1行に書ける最大の文字数
    //[SerializeField] private int _MaxNumPerRow = 10;
    //テキストコンポーネント
    private Text _Text;
    //上に動くまでの待機時間
    [SerializeField] private float _StartTime = 3;
    //時間測定用
    private TimerScript _TimerScript = new TimerScript();
    //上に動くスピード
    [SerializeField] private float _MoveSpeed = 20;
    //レクトトランスフォーム
    private RectTransform _RectTransform;
    //最終位置
    private float _EndPosY;
    private const float SCREEN_HEIGHT = 1080;

    public enum StateEnum
    {
        WAIT,
        MOVE,
        END
    }
    private StateEnum _State = StateEnum.WAIT;
    // Start is called before the first frame update
    void Start()
    {
        //_Text = GetComponent<Text>();
        _TimerScript.ResetTimer(_StartTime);
        _RectTransform = GetComponent<RectTransform>();
        //_NewString = "";
        //CountRowNum();
        //Debug.Log("元の文字列:" + _Str);
        //Debug.Log("行数:" + _RowNum);
        //Debug.Log("文字列:" + _NewString);
        //_Text.text = _Str;
        _EndPosY = _RectTransform.anchoredPosition.y + SCREEN_HEIGHT + _RectTransform.sizeDelta.y;

    }

    // Update is called once per frame
    void Update()
    {
        switch (_State)
        {
            case StateEnum.WAIT:
                _TimerScript.UpdateTimer();
                if (_TimerScript.IsTimeUp)
                {
                    _State = StateEnum.MOVE;
                }
                break;
            case StateEnum.MOVE:
                Vector3 pos = _RectTransform.anchoredPosition;
                pos.y += Time.deltaTime * _MoveSpeed;
                _RectTransform.anchoredPosition = pos;
                if(_RectTransform.anchoredPosition.y >= _EndPosY)
                {
                    _State = StateEnum.END;
                }
                break;
        }
    }

    //private void CountRowNum()
    //{
    //    int numPerRow = 0;
    //    for(int i = 0; i < _Str.Length; i++)
    //    {
    //        char c = _Str[i];
    //        numPerRow++;
    //        //もし文字が改行だったら
    //        if (c.Equals('\n'))
    //        {
    //            //1行の個数リセット
    //            numPerRow = 0;
    //            //行数加算
    //            _RowNum++;
    //            //テキストに追加
    //            _NewString += c;
    //        }
    //        //もし1行の限界を超えるなら
    //        else if(numPerRow > _MaxNumPerRow)
    //        {
    //            //1行の個数リセット
    //            numPerRow = 0;
    //            //行数加算
    //            _RowNum++;
    //            //テキストに改行を加算
    //            _NewString += '\n';

    //            //テキストにcを追加
    //            _NewString += c;
    //            //1行の個数加算
    //            numPerRow++;
    //        }
    //        else
    //        {
    //            //テキストにcを追加
    //            _NewString += c;
    //        }
    //    }
    //}
}
