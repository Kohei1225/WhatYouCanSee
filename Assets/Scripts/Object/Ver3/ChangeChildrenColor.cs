using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 子オブジェクトの色を統一するクラス </summary>
public class ChangeChildrenColor : MonoBehaviour
{
    /// <summary> アタッチされてるColorObjectスクリプト </summary>
    private ColorObjectVer3 _ColorObject;

    /// <summary> 子オブジェクトのSpriteRendererの配列 </summary>
    [SerializeField] private SpriteRenderer[] _ChildrenSpriteRenderers;

    // Start is called before the first frame update
    void Start()
    {
        _ColorObject = GetComponent<ColorObjectVer3>();
    }

    // Update is called once per frame
    void Update()
    {
        ColorObjectVer3.OBJECT_COLOR3 colorType = _ColorObject.colorType;
        Color color = _ColorObject.ChangeColorByType(colorType);

        for(int i = 0; i < _ChildrenSpriteRenderers.Length; i++)
        {
            _ChildrenSpriteRenderers[i].color = color;
        }
    }
}