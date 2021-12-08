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
    //private float volumeToValue;
    //ミュートか覚えるためのbool
    public static bool isMute_BGM;
    public static bool isMute_SE;

    private static bool isFirst = true;

    // Start is called before the first frame update

    private void Awake()
    {
    }

    void Start()
    {
        audioMixer = Resources.Load<AudioMixer>("Audios/AudioMixer");

        if (isFirst)
        {
            //音量設定
            audioMixer.SetFloat("BGMVolume", firstVolume);
            audioMixer.SetFloat("SEVolume", firstVolume);
            isFirst = false;
        }

        //volumeToValue = 1 / (deltaVolume / 2);
        //開始時にスライダーの値をオーディオミキサーから読み取る
        float bgmVolume;
        audioMixer.GetFloat("BGMVolume", out bgmVolume);
        //Debug.Log("BGM:" + bgmVolume);
        BGMVolumeSlider.minValue = 0;
        BGMVolumeSlider.maxValue = 1;

        BGMVolumeSlider.value = Mathf.InverseLerp(firstVolume - (deltaVolume / 2), firstVolume + (deltaVolume / 2), bgmVolume);

        float seVolume;
        audioMixer.GetFloat("SEVolume", out seVolume);
        //Debug.Log("SE:" + seVolume);
        SEVolumeSlider.minValue = 0;
        SEVolumeSlider.maxValue = 1;

        SEVolumeSlider.value = Mathf.InverseLerp(firstVolume - (deltaVolume / 2), firstVolume + (deltaVolume / 2), seVolume);

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

        float bgmVolume;
        audioMixer.GetFloat("BGMVolume", out bgmVolume);

        //BGMVolume = BGMVolumeSlider.value;
        //SEVolume = SEVolumeSlider.value;
        //Debug.Log("BGMVolume:" + BGMVolume);
        //Debug.Log("SEVolume:" + SEVolume);
    }

    //以下は音量調節
    public void Set_bgmVolume(float bgmValue)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Lerp(firstVolume - (deltaVolume / 2), firstVolume + (deltaVolume / 2), bgmValue));
        //Debug.Log("BGM:" + bgmVolume);
    }

    public void Set_seVolume(float seValue)
    {
        audioMixer.SetFloat("SEVolume", Mathf.Lerp(firstVolume - (deltaVolume / 2), firstVolume + (deltaVolume / 2), seValue));
        //Debug.Log("SE:" + seVolume);
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
