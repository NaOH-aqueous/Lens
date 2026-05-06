using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FocusPuzzle : MonoBehaviour
{
    public enum FadeMode
    {
        Inverse, //when the image 2 happens first
        Chain //when the change happens following image1
    }

    [Header("Images")] 
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;

    [Header("Puzzle Settings")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float chainThreshold = 0.3f;
    [SerializeField] private float puzzleThreshold = 0.3f;
    [SerializeField] private float requiredHold = 0.7f; // seconds required for success

    private float minAlpha = 0f;
    private float maxAlpha = 1f; 

    private FadeMode mode = FadeMode.Inverse;
    private Color _color1;
    private Color _color2;

    private Coroutine fadeCoroutine;
    private Coroutine successCoroutine;
    private bool success = false;

    void Start()
    {
        _color1 = image1.color;
        _color2 = image2.color;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            //change speed
            float d = Mathf.Max(0.0001f, duration);
            float time = Time.time;
            float t = Mathf.PingPong(time / d, 1);

            //Ping-pong between minAlpha and maxAlpha;
            _color1.a = Mathf.Lerp(minAlpha, maxAlpha, t);

            //apply new color back
            image1.color = _color1;

            switch (mode)
            {
                case FadeMode.Inverse:
                    _color2.a = Mathf.Lerp(maxAlpha, minAlpha, t);
                    image2.color = _color2;
                    break;

                case FadeMode.Chain:
                    if (_color1.a <= chainThreshold && fadeCoroutine == null)
                    {
                        //increase transparency when the focus image is not
                        //visible
                        fadeCoroutine = StartCoroutine(FadeBlurToTargetAlpha(maxAlpha));
                    }
                    else if (_color1.a > chainThreshold && fadeCoroutine == null)
                    {
                        //fade to 0 when the alpha of focus image is visible
                        fadeCoroutine = StartCoroutine(FadeBlurToTargetAlpha(minAlpha));
                    }
                    break;
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            //check if the puzzel has been solved upon key released
            FocusSucceed();
        }
    }

    private IEnumerator FadeBlurToTargetAlpha(float targetAlpha)
    {
        float start = _color2.a;
        float elapsed = 0f;

        //if duration is too small, snap image2 alpha to target.
        if (duration <= 0.001f)
        {
            _color2.a = targetAlpha;
            image2.color = _color2;
            fadeCoroutine = null;
            yield break;
        }

       //check elapsed time and apply it as a fraction to the alpha of image2
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            //check the progress of elapsed time and compares it to duration
            //of one transition
            float k = Mathf.Clamp01(elapsed / duration);
            _color2.a = Mathf.Lerp(start, targetAlpha, k);
            image2.color = _color2;
            yield return null;
        }

        _color2.a = targetAlpha;
        image2.color =  _color2;
        fadeCoroutine = null;
    }

    private void FocusSucceed()
    {
        //start coroutine if color1 is visible and lower than puzzle threshold
        if (_color1.a >= 0f && _color1.a <= puzzleThreshold && !success && successCoroutine == null)
        {
            successCoroutine = StartCoroutine(CheckForSuccess());
        }
    }
    
    private IEnumerator CheckForSuccess()
    {
        float successTime = 0f;
        const float maxWait = 5f; // timeout limit
        float waited = 0f;

        //if the waited time is within the timeout limit, enter the loop
        while (waited < maxWait)
        {
            //two conditions, one for image1 and another for image2, both
            //are required for the success of the puzzle
            bool focusCondition = (_color1.a >= 0f && _color1.a <= puzzleThreshold);
            bool blurCondition = (_color2.a >= 1f - puzzleThreshold && _color2.a <= 1f + puzzleThreshold);

            if (!focusCondition || !blurCondition)
            {
                // if one of the conditions cannot be succeed, break the loop
                successCoroutine = null;
                yield break;
            }

            successTime += Time.deltaTime;
            waited += Time.deltaTime;
            Debug.Log(successTime);

            //if the success time has been achieved, break out from the loop
            if (successTime >= requiredHold)
            {
                success = true;
                Debug.Log("succeed!");
                successCoroutine = null;
                yield break;
            }

            yield return null;
        }
        successCoroutine = null;
    }
}
