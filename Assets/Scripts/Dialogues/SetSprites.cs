using UnityEngine;
using UnityEngine.UI;

public class SetSprites : MonoBehaviour
{
    public string characterName;

    private Image _image;
    private Animator _anim;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _anim = GetComponent<Animator>();

        Hide();
    }
    private void Start()
    {
        PortraitManager.Instance.Register(characterName, this);
    }
    public void PlayExpression(string expression)
    {
        _image.enabled = true;
        _anim.enabled = true;

        switch (expression)
        {
            case "normal":
                _anim.SetTrigger("normal");
                break;

            case "happy":
                _anim.SetTrigger("happy");
                break;

            case "nervous":
                _anim.SetTrigger("nervous");
                break;
        }
    }

    public void Hide()
    {
        _image.enabled = false;
        _anim.enabled = false;
    }
}