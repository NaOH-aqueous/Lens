using UnityEngine;

public class HomeLensController : LensController
{
    public System.Action OnLensTriggered;
    public System.Action<Item> OnItemTriggered;
    [SerializeField] private LensControl lens;
    [SerializeField] private GameObject altImages;
    [SerializeField] private GameObject pixie;
    [SerializeField] private GameObject pixieLineup;
    [SerializeField] private GameObject pixieWithoutKey;

    private void Start()
    {
        lens.gameObject.SetActive(false);
        altImages.SetActive(false);
    }
    private void Update()
    {
        if(GameManager.instance.CurrentState != GameStateType.Lens)
        {
            return;
        }

        if (InputManager.Instance.IsSubmitPressed())
        {
            Item lensItem = lens.InteractLensItem();
            if(lensItem == null)
            {
                OnLensTriggered?.Invoke();
            }
            else
            {
                OnItemTriggered?.Invoke(lensItem);
            }
        }
    }

    public override void EnableLens()
    {
        base.EnableLens();
        lens.gameObject.SetActive(true);
        altImages.SetActive(true);
    }

    public override void DisableLens()
    {
        base.DisableLens();
        lens.gameObject.SetActive(false);
        altImages.SetActive(false);
    }

    public void SetPixieAltSprite(bool enable) //control the visibility of the pixie alt image
    {
        if (enable)
        {
            pixie.gameObject.SetActive(true);
        }
        else
        {
            pixie.gameObject.SetActive(false);
        }

    }

    public void SetPixieLineup(bool enable)
    {
        if (enable)
        {
            pixieLineup.gameObject.SetActive(true);
        }
        else
        {
            pixieLineup.gameObject.SetActive(false);
        }
    }

    public void SetPixieWithoutKey(bool enable)
    {
        if (enable)
        {
            pixieWithoutKey.gameObject.SetActive(true);
        }
        else
        {
            pixieWithoutKey.gameObject.SetActive(false);
        }
    }
}
