using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WorldMove()
    {
        //�}�b�v�}�l�[�W���[�̏�Ԃ𕁒ʂɂ���
        MapManager.screenStatus = MapManager.ScreenStatuses.NORMAL;
        SceneManager.LoadScene(sceneName);
    }
}