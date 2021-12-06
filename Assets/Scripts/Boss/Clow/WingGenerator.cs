using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingGenerator : MonoBehaviour
{
    /// <summary> 羽を飛ばす周期(この回数回る度に羽を飛ばす) </summary>
    [SerializeField] private int _RotatePeriod = 1;
    /// <summary> 回転スピード </summary>
    private float _RotateSpeed = 33.75f;
    /// <summary> 一周した回数 </summary>
    public int _RotatePeriodCounter = 0;
    /// <summary> 羽を飛ばす角度の範囲の最小値 </summary>
    private int _MinAngle = 10;// + 180;
    /// <summary> 羽を飛ばす角度の範囲の最大値 </summary>
    private int _MaxAngle = 90;// + 180;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary> 羽を飛ばす </summary>
    /// <param name="wing">飛ばす羽のオブジェクト</param>
    /// <param name="dir">飛ばす方向</param>
    /// <param name="color">羽の色</param>
    public void ShotWing(GameObject wing,int dir ,ColorObjectVer3.OBJECT_COLOR3 color)
    {
        transform.Rotate(0,0,_RotateSpeed);

        var minAngle = _MinAngle;
        var maxAngle = _MaxAngle;

        if (dir < 0)
        {
            minAngle += 180;
            maxAngle += 180;
        }

        //一周するたびにカウントする
        if(Mathf.Abs( gameObject.transform.localEulerAngles.z) < 0.1f)
        {
            _RotatePeriodCounter++;
            if(_RotatePeriodCounter % _RotatePeriod == 0)
            {
                Debug.Log("RotatePeriodCounter::" + _RotatePeriodCounter);
            }
        }

        //一定のカウント数で羽を飛ばす
        if (_RotatePeriodCounter % _RotatePeriod == 0)
        {
            if(minAngle<= transform.localEulerAngles.z && transform.localEulerAngles.z <= maxAngle)
            {
                GameObject wingObj = Instantiate(wing, transform.position, transform.rotation);
                wingObj.GetComponent<ColorObjectVer3>().colorType = color;
            }
            
        }
    }

    /// <summary> 周期をリセット </summary>
    public void ResetPeriodCounter()
    {
        _RotatePeriodCounter = 0;
    }
}
