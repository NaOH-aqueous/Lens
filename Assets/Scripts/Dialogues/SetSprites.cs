using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SetSprites : MonoBehaviour
{
    public string characterName;

    private Image _image;
    private Animator _anim;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _anim = GetComponent<Animator>();

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // start hidden
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;

        Hide();
    }

    private void Start()
    {
        if (PortraitManager.Instance != null)
        {
            PortraitManager.Instance.Register(characterName, this);
        }
    }

    private void OnDestroy()
    {
        // Unregister from PortraitManager so the manager doesn't keep a destroyed reference.
        if (PortraitManager.Instance != null)
        {
            PortraitManager.Instance.Unregister(characterName, this);
        }
    }

    public void PlayExpression(string expression)
    {
        // Ensure any hide tweens are cancelled and show the portrait
        if (_canvasGroup != null)
        {
            TweenHelper.FadeCanvasGroup(_canvasGroup, 1f, 0.25f, true, Ease.OutQuad);
        }

        if (_image != null)
            _image.enabled = true;

        if (_anim != null)
            _anim.enabled = true;

        switch (expression)
        {
            case "normal":
                _anim?.SetTrigger("normal");
                break;

            case "happy":
                _anim?.SetTrigger("happy");
                break;

            case "nervous":
                _anim?.SetTrigger("nervous");
                break;
        }
    }

    public void Hide()
    {
        // Fade out and disable image/anim when complete
        Tween t = null;
        if (_canvasGroup != null)
        {
            t = TweenHelper.FadeCanvasGroup(_canvasGroup, 0f, 0.15f, true, Ease.OutQuad);
        }

        if (t != null)
        {
            t.OnComplete(() =>
            {
                if (_image != null)
                    _image.enabled = false;
                if (_anim != null)
                    _anim.enabled = false;
            });
        }
        else
        {
            if (_image != null)
                _image.enabled = false;
            if (_anim != null)
                _anim.enabled = false;
        }
    }
}