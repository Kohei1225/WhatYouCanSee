using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レバーにアタッチするクラス
public class LeverScript : MonoBehaviour
{
    /// <summary> 状態を同期させたい他のレバー </summary>
    [SerializeField] LeverScript _SyncLever = null;
    /// <summary>  </summary>
    public int firstBarPos = 0;

    /// <summary> 同期する際の最初のレバーか </summary>
    private bool firstLever = false;
    /// <summary> バーの状態 </summary>
    public int barPosition { get; private set; }
    /// <summary> プレイヤーが近くにいる時(バーを操作できる状態) </summary>
    public bool canChangeBar { get; private set; }
    /// <summary> 実際に動くバーのオブジェクト </summary>
    GameObject barObject;
    /// <summary> バーの傾きのリスト </summary>
    public float[] barAngleList = { 160, 90, 20, 90 };
    /// <summary> レバーが動くスピード </summary>
    [SerializeField] private float roteteSpeed = 300;

    public float timeSum { private set; get; } = 0;



    float startAngle;
    float endAngle;

    // Start is called before the first frame update
    void Awake()
    {
        //バーのオブジェクトを取得
        barObject = transform.Find("BarObjects").gameObject.transform.Find("Lever1_bar").gameObject;

        //バーの傾きを初期化
        barPosition = firstBarPos;

        //バーの傾きを初期化
        //barObject.transform.localEulerAngles = new Vector3(0,0,barAngleList[barPosition]);

        startAngle = barAngleList[barPosition];
        endAngle = startAngle;
        canChangeBar = false;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが近くでCを押すと切り替わる
        if(canChangeBar && Input.GetKeyDown(KeyCode.C))
        {
            ChangeBarPos();
        }

        //バーの傾きを調整
        Vector3 barAngle = barObject.transform.localEulerAngles;
        float angleDifference = barAngleList[barPosition] - barAngle.z;

        timeSum += Time.deltaTime;
        barAngle.z = Mathf.Lerp(startAngle,endAngle, (roteteSpeed * timeSum) / Mathf.Abs(endAngle - startAngle));
        barObject.transform.localEulerAngles = barAngle;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerController>())
        {
            canChangeBar = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerController>())
        {
            canChangeBar = false;
        }
    }

    //バーの位置をリセットするメソッド
    public void ResetBarPos()
    {
        this.barPosition = firstBarPos;
        firstLever = true;
        _SyncLever?.SyncLeverPos(this.barPosition);
    }

    /// <summary> 他のレバーの位置と同期する </summary>
    /// <param name="barPos">同期元のレバーの状態</param>
    public void SyncLeverPos(int barPos)
    {
        //同期元のレバーだったら何もしない
        if(firstLever)
        {
            this.firstLever = false;
            return;
        }

        this.barPosition = barPos;
        _SyncLever.SyncLeverPos(barPos);
    }

    /// <summary> バーの角度を変更 </summary>
    public void ChangeBarPos()
    {
        Debug.Log("LeverScript");
        timeSum = 0;

        
        startAngle = barObject.transform.localEulerAngles.z;
        barPosition++;
        
        //配列の最後まで来たら最初に戻る
        if(barPosition >= barAngleList.Length)barPosition = 0;
        endAngle = barAngleList[barPosition];

        //他のレバーが連動するなら連動させる
        firstLever = true;
        _SyncLever?.SyncLeverPos(this.barPosition);

    }

}
