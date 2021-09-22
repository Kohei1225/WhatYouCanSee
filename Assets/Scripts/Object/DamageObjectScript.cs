using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ダメージを与えるオブジェクトにアタッチするクラス
public class DamageObjectScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D otherObject)
    {
        if(GetComponent<ColorObjectScript>())
        {
            if(!GetComponent<ColorObjectScript>().noBody)
            {
                if(otherObject.gameObject.tag == "Player")
                {
                    if(!otherObject.gameObject.GetComponent<ColorObjectScript>().noBody)
                    {
                        otherObject.gameObject.GetComponent<PlayerController>().damage = true;
                    }
                }
            }
        }
    }
}
