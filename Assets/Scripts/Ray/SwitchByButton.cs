using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchByButton : MonoBehaviour
{
    public GameObject switchObject;
    public GameObject buttonObject;
    ButtonSwitchScript buttonScript;

    void Awake()
    {
        buttonScript = buttonObject.GetComponent<ButtonSwitchScript>();
        switchObject.SetActive(buttonScript.isPushed);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switchObject.SetActive(buttonScript.isPushed);
    }
}
