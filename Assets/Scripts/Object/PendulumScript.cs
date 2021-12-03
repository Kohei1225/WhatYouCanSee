using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    [SerializeField] private float _MaxTheta = 60;
    private float _Theta;
    [SerializeField] private float _Gravity = 9.8f;
    private float _W;
    private float _Length;
    private float _Time = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(Vector3.forward * _MaxTheta);
        _Length = transform.Find("Bar").gameObject.transform.localScale.y;
        _W = Mathf.Sqrt(_Gravity / _Length);
    }

    // Update is called once per frame
    void Update()
    {
        _Time += Time.deltaTime;
        _Theta = _MaxTheta * Mathf.Cos(_W * _Time);
        transform.rotation = Quaternion.Euler(Vector3.forward * _Theta);
    }
}
