using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Map : MonoBehaviour
{
    private List<Vector3> clearedPoints;
    public MapManager mapManager;
    private int goNo = 0;
    private bool canMove = false;
    public float speed = 15f;
    private float beforeKeyX = 0;
    //ステージポイントからプレイヤーをY軸方向にどのくらいずらすか
    public float shiftY = 1;
    //キーが押せるか
    private bool canPush = true;

    private Animator anim;
    private float firstLocalScaleX;

    // Start is called before the first frame update
    void Start()
    {
        if (clearedPoints == null)
        {
            Debug.Log("clearedPointsがnull");
        }

        transform.position = clearedPoints[goNo] + Vector3.up * shiftY;

        anim = GetComponent<Animator>();
        firstLocalScaleX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        //キーチェック
        KeyCheck();
        //移動
        MovePlayer();
    }

    private void KeyCheck()
    {
        if (!canPush)
            return;

        float keyX = Input.GetAxisRaw("Horizontal");

        if (keyX == 0)
            return;

        if (canMove && keyX != beforeKeyX || !canMove)
        {

            int newGoNo = goNo + (int)keyX;
            //もし移動先が範囲外だったら
            if (newGoNo < 0 || newGoNo > clearedPoints.Count - 1)
                return;

            //Debug.Log("beforeKeyX :" + beforeKeyX + "keyX :" + keyX);

            //移動ポイント更新
            goNo = newGoNo;
            //動けるようにする
            canMove = true;
            anim.SetBool("Walk", canMove);
            //向きを定める
            transform.localScale = new Vector3(firstLocalScaleX * keyX, transform.localScale.y, transform.localScale.z);
        }

        //前回の押したキーを記憶
        beforeKeyX = keyX;
    }

    private void MovePlayer()
    {
        if (!canMove)
            return;

        Vector3 from = transform.position;
        //次にいくポイント + 調整が目的地
        Vector3 to = clearedPoints[goNo] + Vector3.up * shiftY;
        //もし次の移動でステージポイントからはみ出そう、またはぴったりなら
        if(GetLength(from, to) <= speed * Time.deltaTime)
        {
            //調整
            transform.position = to;
            //動けなくする
            canMove = false;
            anim.SetBool("Walk", canMove);
            return;
        }
        Vector3 direction = GetVector(from, to);
        //移動
        transform.Translate(direction * speed * Time.deltaTime);
    }

    //fromからtoまでの単位ベクトルを返す
    private Vector3 GetVector(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    //fromからtoまでの長さを返す
    private float GetLength(Vector3 from, Vector3 to)
    {
        return (to - from).magnitude;
    }

    public void SetClearedPoints(List<Vector3> clearedPoints)
    {
        this.clearedPoints = clearedPoints;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public int GetGoNo()
    {
        return goNo;
    }

    public void SetCanPush(bool canPush)
    {
        this.canPush = canPush;
    }
}
