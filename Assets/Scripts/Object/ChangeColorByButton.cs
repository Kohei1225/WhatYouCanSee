using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorByButton : MonoBehaviour
{
    ButtonSwitchScript buttonScript;
    public GameObject buttonObject;
    ColorObjectScript colorScript;
    public ColorObjectScript.OBJECT_COLOR[] colorList;

    bool hasPushed;
    int colorNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        colorScript = gameObject.GetComponent<ColorObjectScript>();
        buttonScript = buttonObject.GetComponent<ButtonSwitchScript>();
        hasPushed = false;
    }

    // Update is called once per frame
    void Update()
    {

        if(buttonScript.isPushed)
        {
            if(!hasPushed)
            {
                colorScript.colorType = colorList[colorNum];
                colorNum++;
                if(colorNum == colorList.Length)colorNum = 0;
                hasPushed = true;
            }
        }
        else hasPushed = false;
    }
}
