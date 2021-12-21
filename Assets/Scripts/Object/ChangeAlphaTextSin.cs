using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeAlphaTextSin : ChangeColor
{
    private TextMeshProUGUI _TextMeshProUGUI;
    private float _Time = 0;
    [SerializeField] PlayerController_Map _PlayerController_Map;

    protected override void Start()
    {
        _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        base.Start();
    }

    protected override Color getChangeColor()
    {
        //プレイヤーが動いている時か通常状態じゃないとき
        if (_PlayerController_Map.GetCanMove() || MapManager.screenStatus != MapManager.ScreenStatuses.NORMAL)
        {
            //もじが見えないようにする
            _TextMeshProUGUI.enabled = false;
            //時間リセット
            _Time = 0;
        }
        else
        {
            _TextMeshProUGUI.enabled = true;
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
