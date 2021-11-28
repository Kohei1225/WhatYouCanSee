using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeScript : MonoBehaviour
{
    //八の字の中心座標
    private float _CenterX;
    private float _CenterY;
    //経過時間
    private float _Time = 0;
    //飛んでいるときのスピード
    [SerializeField] private float _FlySpeed = 2;
    //攻撃しているときのスピード
    [SerializeField] private float _AttackSpeed = 20;
    //戻るときのスピード
    [SerializeField] private float _BackSpeed = 10;
    private Vector3[] _FromToPoses = new Vector3[2];
    [SerializeField] private float _FigureEightWidth = 5;
    [SerializeField] private float _FigureEightHeight = 2;
    [SerializeField] private float _BeforeAttackTime = 2;
    [SerializeField] private float _BeforeBackTime = 1;
    [SerializeField] private float _MinAttackInterval = 2;
    [SerializeField] private float _MaxAttackInterval = 4;
    [SerializeField] private GameObject _TargetMarkerPrefab;
    //格納するための物
    private GameObject _TargetMarkerObj;
    private float _WaitTime;
    private float _AttackInterval;
    [SerializeField] private GameObject _Player;
    //コライダー
    private Collider2D _Collider2D;
    public enum StateEnum
    {
        FLY,
        TARGET,
        ATTACK,
        WAIT,
        BACK
    }
    [SerializeField] private StateEnum _State;

    public GameObject Player
    {
        set
        {
            _Player = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //座標記憶
        _CenterX = transform.position.x;
        _CenterY = transform.position.y;
        //状態を飛ぶにする
        _State = StateEnum.FLY;
        //攻撃の間隔を決める
        _AttackInterval = Random.Range(_MinAttackInterval, _MaxAttackInterval);
        _Collider2D = GetComponent<Collider2D>();
        //コライダーをなくす
        _Collider2D.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_State)
        {
            case StateEnum.FLY:
                //八の字に飛ぶ
                FlyEight();
                break;
            case StateEnum.TARGET:
                _Time += Time.deltaTime;
                if(_Time >= _BeforeAttackTime)
                {
                    _Time = 0;
                    _State = StateEnum.ATTACK;
                    //コライダーをつける
                    _Collider2D.enabled = true;
                }
                break;
            case StateEnum.ATTACK:
                Attack();
                break;
            case StateEnum.WAIT:
                _Time += Time.deltaTime;
                if (_Time >= _BeforeBackTime)
                {
                    _Time = 0;
                    //マーカー削除
                    if(_TargetMarkerObj != null)
                    {
                        Destroy(_TargetMarkerObj);
                        _TargetMarkerObj = null;
                    }
                    _State = StateEnum.BACK;
                }
                break;
            case StateEnum.BACK:
                Back();
                break;
        }
    }

    private void FlyEight()
    {
        _Time += Time.deltaTime;
        //位置の状態を-1から1として表す
        float t_x = 2 * Mathf.Sin(_Time * _FlySpeed) - 1;
        float t_y = 2 * Mathf.Sin(_Time * _FlySpeed * 2) - 1;
        //移動
        transform.position = new Vector3(_CenterX + t_x * _FigureEightWidth / 2, _CenterY + t_y * _FigureEightHeight / 2);
        //もし攻撃するなら
        if (_Time >= _AttackInterval)
        {
            _Time = 0;
            //位置をセット
            _FromToPoses[0] = transform.position;
            _FromToPoses[1] = _Player.transform.position;
            //マーカー生成
            _TargetMarkerObj = Instantiate(_TargetMarkerPrefab, _FromToPoses[1], Quaternion.identity);
            //マーカの描画順設定
            _TargetMarkerObj.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
            _State = StateEnum.TARGET;
        }
    }

    private void Attack()
    {
        _Time += Time.deltaTime;
        //距離
        float length = (_FromToPoses[1] - _FromToPoses[0]).magnitude;
        //移動
        transform.position = Vector3.Lerp(_FromToPoses[0], _FromToPoses[1], _Time * _AttackSpeed / length);
        //攻撃し終えたら
        if (_Time * _AttackSpeed / length >= 1)
        {
            _Time = 0;
            //コライダーをなくす
            _Collider2D.enabled = false;
            //位置をセット
            _FromToPoses[0] = transform.position;
            _FromToPoses[1] = new Vector3(_CenterX, _CenterY, 0);
            _State = StateEnum.WAIT;
        }
    }

    private void Back()
    {
        _Time += Time.deltaTime;
        //距離
        float length = (_FromToPoses[1] - _FromToPoses[0]).magnitude;
        //移動
        transform.position = Vector3.Lerp(_FromToPoses[0], _FromToPoses[1], _Time * _BackSpeed / length);
        //戻れたら
        if (_Time * _BackSpeed / length >= 1)
        {
            _Time = 0;
            _State = StateEnum.FLY;
        }
    }

    public void ResetBee()
    {
        _State = StateEnum.FLY;
        _Time = 0;
        //コライダーをなくす
        _Collider2D.enabled = false;
        //マーカー削除
        if (_TargetMarkerObj != null)
        {
            Destroy(_TargetMarkerObj);
            _TargetMarkerObj = null;
        }
    }
}
