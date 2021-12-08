using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseObject;
    public GameObject optionObject;
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
            //マップマネージャーの状態がNORMALだったら
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseObject.SetActive(true);
                //マップマネージャーの状態をPAUSEに
                MapManager.screenStatus = MapManager.ScreenStatuses.PAUSE;
                //時間を止める
                Time.timeScale = 0;
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
}
