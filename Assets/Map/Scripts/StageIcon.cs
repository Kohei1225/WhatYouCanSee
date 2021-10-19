using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIcon : MonoBehaviour
{
    //ステージに対応するシーンの名前
    [SerializeField] private string sceneName;
    //ステージ名
    [SerializeField] private string stageName;
    //クリアしているか
    [SerializeField] private bool isClear = true;

    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        DesideColor();
    }

    public string GetSceneName()
    {
        return sceneName;
    }

    public string GetStageName()
    {
        return stageName;
    }

    public Vector2 GetPosision()
    {
        return new Vector2(transform.position.x, transform.position.y);
    }

    public void Clear()
    {
        isClear = true;
        DesideColor();
    }

    public bool GetIsClear()
    {
        return isClear;
    }

    //クリアしてるかで色をセット
    private void DesideColor()
    {
        if (isClear)
        {
            sr.color = Color.blue;
        }
        else
        {
            sr.color = Color.red;
        }
    }
}
