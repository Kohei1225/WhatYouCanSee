using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingScript : MonoBehaviour
{
    private float _WingSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //一定の範囲外に出たら消える
        if(transform.position.x < -50 || 50 < transform.position.x
            || transform.position.y < -50 && 50 < transform.position.y)
        {
            Destroy(gameObject);
        }

        //GetComponent<Rigidbody2D>().velocity = transform.forward * _WingSpeed;
        transform.Translate(Vector3.right * _WingSpeed * Time.deltaTime);

        //transform.Translate(0.05f, 0, 0);
    }
}
