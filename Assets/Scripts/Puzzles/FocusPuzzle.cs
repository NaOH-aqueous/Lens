using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static FocusPuzzle;

public class FocusPuzzle : MonoBehaviour
{
    public System.Action OnPuzzleCompleted;

    [Header("Images")]
    [SerializeField] private Image clearImage;
    [SerializeField] private Image focusImage;
    [SerializeField] private Image blurryImage;
    [SerializeField] private Image marker;

    [Header("Puzzle Settings")]
    [SerializeField] private float duration = 3f;
    [SerializeField] private float puzzleThreshold = 0.3f;
    [SerializeField] private float overHoldDuration = 1f; // time to fade back out
    [SerializeField] private float requiredHold = 1f; // seconds required for success
    [SerializeField] private TextAsset puzzleDialogue;
    private AudioSource _aud;
    [SerializeField] private GameObject tip;

    private float minAlpha = 0f;
    private float maxAlpha = 1f; 

    private Color _color1;
    private Color _color2;

    private float focusState = 0f;
    private float overState = 0f;
    private float cycleTimer = 0f;
    private bool reversing = false;

    private Coroutine successCoroutine;
    private bool success = false;

    private float holdTimer = 0f;



    void Start()
    {
        _color1 = clearImage.color;
        _color2 = focusImage.color;
        marker.gameObject.SetActive(false);
        _aud = GetComponent<AudioSource>();
        tip.SetActive(false);
    }


    void Update()
    {
        if (success)
            return;

        if (Input.GetKey(KeyCode.F))
        {
            float maxCycle = requiredHold + overHoldDuration;

            if (!reversing)
            {
                cycleTimer += Time.unscaledDeltaTime;

                if (cycleTimer >= maxCycle)
                {
                    cycleTimer = maxCycle;
                    reversing = true; // start fading back down
                }
            }
            else
            {
                cycleTimer -= Time.unscaledDeltaTime;

                if (cycleTimer <= 0f)
                {
                    cycleTimer = 0f;
                    reversing = false;
                }
            }

            // derive states from cycle
            focusState = Mathf.Clamp01(cycleTimer / requiredHold);
            overState = Mathf.Clamp01((cycleTimer - requiredHold) / overHoldDuration);
        }

        // APPLY STATE ALWAYS (even after release)
        _color1.a = 1f - focusState;
        clearImage.color = _color1;

        _color2.a = 1f;
        focusImage.color = _color2;

        Color c3 = blurryImage.color;
        c3.a = overState;
        blurryImage.color = c3;

        if (Input.GetKeyUp(KeyCode.F))
        {
            if (focusState >= 0.95f)
                FocusSucceed();
        }
    }

    private void FocusSucceed()
    {
        //start coroutine if color1 is visible and lower than puzzle threshold
        if (focusState >= 0.95f)
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
            bool focusCondition = focusState >= 1f - puzzleThreshold;
            bool blurCondition = overState >= 0f && overState <= puzzleThreshold;

            if (!focusCondition || !blurCondition)
            {
                successCoroutine = null;
                yield break;
            }

            successTime += Time.unscaledDeltaTime;
            waited += Time.unscaledDeltaTime;
            Debug.Log(successTime);
            if (successTime >= requiredHold)
            {
                success = true;
                Debug.Log("succeed!");
                StartCoroutine(HandlePuzzleSucceed());
                successCoroutine = null;
                yield break;
            }

            yield return null;
        }
        successCoroutine = null;
    }

    private IEnumerator HandlePuzzleSucceed()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        marker.gameObject.SetActive(true);
        InputManager.Instance.RegisterInteractPressed();
        InputManager.Instance.RegisterSubmitPressed();
        yield return null;
        OnPuzzleCompleted?.Invoke();
    }
    
    public void SetButtonActive() //intro animation
    {
        tip.SetActive(true);
        tip.GetComponent<Animator>().SetTrigger("enterPuzzle");
    }

}
