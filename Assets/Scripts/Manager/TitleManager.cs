using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public string sceneName;
    private GameObject _ContinueButton;

    // Start is called before the first frame update
    void Start()
    {
        _ContinueButton = transform.Find("ContinueButton").gameObject;
        if (!PlayerPrefs.HasKey("LastGoNo"))
        {
            _ContinueButton.GetComponent<Button>().interactable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetClick()
    {
        MapManager.lastGoNo = 0;
        SceneManager.LoadScene(sceneName);
        Debug.Log("????");
    }

    public void ContinueClick()
    {
        MapManager.lastGoNo = PlayerPrefs.GetInt("LastGoNo");
    }
}
