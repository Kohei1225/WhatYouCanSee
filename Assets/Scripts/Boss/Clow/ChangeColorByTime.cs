using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorByTime : MonoBehaviour
{
    /// <summary> 色を変えられるかどうか </summary>
    public bool _CanChangeColor = false;
    /// <summary> 色を変更するインターバル </summary>
    [SerializeField] private float _ColorChangeInterval = 1f;
    /// <summary> 変える色の配列t </summary>
    [SerializeField] private ColorObjectVer3.OBJECT_COLOR3[] _ColorList = null;
    /// <summary> 色の番号 </summary>
    private int _ColorNum = 0;
    /// <summary> カラーオブジェクトのクラス </summary>
    private ColorObjectVer3 _ColorObjectScript = null;
    /// <summary> 時間計測用インスタンス </summary>
    private TimerScript _Timer = new TimerScript();

    int ColorNum
    {
        set
        {
            if(value < 0 || _ColorList.Length <= value)
            {
                value = 0;
            }
            this._ColorNum = value;
        }

        get { return this._ColorNum; }
    }

    // Start is called before the first frame update
    void Start()
    {
        _CanChangeColor = false;
        _ColorObjectScript = GetComponent<ColorObjectVer3>();
        _Timer.ResetTimer(_ColorChangeInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if(_CanChangeColor)
        {
            _Timer.UpdateTimer();

            if(_Timer.IsTimeUp)
            {
                //一定時間経ったら色を変える
                UpdateColor();
                _Timer.ResetTimer(_ColorChangeInterval);
            }
        }
    }

    /// <summary>  </summary>
    public void UpdateColor()
    {
        ColorNum++;
        _ColorObjectScript.colorType = _ColorList[ColorNum];
    }
}
