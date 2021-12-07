using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 一定の間隔で指定した角度までの回転を続けるクラス </summary>
public class IntervalRotate : MonoBehaviour
{
    /// <summary> インターバル(回らない時間) </summary>
    [SerializeField] private float _TimeInterval = 1.0f;
    /// <summary> 角度の配列 </summary>
    [SerializeField] private float[] _Angles;
    /// <summary> 回転速度 </summary>
    [SerializeField] private float _RotateSpeed = 100;

    /// <summary> 回転前の角度 </summary>
    private float _StartAngle = 0;
    /// <summary> 目標の角度 </summary>
    private float _EndAngle = 0;
    /// <summary>  </summary>
    private int _NextAngle = 1;
    /// <summary> 回転の際に使う時間計測用 </summary>
    private float _TimeSum = 0;
    /// <summary> 時間計測用のインスタンス </summary>
    private TimerScript _Timer = new TimerScript();

    /// <summary> 回れるかどうかの判定 </summary>
    public bool _CanRotate = false;

    // Start is called before the first frame update
    void Start()
    {
        _TimeSum = 0;

        var firstAngle = transform.localEulerAngles;

        firstAngle.z = _Angles[0];

        transform.localEulerAngles = firstAngle;

        _StartAngle = transform.localEulerAngles.z;

        _EndAngle = _Angles[_NextAngle];
    }

    // Update is called once per frame
    void Update()
    {
        //回れる状態じゃなければ飛ばす
        if (!_CanRotate)
        {
            //Debug.Log("_CanRotate = False");
            _Timer.UpdateTimer();

            if (_Timer.IsTimeUp)
            {
                _CanRotate = true;
            }
            return;   
        }

        //回れる状態なら
        if(_CanRotate)
        {
            _TimeSum += Time.deltaTime;

            
            var angle = transform.localEulerAngles;

            //実際に目的の角度まで回転させる
            angle.z = Mathf.Lerp(_StartAngle, _EndAngle, (_RotateSpeed * _TimeSum) / Mathf.Abs(_EndAngle - _StartAngle));
            transform.localEulerAngles = angle;

            //目的の角度まで回ったら時間等をリセット
            if(Mathf.Abs(transform.localEulerAngles.z - _EndAngle) < 1.0f)
            {
                //次の角度に強制的に合わせる
                angle.z = _EndAngle;
                transform.localEulerAngles = angle;

                //タイマーをリセット
                _Timer.ResetTimer(_TimeInterval);
                _CanRotate = false;

                _TimeSum = 0;
                _NextAngle++;
                if(_NextAngle == _Angles.Length)
                {
                    _NextAngle = 0;
                }
                _StartAngle = transform.localEulerAngles.z;
                _EndAngle = _Angles[_NextAngle];

                if(_EndAngle == 0)
                {
                    angle.z = 0;
                    transform.localEulerAngles = angle;
                }

            }
        }
    }
}
