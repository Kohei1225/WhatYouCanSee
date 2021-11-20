using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private AudioSource audioSource_BGM;
    private AudioSource audioSource_SE;
    //public AudioMixer audioMixer;

    public AudioMixerGroup audioMixerGroup_BGM;
    public AudioMixerGroup audioMixerGroup_SE;
    //public AudioMixerGroup audioMixerGroup_Master;

    private AudioClip[] audioClips_BGM;
    private AudioClip[] audioClips_SE;

    private Dictionary<string, int> keyValuePairs_BGM;
    private Dictionary<string, int> keyValuePairs_SE;

    //protected override void Awake()
    //{
    //    //DontDestroyOnLoad(gameObject);
    //    base.Awake();
    //}


    void Start()
    {
        audioSource_BGM = gameObject.AddComponent<AudioSource>();
        audioSource_SE = gameObject.AddComponent<AudioSource>();

        audioSource_BGM.playOnAwake = false;
        audioSource_SE.playOnAwake = false;

        //オーディオソース(BGM)のoutputにBGMをセット
        audioSource_BGM.outputAudioMixerGroup = audioMixerGroup_BGM;
        //オーディオソース(SE)のoutputにSEをセット
        audioSource_SE.outputAudioMixerGroup = audioMixerGroup_SE;

        keyValuePairs_BGM = new Dictionary<string, int>();
        keyValuePairs_SE = new Dictionary<string, int>();

        audioClips_BGM = Resources.LoadAll<AudioClip>("Audios/BGM");
        for (int i = 0; i < audioClips_BGM.Length; i++)
        {
            keyValuePairs_BGM.Add(audioClips_BGM[i].name, i);
        }

        audioClips_SE = Resources.LoadAll<AudioClip>("Audios/SE");
        for (int i = 0; i < audioClips_SE.Length; i++)
        {
            keyValuePairs_SE.Add(audioClips_SE[i].name, i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetIndex_BGM(string fileName)
    {
        if (keyValuePairs_BGM.ContainsKey(fileName))
        {
            return keyValuePairs_BGM[fileName];
        }
        else
        {
            Debug.LogError("ファイルが見つかりません");
            return 0;
        }
    }

    public int GetIndex_SE(string fileName)
    {
        if (keyValuePairs_SE.ContainsKey(fileName))
        {
            return keyValuePairs_SE[fileName];
        }
        else
        {
            Debug.LogError("ファイルが見つかりません");
            return 0;
        }
    }

    public void PlayBGM(string fileName)
    {
        //今の音と同じではないなら流す
        AudioClip newBGM = audioClips_BGM[GetIndex_BGM(fileName)];
        if (audioSource_BGM.clip != newBGM)
        {
            audioSource_BGM.clip = newBGM;
            audioSource_BGM.Play();
        }
    }

    public void StopBGM()
    {
        if(audioSource_BGM.clip != null)
        {
            audioSource_BGM.Stop();
            audioSource_BGM.clip = null;
        }
    }

    public void PlaySE(string fileName)
    {
        //流す
        AudioClip newSE = audioClips_SE[GetIndex_SE(fileName)];
        audioSource_SE.PlayOneShot(newSE);
    }
}
