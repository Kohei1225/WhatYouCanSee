using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour
{
    //public ChameleonScript _ChameleonScript;
    private bool _IsFin = false;
    private float _Time = 0;
    private bool _CanMove = false;
    //private TimerScript _TimerScript = new TimerScript();
    public ChameleonScript _ChameleonScript;
    //雷が落ち続ける時間
    [SerializeField] private float _LightningTime = 3;
    //生成するブロックの数
    [SerializeField] private int _MakeLightningBlockNum = 3;
    //雷が落ちる時間
    [SerializeField] private float _LightningInterval = 15;
    /// <summary> 雷用のライト
    public GameObject _LightningLight;
    /// <summary> 雷のときの背景の色
    public Color _LightingBackColor;
    /// <summary> ブロックのプレハブ
    public GameObject _LightningBlockPrefab;
    private GameObject[] _Blocks;
    //[SerializeField] private float _BlockPosY;
    /// <summary>
    /// 左と右の限界の位置
    /// </summary>
    public GameObject _LeftPosObj,_RightPosObj;

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
        //最初は見えない
        _LightningLight.SetActive(false);
        ////時間をセット
        //_TimerScript.ResetTimer(_LightningInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsFin)
            return;
        _Time += Time.deltaTime;
        if (!_CanMove && _Time >= _LightningInterval)
        {
            //光らせる
            StartLightning();
            //怯える
            _ChameleonScript.Scared();
            //ブロック生成(数の分だけ)
            _Blocks = new GameObject[_MakeLightningBlockNum];
            for (int i = 0; i < _MakeLightningBlockNum; i++)
            {
                float t = Random.Range(0f, 1f);
                float x = Mathf.Lerp(_LeftPosObj.transform.position.x, _RightPosObj.transform.position.x, t);
                //ブロック生成
                _Blocks[i] = Instantiate(_LightningBlockPrefab, new Vector3(x, _LeftPosObj.transform.position.y, 0), Quaternion.identity);
                //レイヤー強制変更
                _Blocks[i].layer = LayerMask.NameToLayer("Real");
            }
        }
        else if (_CanMove && _Time > _LightningTime)
        {
            _IsFin = true;
            _CanMove = false;
            //見えなくする
            _LightningLight.SetActive(false);
            //ブロック消去
            foreach(GameObject block in _Blocks)
            {
                Destroy(block);
            }
            _Blocks = null;
        }
    }

    //雷を起こす
    public void StartLightning()
    {
        _IsFin = false;
        _CanMove = true;
        _Time = 0;
        //見えるように
        _LightningLight.SetActive(true);
    }

    public void ResetLightning()
    {
        _IsFin = false;
        _CanMove = false;
        //見えなくする
        _LightningLight.SetActive(false);
        _Time = 0;
        //ブロック消去
        if(_Blocks != null)
        {
            foreach (GameObject block in _Blocks)
            {
                Destroy(block);
            }
            _Blocks = null;
        }
    }
}
