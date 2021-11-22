using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public GameObject pauseObject;
    public GameObject optionObject;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pauseObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseObject.SetActive(false);
                //マップマネージャーの状態をNORMALに
                MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
            }
        }
        else
        {
            //マップマネージャーの状態がNORMALだったら
            if (Input.GetKeyDown(KeyCode.Escape) && MapManager.screenStatus == MapManager.ScreenStatuses.NORMAL)
            {
                pauseObject.SetActive(true);
                //マップマネージャーの状態をPAUSEに
                MapManager.screenStatus = MapManager.ScreenStatuses.PAUSE;
            }
        }
        
    }

    public void RestartClick()
    {
        pauseObject.SetActive(false);
        //マップマネージャーの状態をNORMALに
        MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
    }

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
    }
}
