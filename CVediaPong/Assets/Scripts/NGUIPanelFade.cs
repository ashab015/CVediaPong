using UnityEngine;
using System.Collections;


/// <summary> This class allows for NGUI Widget and UIPanel fading via the widget / panel alpha values. </summary>
public class NGUIPanelFade : MonoBehaviour {

    /// <summary> How the UI Element fade in and out. </summary>
    public float FadeSpeed;
    /// <summary> True if this object is fading. </summary>
    private bool IsFading = false;
    /// <summary> True to widget. </summary>
    public bool Widget = false;
    /// <summary> If checked this will fade the UIPanel or UIWidget in when the game starts. </summary>
    public bool OnStart = false;
    /// <summary> If checked this will fade the UIPanel or UIWidget in when the gameobject is enabled. </summary>
    public bool OnEnabled = false;

    /// <summary> Starts this object. </summary>
    void Start()
    {
        if (OnStart == true)
        {
            if (Widget == true)
                GetComponent<UIWidget>().alpha = 0.0f;
            else
                GetComponent<UIPanel>().alpha = 0.0f;
            FadeIn();
        }
            
    }
    /// <summary> Executes the enable action. </summary>
    void OnEnable()
    {
        if (OnEnabled == true)
        {
            if (Widget == true)
                GetComponent<UIWidget>().alpha = 0.0f;
            else
                GetComponent<UIPanel>().alpha = 0.0f;
            FadeIn();
        }
    }
    /// <summary> Fade in. </summary>
    public void FadeIn()
    {
        if (Widget == false)
            StartCoroutine(FadeInE());
        else
            StartCoroutine(FadeInE2());
    }

    /// <summary> Fade out. </summary>
    public void FadeOut()
    {
        if (Widget == false)
            StartCoroutine(FadeOutE());
        else
            StartCoroutine(FadeOutE2());
    }

    /// <summary> Diffrent fade in and fade out ienumerator functions. </summary>
    ///
    /// <returns> An IEnumerator. </returns>
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
    /// <summary> Fade out e. </summary>
    ///
    /// <returns> An IEnumerator. </returns>
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
    /// <summary> Fade in e 2. </summary>
    ///
    /// <returns> An IEnumerator. </returns>
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
    /// <summary> Fade out e 2. </summary>
    ///
    /// <returns> An IEnumerator. </returns>
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

    /// <summary> This functions let the UI Element fade in then wait for a specified seconds then fade out. </summary>
    ///
    /// <param name="seconds">  The seconds. </param>
    public void FadeInWaitFadeOut(float seconds)
    {
        StartCoroutine(FadeInWaitFadeOutE(seconds));
    }
    /// <summary> Fade in wait fade out e. </summary>
    ///
    /// <param name="seconds">  The seconds. </param>
    ///
    /// <returns> An IEnumerator. </returns>
    private IEnumerator FadeInWaitFadeOutE(float seconds)
    {

        yield return StartCoroutine(FadeInE());

        yield return new WaitForSeconds(seconds);

        yield return StartCoroutine(FadeOutE());

    }

   
}
