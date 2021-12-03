using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCloud : MonoBehaviour
{
    [SerializeField] private float _MakeInterval = 5;
    [SerializeField] private GameObject _Cloud;
    private float _Time = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _Time += Time.deltaTime;
        if(_Time >= _MakeInterval)
        {
            _Time = 0;
            Instantiate(_Cloud);
        }
    }
}
