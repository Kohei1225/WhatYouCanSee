using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolume : MonoBehaviour
{
    public Slider BGMVolumeSlider;
    public Slider SEVolumeSlider;
    public static float BGMVolume;
    public static float SEVolume;
    public GameObject pauseObject;
    public GameObject optionObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (optionObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                optionObject.SetActive(false);
            }
        }

        BGMVolume = BGMVolumeSlider.value;
        SEVolume = SEVolumeSlider.value;
        Debug.Log("BGMVolume:" + BGMVolume);
        Debug.Log("SEVolume:" + SEVolume);
    }

    public void MoveClick()
    {
        optionObject.SetActive(false);

        pauseObject.SetActive(true);
    }
}
