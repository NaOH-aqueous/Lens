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

        if (DialogueManager.Instance == null)
            yield break;

        string speaker = DialogueManager.Instance.GetSpeakerTag();
        string portrait = DialogueManager.Instance.GetExpressionTag();

        ApplyPortrait(speaker, portrait);
    }

    public void Register(string characterName, SetSprites sprite)
    {
        if (string.IsNullOrEmpty(characterName) || sprite == null)
            return;

        // If an entry exists but the referenced object was destroyed, replace it.
        if (portraits.TryGetValue(characterName, out SetSprites existing))
        {
            if (existing == null)
            {
                portraits[characterName] = sprite;
                return;
            }

            // If the same sprite is already registered, nothing to do.
            if (existing == sprite)
                return;

            // Otherwise new sprite for the same name - replace and log a warning.
            portraits[characterName] = sprite;
            Debug.LogWarning($"PortraitManager: Replaced existing portrait for '{characterName}'.");
            return;
        }

        portraits.Add(characterName, sprite);
    }

    public void Unregister(string characterName, SetSprites sprite)
    {
        if (string.IsNullOrEmpty(characterName))
            return;

        if (portraits.TryGetValue(characterName, out SetSprites existing))
        {
            // Remove if matching instance or if existing has been destroyed.
            if (existing == null || existing == sprite)
            {
                portraits.Remove(characterName);
            }
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

        // Clean up any destroyed entries before lookup
        CleanupDestroyedEntries();

        if (portraits.TryGetValue(speaker, out SetSprites sprite) && sprite != null)
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
            HideAllPortraits();
        }
    }

    private void HideAllPortraits()
    {
        // Iterate over a copy of values to avoid modification during enumeration.
        var keys = new List<string>(portraits.Keys);
        foreach (var key in keys)
        {
            if (portraits.TryGetValue(key, out SetSprites portrait))
            {
                if (portrait != null)
                {
                    portrait.Hide();
                }
                else
                {
                    // remove destroyed entries
                    portraits.Remove(key);
                }
            }
        }
    }

    private void CleanupDestroyedEntries()
    {
        var keysToRemove = new List<string>();
        foreach (var kv in portraits)
        {
            if (kv.Value == null)
                keysToRemove.Add(kv.Key);
        }

        foreach (var k in keysToRemove)
            portraits.Remove(k);
    }
}