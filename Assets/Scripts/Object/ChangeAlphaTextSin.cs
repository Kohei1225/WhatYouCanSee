using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeAlphaTextSin : ChangeColor
{
    private TextMeshProUGUI _TextMeshProUGUI;
    private float _Time = 0;

    protected override void Start()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        base.Start();
    }

    protected override Color getChangeColor()
    {
        //もじが見えていないときは
        if (!_TextMeshProUGUI.enabled)
        {
            //時間リセット
            _Time = 0;
        }
        _Time += Time.deltaTime;
        float alpha = Mathf.Sin(_Time * _ChangeSpeed);
        return new Color(_Color.r, _Color.g, _Color.b, alpha);
    }

    protected override void SetInitColor()
    {
        _Color = _TextMeshProUGUI.color;
    }

    protected override void UpdateColor()
    {
        _TextMeshProUGUI.color = _Color;
    }
}
