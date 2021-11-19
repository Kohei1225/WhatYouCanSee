using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> ボスが死んだ後のアニメーションの再生を管理 </summary>
public class AnimAfterBossDead : MonoBehaviour
{
    /// <summary> 対象のボス </summary>
    [SerializeField] private PandaScript _Boss = null;
    /// <summary> 再生するアニメーションの名前 </summary>
    [SerializeField] private string _AnimStateName;
    /// <summary> 死んでから再生するまでの時間 </summary>
    [SerializeField] private float _StartAnimTime = 0;

    /// <summary> アニメーター </summary>
    private Animator _AnimController = null;

    /// <summary> 時間を計測するインスタンス </summary>
    private TimerScript _Timer = new TimerScript();
    /// <summary> ボスが既に死んでるかどうか </summary>
    private bool hasDead = false;
    /// <summary> 既にアニメーションを再生したか </summary>
    private bool _HasPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        _AnimController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_Boss == null)
        {
            return;
        }

        if (!hasDead)
        {
            //ボスが死んだタイミングで時間の計測を開始
            if (_Boss.IsDead)
            {
                hasDead = true;
                _Timer.ResetTimer(_StartAnimTime);
            }
        }
        else
        {
            if (!_HasPlay)
            {
                //アニメーションが再生されるまで時間を計測する
                _Timer.UpdateTimer();
                if(_Timer.IsTimeUp)
                {
                    //指定された時間になったらアニメーションを再生
                    _AnimController.Play(_AnimStateName, 0, 0);
                    _HasPlay = _Timer.IsTimeUp;
                }
            }
        }
    }
}
