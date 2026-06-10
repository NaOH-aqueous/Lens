using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortraitManager : MonoBehaviour
{
    public static PortraitManager Instance;

    private bool cutsceneOverride = false;

    private Dictionary<string, SetSprites> portraits = new Dictionary<string, SetSprites>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnPortraitTagChanged += HandlePortraitChanged;
            DialogueManager.Instance.OnDialogueStatusChanged += HandleDialogueStatus;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnPortraitTagChanged -= HandlePortraitChanged;
            DialogueManager.Instance.OnDialogueStatusChanged -= HandleDialogueStatus;
        }
    }

    public void SetCutscenePortraitLock(bool active)
    {
        cutsceneOverride = active;

        if (active)
        {
            HideAllPortraits();
        }
        else
        {
            StartCoroutine(ReapplyCurrentLine());
        }
    }

    private IEnumerator ReapplyCurrentLine()
    {
        yield return null;

        string speaker = DialogueManager.Instance.GetSpeakerTag();
        string portrait = DialogueManager.Instance.GetExpressionTag();

        ApplyPortrait(speaker, portrait);
    }


    public void Register(string characterName, SetSprites sprite)
    {
        if (!portraits.ContainsKey(characterName))
        {
            portraits.Add(characterName, sprite);
        }
    }

    private void HandlePortraitChanged(string speaker, string portraitTag)
    {
        if (cutsceneOverride)
        {
            return;
        }

        ApplyPortrait(speaker, portraitTag);

    }

    private void ApplyPortrait(string speaker, string portraitTag)
    {
        if (string.IsNullOrEmpty(portraitTag))
        {
            HideAllPortraits();
            return;
        }

        if (string.IsNullOrEmpty(speaker))
            return;

        if (portraits.TryGetValue(speaker, out SetSprites sprite))
        {
            sprite.PlayExpression(portraitTag);
        }
        else
        {
            HideAllPortraits();
        }
    }

    private void HandleDialogueStatus(bool playing)
    {
        if (!playing)
        {
            foreach (var p in portraits.Values)
            {
                p.Hide();
            }
        }
    }

    private void HideAllPortraits()
    {
        foreach (var portrait in portraits.Values)
        {
            portrait.Hide();
        }
    }
}