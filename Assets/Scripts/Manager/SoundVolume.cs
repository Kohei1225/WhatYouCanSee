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
    //初期のオーディオミキサーの音量
    public float firstVolume = -10;
    //オーディオミキサーの音量からスライダーの値にこれをかけて変換する
    private float volumeToValue;
    //ミュートか覚えるためのbool
    public static bool isMute_BGM;
    public static bool isMute_SE;

    // Start is called before the first frame update

    private void Awake()
    {
        audioMixer = Resources.Load<AudioMixer>("Audios/AudioMixer");

        audioMixer.SetFloat("BGMVolume", -10);
        audioMixer.SetFloat("SEVolume", -10);
    }

    void Start()
    {

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

        //開始時にトグルの値をオーディオミキサーから読み取る
        SoundManager.Instance.SetMuteBGM(isMute_BGM);
        SoundManager.Instance.SetMuteSE(isMute_SE);
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
        audioMixer.SetFloat("BGMVolume", bgmVolume / volumeToValue - firstVolume);
        //Debug.Log("BGM:" + bgmVolume);
    }

    public void Set_seVolume(float seVolume)
    {
        audioMixer.SetFloat("SEVolume", seVolume / volumeToValue - firstVolume);
        //Debug.Log("SE:" + seVolume);
    }

    public void Set_masterVolume(float masterVolume)
    {
        audioMixer.SetFloat("MasterVolume", masterVolume / volumeToValue);
        //Debug.Log("Master:" + masterVolume);
    }

    /// <summary>
    /// トグルで呼び出す
    /// </summary>
    /// <param name="isMute"></param>
    public void MuteBGM(bool isMute)
    {
        isMute_BGM = isMute;
        SoundManager.Instance.SetMuteBGM(isMute);
    }

    /// <summary>
    /// トグルで呼び出す
    /// </summary>
    /// <param name="isMute"></param>
    public void MuteSE(bool isMute)
    {
        isMute_SE = isMute;
        SoundManager.Instance.SetMuteSE(isMute);
    }
}
