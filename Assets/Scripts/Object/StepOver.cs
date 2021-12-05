using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepOver : MonoBehaviour
{
    public AreaInObj areaInObjUp;
    public AreaInObj areaInObjDown;
    private Rigidbody2D rigidbody2D;
    public float height = 0.8f;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody2D.velocity.x != 0)
        {
            bool up = areaInObjUp.IsIn;
            bool down = areaInObjDown.IsIn;
            if (up == false && down == true)
            {
                transform.Translate(Vector3.up * height);
                Debug.Log("Object up");
            }
        }
    }

}
