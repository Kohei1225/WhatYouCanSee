using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public GameObject textPanel;
    //playerが触れたらtrue
    private bool isToutch = false;
    private float time = 0;
    public GameObject player = null;
    //最小サイズ
    private float minScale;
    //何分の1のサイズになるか
    public float denominatorNum = 3;
    //縮小スピード
    private float smallSpeed = 0.1f;
    //すぐにステージ遷移ができるbool
    private bool canGo = false;
    //暗くなるbool
    private bool canDark = false;
    //ワープに吸い込まれるスピード
    public float suckedSpeed = 0.1f;
    //ワープの周りを回る角度のはやさ
    public float rotateAroundSpeed = 200f;
    //プレイヤー自身の回転の速さ
    public float rotateSpeed = 200f;
    //プレイヤーの現在の角度
    private float angle;
    //吸い込まれるフラグ
    private bool canSuck = false;
    //戻るシーン名
    public string worldSceneName;
    //オーディオソース
    private AudioSource audioSource;
    //音素材
    public AudioClip[] audioClip;

    public MaskManager maskManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        minScale = player.transform.localScale.x / denominatorNum;
    }

    private void Update()
    {
        if (canSuck && !canDark)
        {
            suckAnimation();
        }
        else if (canDark)
        {
            //マスクが無くなったら
            if (!maskManager.Get_canMove())
            {
                canDark = false;
                canGo = true;
            }
        }
        else if(canGo)
        {
            time += Time.deltaTime;
            if (time >= 2) {
                //ステージクリアフラグオン
                MapManager.screenStatus = MapManager.ScreenStatuses.CLEAR;
                //ステージセレクト画面へ
                SceneManager.LoadScene(worldSceneName);
            }

        }
    }

    private void suckAnimation()
    {
        //playerの現在地
        Vector3 from = player.transform.position;
        //ワープの位置
        Vector3 to = transform.position;
        //方向ベクトルを作成
        Vector3 direction = (to - from).normalized;
        //fromからtoまでの距離がspeed以下だったら
        if ((to - from).magnitude <= suckedSpeed * Time.deltaTime)
        {
            //playerをワープ位置まで移動
            player.transform.position = transform.position;
            canDark = true;
            canSuck = false;
            //見えなくする
            player.GetComponent<SpriteRenderer>().enabled = false;
            //円マスクが小さくなる
            //スピードを1/2にする
            maskManager.Set_speed(maskManager.Get_speed() / 2f);
            maskManager.Set_isShrink(true);
            maskManager.Set_canMove(true);

            //音ならす
            audioSource.PlayOneShot(audioClip[1]);
            return;
        }
        //ワープに向かって少し移動
        player.transform.Translate(direction * Time.deltaTime * suckedSpeed, Space.World);

        ////
        Vector3 rotateCenterPos = transform.position;
        Quaternion angleAxis_P = Quaternion.AngleAxis(rotateAroundSpeed * Time.deltaTime, Vector3.forward);
        Vector3 pos = player.transform.position;
        pos -= rotateCenterPos;
        pos = angleAxis_P * pos;
        pos += rotateCenterPos;
        //プレイヤーの位置更新
        player.transform.position = pos;

        //プレイヤー自身の回転
        Quaternion angleAxis_R = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, Vector3.forward);
        Quaternion rotation = player.transform.rotation;
        rotation = angleAxis_R * rotation;
        //プレイヤーの回転更新
        player.transform.rotation = rotation;
        //player.transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.forward);

        //プレイヤーの縮小
        player.transform.localScale = new Vector3();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.gameObject.name == "Player")
        {
            //player = collision.transform.parent.gameObject;
            angle = player.transform.rotation.z;
            //プレイヤー操作不能
            player.GetComponent<PlayerController>().Set_canCtrl(false);
            //無重力に
            Rigidbody2D playerRB = player.GetComponent<Rigidbody2D>();
            playerRB.velocity = Vector2.zero;
            playerRB.isKinematic = true;

            CameraController camCtrl = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
            camCtrl.enabled = true;
            textPanel.SetActive(true);
            textPanel.GetComponent<Text>().text = "Game Clear";

            canSuck = true;
            //音ならす
            audioSource.PlayOneShot(audioClip[0]);
        }
    }

    public void Set_canSuck(bool canSuck)
    {
        this.canSuck = canSuck;
    }
}
