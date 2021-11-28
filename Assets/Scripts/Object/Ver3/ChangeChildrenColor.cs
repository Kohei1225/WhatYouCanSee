using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 子オブジェクトの色を統一するクラス </summary>
public class ChangeChildrenColor : MonoBehaviour
{
    /// <summary> アタッチされてるColorObjectスクリプト </summary>
    private ColorObjectVer3 _ColorObject;
    /// <summary> アタッチされてるSpriteRenderer </summary>
    private SpriteRenderer _SpriteRenderer;

    /// <summary> 子オブジェクトのの配列 </summary>
    [SerializeField] private SpriteRenderer[] _ChildrenSpriteRenderers;

    // Start is called before the first frame update
    void Start()
    {
        _ColorObject = GetComponent<ColorObjectVer3>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ColorObjectVer3.OBJECT_COLOR3 colorType = _ColorObject.colorType;
        Color color = _SpriteRenderer.color;

        for(int i = 0; i < _ChildrenSpriteRenderers.Length; i++)
        {
            _ChildrenSpriteRenderers[i].color = color;
        }
    }
}