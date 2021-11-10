using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{
    protected float alpha;
    public float speed = 0.01f;
    //濃くなるか(alphaが増えるか)
    public bool isFadeIn;
    private int fadeVec;
    private bool isFin = true;
    // Start is called before the first frame update
    private void Start()
    {
        Initializing();
    }

    private void Initializing()
    {
        if (isFadeIn)
        {
            alpha = 0;
            fadeVec = 1;
        }
        else
        {
            alpha = 1;
            fadeVec = -1;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isFin)
        {
            float newAlpha = alpha + fadeVec * speed * Time.deltaTime;
            if (newAlpha >= 1 || newAlpha <= 0)
            {
                isFin = true;
            }
            alpha = Mathf.Lerp(0, 1, newAlpha);
        }
    }

    //boolを変えてスタート
    public void Restart(bool isFadeIn)
    {
        this.isFadeIn = isFadeIn;
        isFin = false;
        Initializing();
    }

    public bool Get_isFin()
    {
        return isFin;
    }

    public float Get_alpha()
    {
        return alpha;
    }
}
