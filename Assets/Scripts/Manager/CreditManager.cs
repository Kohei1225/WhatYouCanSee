using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CreditManager : MonoBehaviour
{
    [SerializeField] private float _Time = 0;
    public enum StateEnum
    {
        FIRST_WAIT,
        LOGO_FADE_IN,
        SECOND_WAIT,
        LOGO_FADE_OUT,
        LAST_WAIT,
    }
    [SerializeField] private StateEnum _State = StateEnum.FIRST_WAIT;
    [SerializeField] private SpriteRenderer _SpriteRendererLogo;
    [SerializeField] private float _FadeTime = 3;
    [SerializeField] private float _FirstWaitTime = 1;
    [SerializeField] private float _SecondWaitTime = 3;
    [SerializeField] private float _LastWaitTime = 1;

    [SerializeField] private Text _DendaiText;
    [SerializeField] private TextMeshProUGUI _AMText;

    // Start is called before the first frame update
    void Start()
    {
        float alpha = 0;
        _SpriteRendererLogo.color = GetChangeColor(_SpriteRendererLogo.color, alpha);
        _DendaiText.color = GetChangeColor(_DendaiText.color, alpha);
        _AMText.color = GetChangeColor(_AMText.color, alpha);
    }

    // Update is called once per frame
    void Update()
    {
        _Time += Time.deltaTime;

        switch (_State)
        {
            case StateEnum.FIRST_WAIT:
                if (_Time >= _FirstWaitTime)
                {
                    _Time = 0;
                    _State = StateEnum.LOGO_FADE_IN;
                }
                break;
            case StateEnum.LOGO_FADE_IN:
                float alpha = Mathf.Lerp(0, 1, _Time / _FadeTime);
                _SpriteRendererLogo.color = GetChangeColor(_SpriteRendererLogo.color, alpha);
                _DendaiText.color = GetChangeColor(_DendaiText.color, alpha);
                _AMText.color = GetChangeColor(_AMText.color, alpha);
                if (_Time >= _FadeTime)
                {
                    _Time = 0;
                    _State = StateEnum.SECOND_WAIT;
                }
                break;
            case StateEnum.SECOND_WAIT:
                if (_Time >= _SecondWaitTime)
                {
                    _Time = 0;
                    _State = StateEnum.LOGO_FADE_OUT;
                }
                break;
            case StateEnum.LOGO_FADE_OUT:
                alpha = Mathf.Lerp(1, 0, _Time / _FadeTime);
                _SpriteRendererLogo.color = GetChangeColor(_SpriteRendererLogo.color, alpha);
                _DendaiText.color = GetChangeColor(_DendaiText.color, alpha);
                _AMText.color = GetChangeColor(_AMText.color, alpha);
                if (_Time >= _FadeTime)
                {
                    _Time = 0;
                    _State = StateEnum.LAST_WAIT;
                }
                break;
            case StateEnum.LAST_WAIT:
                if (_Time >= _LastWaitTime)
                {
                    SceneManager.LoadScene("Title");
                }
                break;
        }
    }

    private Color GetChangeColor(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}