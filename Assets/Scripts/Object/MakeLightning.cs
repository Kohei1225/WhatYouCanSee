using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class MakeLightning : MonoBehaviour
{
    [SerializeField] private float _WaitTime = 3;

    private float _Time = 0;
    [SerializeField] private float _Speed = 20;
    [SerializeField] private Vector3 _StartPos;
    [SerializeField] private Vector3 _EndPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _Time += Time.deltaTime;
        float t = _Time * _Speed / (_EndPos - _StartPos).magnitude;
        transform.position = Vector3.Lerp(_StartPos, _EndPos, t);
        if (t >= 1)
        {
            if (gameObject.GetComponent<MakeLightning>() != null)
            {

            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Light2D>() != null)
        {
            GameObject lightObj = collision.gameObject;
            StartCoroutine(Lightning(lightObj));
        }
    }

    IEnumerator Lightning(GameObject lightObj)
    {
        //光らせる
        lightObj.GetComponent<Light2D>().enabled = true;
        lightObj.GetComponent<LightRayScriptVer2>().enabled = true;
        //音ならす
        SoundManager.Instance.PlaySE("Thunder");
        yield return new WaitForSeconds(_WaitTime);
        //光を消す
        lightObj.GetComponent<Light2D>().enabled = false;
        lightObj.GetComponent<LightRayScriptVer2>().enabled = false;
    }

}
