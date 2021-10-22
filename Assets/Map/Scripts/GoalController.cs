using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    public GameObject textPanel;
    private bool isFin = false;
    private float time = 0;
    private GameObject player;

    private void Update()
    {
        if (isFin)
        {
            time += Time.deltaTime;
            if(time >= 2)
            {
                //ステージセレクト画面へ
                SceneManager.LoadScene("StageSelect");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject player = collision.gameObject;
            player.SetActive(false);

            Debug.Log("clear");
            textPanel.SetActive(true);
            textPanel.GetComponent<Text>().text = "Game Clear";
            isFin = true;
        }
    }
}
