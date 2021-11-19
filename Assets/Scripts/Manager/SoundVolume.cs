using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundVolume : MonoBehaviour
{
    public Slider BGMVolumeSlider;
    public Slider SEVolumeSlider;
    //public static float BGMVolume;
    //public static float SEVolume;
    public GameObject pauseObject;
    public GameObject optionObject;
    //オーディオミキサー
    private AudioMixer audioMixer;
    //デシベルの最小値と最大値の差
    //スライダーの最小値は-1、最大値は1
    public float deltaVolume = 20;
    //オーディオミキサーの音量からスライダーの値にこれをかけて変換する
    private float volumeToValue;

    // Start is called before the first frame update

    void Start()
    {
        audioMixer = Resources.Load<AudioMixer>("Audios/AudioMixer");

        volumeToValue = 1 / (deltaVolume / 2);
        //開始時にスライダーの値をオーディオミキサーから読み取る
        float bgmVolume;
        audioMixer.GetFloat("BGMVolume", out bgmVolume);
        BGMVolumeSlider.value = bgmVolume * volumeToValue;

        float seVolume;
        audioMixer.GetFloat("SEVolume", out seVolume);
        SEVolumeSlider.value = seVolume * volumeToValue;

        //float masterVolume;
        //audioMixer.GetFloat("SEVolume", out masterVolume);
        //MasterVolumeSlider.value = masterVolume;
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

        //BGMVolume = BGMVolumeSlider.value;
        //SEVolume = SEVolumeSlider.value;
        //Debug.Log("BGMVolume:" + BGMVolume);
        //Debug.Log("SEVolume:" + SEVolume);
    }

    public void MoveClick()
    {
        optionObject.SetActive(false);

        pauseObject.SetActive(true);
    }

    //以下は音量調節
    public void Set_bgmVolume(float bgmVolume)
    {
        audioMixer.SetFloat("BGMVolume", bgmVolume / volumeToValue);
        //Debug.Log("BGM:" + bgmVolume);
    }

    public void Set_seVolume(float seVolume)
    {
        audioMixer.SetFloat("SEVolume", seVolume / volumeToValue);
        //Debug.Log("SE:" + seVolume);
    }

    public void Set_masterVolume(float masterVolume)
    {
        audioMixer.SetFloat("MasterVolume", masterVolume / volumeToValue);
        //Debug.Log("Master:" + masterVolume);
    }
}
