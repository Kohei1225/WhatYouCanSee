using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslateByLever : MonoBehaviour
{
    private Vector3[] _Poses;
    [SerializeField] private GameObject[] _TranslatePosObjs;
    [SerializeField] private LeverScript _LeverScript;
    // Start is called before the first frame update
    void Start()
    {
        _Poses = new Vector3[_TranslatePosObjs.Length];
        for (int i = 0; i < _TranslatePosObjs.Length; i++)
        {
            _Poses[i] = _TranslatePosObjs[i].transform.position;
        }
        _TranslatePosObjs = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (_Poses.Length <= _LeverScript.barPosition)
            return;
        transform.position = _Poses[_LeverScript.barPosition];
    }
}
