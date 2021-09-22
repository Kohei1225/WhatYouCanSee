using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchByButton : MonoBehaviour
{
    public GameObject switchObject;
    public GameObject buttonObject;
    ButtonSwitchScript buttonScript;

    // Start is called before the first frame update
    void Start()
    {
        buttonScript = buttonObject.GetComponent<ButtonSwitchScript>();
    }

    // Update is called once per frame
    void Update()
    {
        switchObject.SetActive(buttonScript.isPushed);
    }
}
