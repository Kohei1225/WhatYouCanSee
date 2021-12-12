using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseObject;
    public GameObject optionObject;
    public GameObject saveObject;
    public string sceneName;
    [SerializeField] private MapManager.ScreenStatuses _Status;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _Status = MapManager.screenStatus;
        //セーブしました画面がある時はreturn
        if (saveObject)
        {
            if (saveObject.activeSelf)
                return;
        }
        if (pauseObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseObject.SetActive(false);
                //マップマネージャーの状態をNORMALに
                MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
                //時間を動かす
                Time.timeScale = 1;
            }
        }
        else
        {
            //マップマネージャーの状態がNORMALかPauseだったら
            if (MapManager.screenStatus == MapManager.ScreenStatuses.NORMAL || MapManager.screenStatus == MapManager.ScreenStatuses.PAUSE)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PlayerController playerController = GameObject.Find("Player")?.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        if (playerController.damage)
                            return;
                    }
                    pauseObject.SetActive(true);
                    //マップマネージャーの状態をPAUSEに
                    MapManager.screenStatus = MapManager.ScreenStatuses.PAUSE;
                    //時間を止める
                    Time.timeScale = 0;
                }
            }
        }
        
    }

    //ポーズを閉じる
    public void RestartClick()
    {
        pauseObject.SetActive(false);
        //マップマネージャーの状態をNORMALに
        MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
        //時間を動かす
        Time.timeScale = 1;
    }

    //ポーズからオプションへ
    public void OptionClick()
    {
        pauseObject.SetActive(false);

        optionObject.SetActive(true);
    }

    public void WorldMove()
    {
        //マップマネージャーの状態を普通にする
        MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
        SceneManager.LoadScene(sceneName);
        //時間を動かす
        Time.timeScale = 1;
    }

    //オプションからポーズへ
    public void MoveClick()
    {
        optionObject.SetActive(false);

        pauseObject.SetActive(true);
    }

    public void SaveClick()
    {
        PlayerPrefs.SetInt("LastGoNo", MapManager.lastGoNo);
        PlayerPrefs.SetInt("NowNo", PlayerController_Map.nowNo);
        PlayerPrefs.SetInt("GoWorldNo", CameraController_Map.goWorldNo);
        Debug.Log("セーブしたよ");
        PlayerPrefs.Save();
        //セーブしましたを表示
        saveObject.SetActive(true);
        //ポーズ非表示
        pauseObject.SetActive(false);
    }

    //再読み込み
    public void ResetStageClick()
    {
        //マップマネージャーの状態を普通にする
        MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
        //時間を動かす
        Time.timeScale = 1;
        //同じシーンを読み込む
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //セーブしましたを閉じるボタン
    public void CloseSaveClick()
    {
        //セーブしましたを非表示
        saveObject.SetActive(false);
        //ポーズ表示
        pauseObject.SetActive(true);
    }
}
