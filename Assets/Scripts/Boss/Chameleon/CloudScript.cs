using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour
{
    private bool _IsFin = false;
    private float _Time = 0;
    private bool _CanMove = false;
    //雲
    [SerializeField] GameObject _CloudPrefab;
    //雲が見えるまでの時間
    [SerializeField] private float _CanSeeSec = 1;
    //到着するまでの時間
    [SerializeField] private float _CloudSpeedSec = 2;
    //雲のX位置(右側)
    [SerializeField] private float _PosX = 50;
    //雲のy位置
    [SerializeField] private float _PosY = 20;
    private int _CloudNum = 4;
    [SerializeField] private AnimationCurve _AnimationCurve;
    //オブジェクトと初期位置、最終位置
    [SerializeField] private Dictionary<GameObject, Vector2> _KeyValuePairs;

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
        _KeyValuePairs = new Dictionary<GameObject, Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsFin || !_CanMove)
            return;
        _Time += Time.deltaTime;
        //CanSeeCloud();
        MoveCloud();
        if(_Time >= _CloudSpeedSec)
        {
            _IsFin = true;
            _CanMove = false;
        }
    }

    //動く
    private void MoveCloud()
    {
        foreach(KeyValuePair<GameObject, Vector2> pair in _KeyValuePairs)
        {
            GameObject cloud = pair.Key;
            Vector2 fromToPos = pair.Value;
            Vector3 pos = cloud.transform.position;
            Vector3 newPos = new Vector3(Mathf.Lerp(fromToPos.x, fromToPos.y, _AnimationCurve.Evaluate(_Time / _CloudSpeedSec)), pos.y, pos.z);
            cloud.transform.position = newPos;
        }
    }

    //見えるようになる
    private void CanSeeCloud()
    {
        foreach (KeyValuePair<GameObject, Vector2> pair in _KeyValuePairs)
        {
            GameObject cloud = pair.Key;
            SpriteRenderer spriteRenderer = cloud.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(0, 1, _Time / _CanSeeSec));
        }
    }

    public void StartCloud()
    {
        _IsFin = false;
        _CanMove = true;
        _Time = 0;
        //雲を作る
        float leftX = -_PosX;
        float rightX = _PosX;
        //左から右への雲作成
        for (int i = 0; i < _CloudNum / 2; i++)
        {
            GameObject cloud = Instantiate(_CloudPrefab, new Vector3(leftX, _PosY, 0), Quaternion.identity);
            Vector2 fromToPosX = new Vector2(leftX, leftX + (rightX - leftX) * (i + 1) / (_CloudNum + 1));
            _KeyValuePairs.Add(cloud, fromToPosX);
        }
        //右から左への雲作成
        for (int i = 0; i < _CloudNum / 2; i++)
        {
            GameObject cloud = Instantiate(_CloudPrefab, new Vector3(rightX, _PosY, 0), Quaternion.identity);
            Vector2 fromToPosX = new Vector2(rightX, rightX + (leftX - rightX) * (i + 1) / (_CloudNum + 1));
            _KeyValuePairs.Add(cloud, fromToPosX);
        }
    }

    //すぐに雲を消す
    public void RemoveCloud()
    {
        //雲の消去
        if(_KeyValuePairs.Count > 0)
        {
            foreach (KeyValuePair<GameObject, Vector2> pair in _KeyValuePairs)
            {
                //オブジェクト消去
                GameObject cloud = pair.Key;
                Destroy(cloud);
            }
            //中身からに
            _KeyValuePairs.Clear();
        }
        _IsFin = true;
        _CanMove = false;
    }
}
