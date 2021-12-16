using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeTextAlpha : MonoBehaviour
{
    private TextMeshProUGUI text;
    private ChangeAlpha changeAlpha;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        changeAlpha = GetComponent<ChangeAlpha>();
    }

    private void Update()
    {
        SetAlphaToThing();
    }

    //アルファ値を実際にいれて適用
    public void SetAlphaToThing()
    {
        Color color = text.color;
        text.color = new Color(color.r, color.g, color.b, changeAlpha.Get_alpha());
        //throw new System.NotImplementedException();
    }
}
