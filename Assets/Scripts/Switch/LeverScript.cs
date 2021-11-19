using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//レバーにアタッチするクラス
public class LeverScript : MonoBehaviour
{
    [SerializeField] LeverScript _SyncLever = null;
    private bool firstLever = false;
    public int barPosition{get;private set;}//バーの状態
    public bool existMidPos;//バーが真ん中で止まるかどうか
    bool canChangeBar;//プレイヤーが近くにいる時(バーを操作できる状態)
    GameObject barObject;//実際に動くバーのオブジェクト
    public float[] barAngleList = {160,90,20,90};//バーの傾きのリスト
    private int firstBarPos = 0;


    // Start is called before the first frame update
    void Start()
    {
        //バーのオブジェクトを取得
        barObject = transform.Find("BarObjects").gameObject.transform.Find("Lever1_bar").gameObject;

        //バーの初期位置を設定
        if(existMidPos)firstBarPos = 1;
        else firstBarPos = 0;
        barPosition = firstBarPos;

        //バーの傾きを初期化
        barObject.transform.localEulerAngles = new Vector3(0,0,barAngleList[barPosition]);

        canChangeBar = false;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーが近くでCを押すと切り替わる
        if(canChangeBar && Input.GetKeyDown(KeyCode.C))
        {
            //３段階以上で変えられる時
            if(existMidPos)
            {
                barPosition++;      
            }
            //２段階の時
            else
            {
                barPosition += 2;
            }

            //配列の最後まで来たら最初に戻る
            if(barPosition >= barAngleList.Length)barPosition = 0;

            //他のレバーが連動するなら連動させる
            firstLever = true;
            _SyncLever?.SyncLeverPos(this.barPosition);
        }

        //バーの傾きを調整
        Vector3 barAngle = barObject.transform.localEulerAngles;
        float angleDifference = barAngleList[barPosition] - barAngle.z;

        //十分傾いてたら傾ける操作をしない
        if(Mathf.Abs(angleDifference) > 0.1f)
        {
            if(angleDifference > 0)barAngle.z += 10;
            if(angleDifference < 0)barAngle.z -= 10;
            barObject.transform.localEulerAngles = barAngle;
        }
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

    public void SyncLeverPos(int barPos)
    {
        if(firstLever)
        {
            this.firstLever = false;
            return;
        }

        //
        this.barPosition = barPos;
        _SyncLever.SyncLeverPos(barPos);
    }

}
