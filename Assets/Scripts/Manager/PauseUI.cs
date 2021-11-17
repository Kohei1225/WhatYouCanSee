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
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseObject.SetActive(true);
            }
        }
        
    }

    public void RestartClick()
    {
        pauseObject.SetActive(false);
    }

    public void OptionClick()
    {
        pauseObject.SetActive(false);

        optionObject.SetActive(true);
    }

    public void WorldMove()
    {
        SceneManager.LoadScene(sceneName);
    }
}
