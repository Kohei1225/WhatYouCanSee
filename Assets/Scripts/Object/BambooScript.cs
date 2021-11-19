using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooScript : MonoBehaviour
{

    [SerializeField]private PandaScript _Panda = null;

    private Animator _AnimController = null;
    private TimerScript _Timer = new TimerScript();
    private bool hasDead = false;

    // Start is called before the first frame update
    void Start()
    {
        _AnimController = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (_Panda == null)
        {
            return;
        }

        if(!hasDead)
        {
            if(_Panda.IsDead)
            {
                hasDead = true;
                _Timer.ResetTimer(5.0f);
            }
        }
        else
        {
            if (!_Timer.IsTimeUp)
            {
                _Timer.UpdateTimer();
                _AnimController.SetBool("IsBreak",_Timer.IsTimeUp);
            }
            
        }
    }
}
