using UnityEngine;

public class HomeLensController : LensController
{
    public System.Action OnLensClosed;
    public System.Action OnLensTriggered;
    public System.Action<Item> OnItemTriggered;
    [SerializeField] private LensControl lens;
    [SerializeField] private GameObject altImages;
    [SerializeField] private GameObject pixie;
    [SerializeField] private GameObject pixieLineup;
    [SerializeField] private GameObject pixieWithoutKey;
    [SerializeField] private GameObject voidness;

    // Prevent duplicate subscriptions
    private bool submitSubscribed;

    private void Start()
    {
        lens.gameObject.SetActive(false);
        altImages.SetActive(false);
    }

    private void OnDestroy()
    {
        UnsubscribeSubmit();
    }

    public override void EnableLens()
    {
        base.EnableLens();
        lens.gameObject.SetActive(true);
        altImages.SetActive(true);
        SubscribeSubmit();
    }

    public override void DisableLens()
    {
        base.DisableLens();
        lens.gameObject.SetActive(false);
        altImages.SetActive(false);

        UnsubscribeSubmit();
        CloseAllAltImages();
        OnLensClosed?.Invoke();
    }

    private void SubscribeSubmit()
    {
        if (submitSubscribed)
            return;

        if (InputManager.Instance == null)
            return;

        // defensive unsubscribe then subscribe
        InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;
        InputManager.Instance.SubmitPerformed += OnSubmitPerformed;
        submitSubscribed = true;
    }

    private void UnsubscribeSubmit()
    {
        if (!submitSubscribed)
            return;

        if (InputManager.Instance != null)
            InputManager.Instance.SubmitPerformed -= OnSubmitPerformed;

        submitSubscribed = false;
    }

    private void OnSubmitPerformed()
    {
        if (GameManager.instance.CurrentState != GameStateType.Lens)
            return;

        if (DialogueManager.Instance != null && DialogueManager.Instance.CheckDialoguePlaying())
            return;

        Item lensItem = lens.InteractLensItem();
        if (lensItem == null)
        {
            OnLensTriggered?.Invoke();
        }
        else
        {
            OnItemTriggered?.Invoke(lensItem);
        }
    }

    private void CloseAllAltImages()
    {
        pixie?.SetActive(false);
        pixieLineup?.SetActive(false);
        pixieWithoutKey?.SetActive(false);
        voidness?.SetActive(false);
    }

    public void SetPixieAltSprite(bool enable) //control the visibility of the pixie alt image
    {
        pixie?.SetActive(enable);
    }

    public void SetPixieLineup(bool enable)
    {
        pixieLineup?.SetActive(enable);
    }

    public void SetPixieWithoutKey(bool enable)
    {
        pixieWithoutKey?.SetActive(enable);
    }

    public void SetVoidnessAlt(bool enable)
    {
        voidness?.SetActive(enable);
    }
}
