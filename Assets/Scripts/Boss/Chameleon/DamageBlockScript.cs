using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBlockScript : MonoBehaviour
{
    /// <summary> ダメージを与えられるか </summary>
    public bool _CanDamage;
    /// <summary> これよりスピードが遅いとダメージがなくなる </summary>
    [SerializeField] private float _NotDamageSpeedLine = 0.2f;
    private bool _IsAccelerate = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_CanDamage)
        {
            float velX = Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x);
            if (!_IsAccelerate)
            {
                //十分に加速したらtrue
                _IsAccelerate = velX > _NotDamageSpeedLine;
            }
            else
            {
                //スピードが無くなったら
                if (velX <= _NotDamageSpeedLine)
                {
                    //リセット
                    //ダメージは当られない
                    _CanDamage = false;
                    _IsAccelerate = false;
                }
            }
        }
    }
}
