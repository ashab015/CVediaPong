using UnityEngine;
using System.Collections;


// This class allows for NGUI Widget and UIPanel fading via the widget / panel alpha values.
public class NGUIPanelFade : MonoBehaviour {

    // How the UI Element fade in and out
    public float FadeSpeed;
    private bool IsFading = false;
    public bool Widget = false;

    public void FadeIn()
    {
        if (Widget == false)
            StartCoroutine(FadeInE());
        else
            StartCoroutine(FadeInE2());
    }

    public void FadeOut()
    {
        if (Widget == false)
            StartCoroutine(FadeOutE());
        else
            StartCoroutine(FadeOutE2());
    }

    // Diffrent fade in and fade out ienumerator functions
    private IEnumerator FadeInE()
    {

        float timeSinceStarted = 0f;
        if (IsFading == true || GetComponent<UIPanel>().alpha == 1.0f)
        {
            yield break;
        }
        IsFading = true;
        while (true)
        {
            timeSinceStarted += FadeSpeed * Time.deltaTime;
            GetComponent<UIPanel>().alpha = Mathf.Lerp(GetComponent<UIPanel>().alpha, 1.0f, timeSinceStarted);

            // if the object has arrived stop the corotine
            float distance = 1.0f - GetComponent<UIPanel>().alpha;
            if (GetComponent<UIPanel>().alpha == 1.0f || distance < 0.05)
            {
                GetComponent<UIPanel>().alpha = 1.0f;
                IsFading = false;
                yield break;
            }
            // otherwise continue new frame
            yield return null;
        }

    }
    private IEnumerator FadeOutE()
    {

        float timeSinceStarted = 0f;
        if (IsFading == true || GetComponent<UIPanel>().alpha == 0.0f)
        {
            yield break;
        }
        IsFading = true;
        while (true)
        {
            timeSinceStarted += FadeSpeed * Time.deltaTime;
            GetComponent<UIPanel>().alpha = Mathf.Lerp(GetComponent<UIPanel>().alpha, 0.0f, timeSinceStarted);

            // if the object has arrived stop the corotine
            float distance = GetComponent<UIPanel>().alpha;
            if (GetComponent<UIPanel>().alpha == 0.0f || distance < 0.05)
            {
                GetComponent<UIPanel>().alpha = 0.0f;
                IsFading = false;
                yield break;
            }
            // otherwise continue new frame
            yield return null;
        }

    }
    private IEnumerator FadeInE2()
    {

        float timeSinceStarted = 0f;
        if (IsFading == true || GetComponent<UIWidget>().alpha == 1.0f)
        {
            yield break;
        }
        IsFading = true;
        while (true)
        {
            timeSinceStarted += FadeSpeed * Time.deltaTime;
            GetComponent<UIWidget>().alpha = Mathf.Lerp(GetComponent<UIWidget>().alpha, 1.0f, timeSinceStarted);

            // if the object has arrived stop the corotine
            float distance = 1.0f - GetComponent<UIWidget>().alpha;
            if (GetComponent<UIWidget>().alpha == 1.0f || distance < 0.05)
            {
                GetComponent<UIWidget>().alpha = 1.0f;
                IsFading = false;
                yield break;
            }
            // otherwise continue new frame
            yield return null;
        }

    }
    private IEnumerator FadeOutE2()
    {

        float timeSinceStarted = 0f;
        if (IsFading == true || GetComponent<UIWidget>().alpha == 0.0f)
        {
            yield break;
        }
        IsFading = true;
        while (true)
        {
            timeSinceStarted += FadeSpeed * Time.deltaTime;
            GetComponent<UIWidget>().alpha = Mathf.Lerp(GetComponent<UIWidget>().alpha, 0.0f, timeSinceStarted);

            // if the object has arrived stop the corotine
            float distance = GetComponent<UIWidget>().alpha;
            if (GetComponent<UIWidget>().alpha == 0.0f || distance < 0.05)
            {
                GetComponent<UIWidget>().alpha = 0.0f;
                IsFading = false;
                yield break;
            }
            // otherwise continue new frame
            yield return null;
        }

    }

    // This functions let the UI Element fade in then wait for a specified seconds then fade out
    public void FadeInWaitFadeOut(float seconds)
    {
        StartCoroutine(FadeInWaitFadeOutE(seconds));
    }
    private IEnumerator FadeInWaitFadeOutE(float seconds)
    {

        yield return StartCoroutine(FadeInE());

        yield return new WaitForSeconds(seconds);

        yield return StartCoroutine(FadeOutE());

    }

   
}
