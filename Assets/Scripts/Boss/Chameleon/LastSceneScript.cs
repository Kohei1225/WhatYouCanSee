using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastSceneScript : MonoBehaviour
{
    [SerializeField] private GameObject _Chameleon;
    [SerializeField] private GameObject[] _Objs;
    [SerializeField] private float[] _LastPosesY;
    private float[] _FirstPosesY;
    private float _Time = 0;
    private ChameleonScript _ChameleonScript;
    [SerializeField] private float _MoveSec = 2;
    private bool _CanMove = false;
    //死んでから何秒後に動くか
    [SerializeField] private float _WaitTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        _ChameleonScript = _Chameleon?.GetComponent<ChameleonScript>();
        _FirstPosesY = new float[_Objs.Length];
        if(_LastPosesY.Length != _Objs.Length)
        {
            Debug.LogError("サイズが" + _Objs.Length + "ではないよ");
        }
        for(int i = 0; i < _Objs.Length; i++)
        {
            //初期位置記憶
            _FirstPosesY[i] = _Objs[i].transform.position.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_ChameleonScript != null)
        {
            if (_ChameleonScript.IsDead)
            {
                _Time += Time.deltaTime;
                if (!_CanMove)
                {
                    if (_Time >= _WaitTime)
                    {
                        _CanMove = true;
                        _Time = 0;
                    }
                }
                else
                {
                    for (int i = 0; i < _Objs.Length; i++)
                    {
                        float t = _Time / _MoveSec;
                        //移動
                        Vector3 pos = _Objs[i].transform.position;
                        pos.y = Mathf.Lerp(_FirstPosesY[i], _LastPosesY[i], t);
                        _Objs[i].transform.position = pos;
                    }
                }
            }
        }
    }
}
