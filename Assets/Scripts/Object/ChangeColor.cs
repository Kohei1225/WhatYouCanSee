using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangeColor : MonoBehaviour
{
    protected Color _Color;
    [SerializeField] protected float _ChangeSpeed = 10;

    protected virtual void Start()
    {
        SetInitColor();
    }

    //あるカラーをcolorにセット
    protected abstract void SetInitColor();

    protected virtual void Update()
    {
        //カラーを変えて
        _Color = getChangeColor();
        //適用
        UpdateColor();
    }

    //colorを実際に適用する関数
    protected abstract void UpdateColor();

    //カラーを変える関数
    protected abstract Color getChangeColor();

}
