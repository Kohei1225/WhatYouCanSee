using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentScript : MonoBehaviour
{
    [SerializeField] private float _TransparentSec = 1.5f;
    private bool _IsFin = false;
    private float time = 0;
    [SerializeField] private float _MinAlpha = 0.2f;
    private float _FirstAlpha;
    private float _EndAlpha;
    private SpriteRenderer _SpriteRenderer;
    private ChameleonScript _ChameleonScript;
    private bool _CanMove = false;


    public bool IsFin
    {
        get
        {
            return _IsFin;
        }
    }

    public bool IsTransparent { private get; set; } = true;
    // Start is called before the first frame update
    void Start()
    {
        _SpriteRenderer = GetComponent<SpriteRenderer>();
        _ChameleonScript = GetComponent<ChameleonScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsFin || !_CanMove)
            return;
        time += Time.deltaTime;
        Color color = _SpriteRenderer.color;
        //アルファ値が変化
        _SpriteRenderer.color = new Color(color.r, color.g, color.b, Mathf.Lerp(_FirstAlpha, _EndAlpha, time / _TransparentSec));
        //最終目的の透明度になったら
        if(_SpriteRenderer.color.a == _EndAlpha)
        {
            _IsFin = true;
            _CanMove = false;
        }
    }

    /// <summary> 最初にこれを呼んで初期化
    public void StartTransparent()
    {
        if (IsTransparent)
        {
            _FirstAlpha = 1;
            _EndAlpha = _MinAlpha;
        }
        else
        {
            _FirstAlpha = _MinAlpha;
            _EndAlpha = 1;
        }
        if(_SpriteRenderer.color.a == _EndAlpha)
        {
            _IsFin = true;
            return;
        }
        _IsFin = false;
        _CanMove = true;
        time = 0;
    }
}
