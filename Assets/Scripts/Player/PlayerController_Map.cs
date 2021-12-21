using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController_Map : MonoBehaviour
{
    private List<Vector3> clearedPoints;
    public MapManager mapManager;
    public static int nowNo;
    private int goNo = nowNo;
    private bool canMove = false;
    public float speed = 15f;
    private float beforeKeyX = 0;
    //ステージポイントからプレイヤーをY軸方向にどのくらいずらすか
    public float shiftY = 1;
    ////キーが押せるか
    //private bool canPush = true;

    private Animator anim;
    private float firstLocalScaleX;
    //ジャンプの高さ
    public float jumpHeight = 10;
    //今ジャンプ中か
    private bool isJump = false;
    //ジャンプの速度
    public float jumpVY = 0.25f;

    public CameraController_Map cameraController_map;

    public GameObject stagePanel;

    public TextMeshProUGUI worldName_text;
    public WorldSignboardScript worldSignboardScript;

    //ステージの画面を見せるもの
    [SerializeField] private Image _StageImage;

    //ステージ選択方法のTextMeshPro
    [SerializeField] private TextMeshProUGUI _HowToText;


    // Start is called before the first frame update
    void Start()
    {

        if (clearedPoints == null)
        {
            Debug.Log("clearedPointsがnull");
        }

        transform.position = clearedPoints[nowNo] + Vector3.up * shiftY;

        anim = GetComponent<Animator>();
        firstLocalScaleX = transform.localScale.x;

        //hereIconを見えるようにする
        GameObject hereIcon = mapManager.getStageIcon(nowNo).transform.GetChild(0).gameObject;
        hereIcon.SetActive(true);
        //ステージパネルを下げている状態にする
        stagePanel.SetActive(true);
        //看板の名前を更新
        int worldNo = (int)mapManager.getStageIcon(nowNo).GetComponent<StageIcon>().worldNo;
        string worldName = mapManager.worldName[worldNo];
        //ワールド名変更
        worldName_text.text = worldName;
        //ステージ画像を取り替え
        Sprite sprite = Resources.Load<Sprite>("Sprites/Worlds/" + mapManager.getStageIcon(goNo).GetComponent<StageIcon>().GetStageName());
        if (sprite == null)
            Debug.Log("ステージ画像が見つからない");
        else
        {
            _StageImage.sprite = sprite;
        }
        //アイコンにあるワールドから流す曲を決める
        mapManager.PlayWorldBGM(worldNo);
    }

    // Update is called once per frame
    void Update()
    {
        //キーチェック
        KeyCheck();
        //移動
        MovePlayer();

        if (isJump)
        {
            if (transform.position.y - clearedPoints[goNo].y + shiftY < jumpHeight)
            {
                transform.Translate(Vector3.up * jumpVY);
            }
            //ジャンプが終わったら
            else
            {
                isJump = false;
            }
        }

        //clearedPoints = mapManager.GetClearedPoints();
    }

    private void KeyCheck()
    {
        //if (!canPush)
        //    return;

        float keyX = Input.GetAxisRaw("Horizontal");

        if (keyX == 0)
            return;

        if (MapManager.screenStatus != MapManager.ScreenStatuses.NORMAL)
            return;

        if (canMove && keyX != beforeKeyX || !canMove)
        {

            int newGoNo = goNo + (int)keyX;
            //もし移動先が範囲外だったら
            if (newGoNo < 0 || newGoNo > clearedPoints.Count - 1)
                return;

            //Debug.Log("beforeKeyX :" + beforeKeyX + "keyX :" + keyX);

            //動けるようにする
            canMove = true;
            anim.SetBool("Walk", canMove);
            //hereIconを見えないようにする
            GameObject hereIcon = mapManager.getStageIcon(goNo).transform.GetChild(0).gameObject;
            hereIcon.SetActive(false);
            //ステージパネルを見えないようにする
            stagePanel.SetActive(false);
            //操作方法見えないようにする
            _HowToText.enabled = false;
            //元の位置記憶
            nowNo = goNo;
            //移動ポイント更新
            goNo = newGoNo;

            //向きを定める
            transform.localScale = new Vector3(firstLocalScaleX * keyX, transform.localScale.y, transform.localScale.z);


            int goWorldNo = (int)mapManager.getStageIcon(goNo).GetComponent<StageIcon>().worldNo;
            int nowWorldNo = (int)mapManager.getStageIcon(nowNo).GetComponent<StageIcon>().worldNo;
            //もし移動さきが新しいワールドだったら
            if (nowWorldNo != goWorldNo)
            {
                Vector3 from = transform.position;
                //次にいくポイント + 調整が目的地
                Vector3 to = clearedPoints[goNo] + Vector3.up * shiftY;

                cameraController_map.CuluculateRate(goWorldNo, to - from);

                //看板をしまう
                worldSignboardScript.Init(true);
                worldSignboardScript.Set_isMove(true);

            }
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
            //hereIconを見えるようにする
            GameObject hereIcon = mapManager.getStageIcon(goNo).transform.GetChild(0).gameObject;
            hereIcon.SetActive(true);
            //ステージパネルを見えるようにする
            stagePanel.SetActive(true);
            //操作方法見えるようにする
            _HowToText.enabled = true;
            //目的の場所は今いる場所に
            nowNo = goNo;

            //カメラ位置調整(カメラ移動の終わり)
            cameraController_map.TranslateGoPos();
            int worldNo = (int)mapManager.getStageIcon(goNo).GetComponent<StageIcon>().worldNo;
            string worldName = mapManager.worldName[worldNo];
            //ワールド名変更
            worldName_text.text = worldName;
            //看板を出す
            worldSignboardScript.Init(false);
            worldSignboardScript.Set_isMove(true);
            //ステージ画像を取り替え
            Sprite sprite = Resources.Load<Sprite>("Sprites/Worlds/" + mapManager.getStageIcon(goNo).GetComponent<StageIcon>().GetStageName());
            if (sprite == null)
                Debug.Log("ステージ画像が見つからない");
            else
            {
                _StageImage.sprite = sprite;
            }
            //アイコンにあるワールドから流す曲を決める
            mapManager.PlayWorldBGM(worldNo);
            return;
        }
        Vector3 direction = GetVector(from, to);
        //移動
        transform.Translate(direction * speed * Time.deltaTime);

        //カメラの移動
        cameraController_map.MoveCamera(direction * speed * Time.deltaTime);
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

    public void Jump()
    {
        anim.SetBool("Jump", true);
        isJump = true;
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

    //public void SetCanPush(bool canPush)
    //{
    //    this.canPush = canPush;
    //}

    //public bool GetCanPush()
    //{
    //    return canPush;
    //}

    public bool Get_isJump() {
        return isJump;
    }
    
}
