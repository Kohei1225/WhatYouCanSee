using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    //public ChameleonScript _ChameleonScript;
    private bool _IsFin = false;
    private float _Time = 0;
    //private bool _CanMove = false;
    //private TimerScript _TimerScript = new TimerScript();
    public ChameleonScript _ChameleonScript;
    public enum StateEnum
    {
        NONE,
        PREPARE,
        CLOUD,
        DARKER,
        LIGHTNING
    }
    [SerializeField] private StateEnum _State = StateEnum.NONE;
    //雲が出てくる時間(フェーズごと)
    [SerializeField] private float[] _StartingIntervals = {5, 5, 30 };
    //暗くなって落ちるまでの時間落ちるまでの時間
    [SerializeField] private float _WaitTime = 2;
    //雷が落ち続ける時間
    [SerializeField] private float _LightningTime = 3;
    //生成するブロックの数
    [SerializeField] private int _MakeLightningBlockNum = 3;
    /// <summary> 雷用のライト
    public GameObject _LightningLight;
    /// <summary> 雷のときの背景の色
    public Color _LightingBackColor;
    /// <summary> ブロックのプレハブ
    [SerializeField] public GameObject _LightningBlockPrefab;
    /// <summary> ブロックを生成するか
    private bool _CanMake;
    private GameObject[] _Blocks;
    //[SerializeField] private float _BlockPosY;
    //ブロック生成可能な端posX(ボスの戻る位置も)
    [SerializeField] private float _SidePosX = 40;
    [SerializeField] private float _SidePosY = 17;
    //雲のスクリプト
    private CloudScript _CloudScript;
    //ゲームマネージャー
    [SerializeField] private GameManagerScript _GameManagerScript;
    //背景のカラースクリプト
    [SerializeField] private ColorObjectVer3 _BackGroundColorObjectVer3;

    public bool IsFin
    {
        get
        {
            return _IsFin;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _CloudScript = GetComponent<CloudScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsFin)
            return;
        _Time += Time.deltaTime;
        switch (_State)
        {
            case StateEnum.NONE:
                break;
            case StateEnum.PREPARE:
                if (_Time >= _StartingIntervals[_ChameleonScript.Phase])
                {
                    _CloudScript.StartCloud();
                    _State = StateEnum.CLOUD;
                }
                break;
            case StateEnum.CLOUD:
                if (_CloudScript.IsFin)
                {
                    _State = StateEnum.DARKER;
                    _Time = 0;
                }
                break;
            case StateEnum.DARKER:
                //暗くさせる
                _GameManagerScript.existRay = true;
                if (_Time >= _WaitTime)
                {
                    _State = StateEnum.LIGHTNING;
                    _Time = 0;
                    //ゴロゴロ
                    //背景を黄色に
                    _BackGroundColorObjectVer3.colorType = ColorObjectVer3.OBJECT_COLOR3.DARK_YELLOW;
                    //怯える
                    _ChameleonScript.Scared();
                    if (_CanMake)
                    {
                        //ブロック生成(数の分だけ)
                        _Blocks = new GameObject[_MakeLightningBlockNum];
                        for (int i = 0; i < _MakeLightningBlockNum; i++)
                        {
                            float t = Random.Range(0f, 1f);
                            float x = Mathf.Lerp(-_SidePosX, _SidePosX, t);
                            //ブロック生成
                            _Blocks[i] = Instantiate(_LightningBlockPrefab, new Vector3(x, _SidePosY, 0), Quaternion.identity);
                            //レイヤー強制変更
                            _Blocks[i].layer = LayerMask.NameToLayer("Real");
                        }
                    }
                }
                break;
            case StateEnum.LIGHTNING:
                if(_Time >= _LightningTime)
                {
                    //終わる
                    FinishLightning();
                }
                break;

        }
    }

    //雷を起こす準備をする(ブロックを生成する？)
    public void PrepareLightning(bool canMake)
    {
        _CanMake = canMake;
        _IsFin = false;
        _Time = 0;
        _State = StateEnum.PREPARE;
    }

    public void FinishLightning()
    {
        _IsFin = true;
        //終わり
        _State = StateEnum.NONE;
        //ブロック消去
        if (_Blocks != null)
        {
            foreach (GameObject block in _Blocks)
            {
                Destroy(block);
            }
            _Blocks = null;
        }
        //雲消去
        _CloudScript.RemoveCloud();
        //明るくする
        _GameManagerScript.existRay = false;
    }
}
