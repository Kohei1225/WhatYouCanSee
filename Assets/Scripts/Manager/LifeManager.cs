using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private PlayerController _PlayerController;
    [SerializeField] private GameObject _HeartPrefab;
    //ハートとハートの隙間の大きさ
    [SerializeField] private float _Blank = 5;
    private int _MaxLife;
    private GameObject[] _LifeObjs;
    private RectTransform _RectTransform;
    // Start is called before the first frame update
    void Start()
    {
        _RectTransform = GetComponent<RectTransform>();
        _MaxLife = _PlayerController.Life;
        float scaleX = _RectTransform.sizeDelta.x;
        //左端のX座標
        float leftX = _RectTransform.anchoredPosition.x;
        //プレハブ作成
        _LifeObjs = new GameObject[_MaxLife];
        for (int i = 0; i < _MaxLife; i++)
        {
            float x = leftX + (_Blank + scaleX) * i;
            float y = _RectTransform.anchoredPosition.y;
            _LifeObjs[i] = Instantiate(_HeartPrefab);
            _LifeObjs[i].transform.SetParent(transform.parent,false);
            _LifeObjs[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(x, y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int life = _PlayerController.Life;
        for (int i = 0; i < _MaxLife; i++)
        {
            _LifeObjs[i].SetActive(i < life);
        }
    }
}
