using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class SetSprites : MonoBehaviour
{
    public string characterName;

    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteNervous;
    [SerializeField] private Sprite spriteHappy;
    [SerializeField] private Sprite spriteSad;
    [SerializeField] private Sprite spriteThinking;
    [SerializeField] private Sprite spriteAngry;

    [SerializeField] private Image spriteImage;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ChangeSprite();
    }

    private void ChangeSprite()
    {
        if(DialogueManager.Instance.GetSpeakerTag() == characterName)
        {
            string currrentSprite = DialogueManager.Instance.GetExpressionTag();
            switch (currrentSprite)
            {
                case "normal":
                    setPortrait(spriteNormal);
                    break;
                case "nervous":
                    setPortrait(spriteNervous);
                    break;
                case "happy":
                    setPortrait(spriteHappy);
                    break;
                case "sad":
                    setPortrait(spriteSad);
                    break;
                case "thinking":
                    setPortrait(spriteThinking);
                    break;
                case "angry":
                    setPortrait(spriteAngry);
                    break;
            }
        }
        //spriteRenderer.sprite = sprite;
    }

    private void setPortrait(Sprite sprite)
    {
        if(sprite != null && sprite != spriteRenderer.sprite)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            return;
        }
    }
}
