using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    private bool fadeState = false;

    private float duration = 10f;

    private void Update()
    {
        Fade();
    }

    public void Fade()
    {
        var canvas = GetComponent<CanvasGroup>();

        if (canvas.alpha > 0)
        {
            StartCoroutine(DoFadeOut(canvas, canvas.alpha, 0));
        }
        fadeState = true;
    }

    public IEnumerator DoFadeOut(CanvasGroup _canvas, float _startCanvasValue, float _endCanvasValue)
    {
        float counter = 0;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            if (_canvas.alpha > 0.01f)
            {
                _canvas.alpha = Mathf.Lerp(_startCanvasValue, _endCanvasValue, counter / duration);
            }
            else
            {
                _canvas.alpha = 0;
            }

            yield return null;
        }
    }
}
