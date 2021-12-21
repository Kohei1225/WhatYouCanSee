using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//対象のオブジェクトにTextを移動させるscript
public class TextTransNearObj : MonoBehaviour
{
    //対象のオブジェクト
    [SerializeField] private GameObject _TargetObj;
    //対象のオブジェクトからどのくらい離れているか
    [SerializeField] private Vector2 _DeltaVec;
    //カメラ
    private Camera _Camera;
    //キャンバスのRectTransform
    [SerializeField] private RectTransform _CanvasRect;
    // Start is called before the first frame update
    void Start()
    {
        _Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(_Camera, _TargetObj.transform.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_CanvasRect, screenPos, _Camera, out newPos);

        GetComponent<RectTransform>().localPosition = newPos + _DeltaVec;
    }
}
