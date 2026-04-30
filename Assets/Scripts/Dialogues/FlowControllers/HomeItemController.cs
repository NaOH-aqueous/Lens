using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeItemController : MonoBehaviour
{
    public Sprite papertowelSprite;
    public CGItem cgPlayer;
    public Sprite papertowelIcon;
    public Sprite notesSprite;

    private const string PAPERTOWEL = "PaperTowel";
    private Item papertowel;
    private bool paperGet;

    private const string NOTES = "Notes";
    private Item notes;

    private bool papertowelDisplayed = false;

    private void Start()
    {
        papertowel.item_Name = PAPERTOWEL;
        papertowel.item_Sprite = papertowelSprite;
        papertowel.item_Icon = papertowelIcon;

        notes.item_Name = NOTES;
        notes.item_Sprite = notesSprite;

    }

    private void OnEnable()
    {
        if(DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged += HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged += HandleVariableChanged;
        }
    }

    private void OnDisable()
    {
        if(DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnItemTagChanged -= HandleItemTagChanged;
            DialogueManager.Instance.OnVariableChanged -= HandleVariableChanged;
        }
    }

    private void HandleItemTagChanged(string newTag)
    {
        Debug.Log("Item tag changed to: " + newTag);
        // newTag is empty string when no item tag present
        if (string.IsNullOrEmpty(newTag))
        {
            cgPlayer.ClearDisplay();
            return;
        }

        // decide what to do based on tag name
        if (newTag == PAPERTOWEL)
        {
            cgPlayer.DisplayItemInfo(papertowel);
            InventoryManager.Instance.QueueItem(papertowel);
        }
        else
        {
            // unknown tag: clear or ignore
            cgPlayer.ClearDisplay();
        }
    }

    private void HandleVariableChanged(string name, Ink.Runtime.Object value)
    {
        if (name != "read_notes")
            return;

        if (value is Ink.Runtime.BoolValue b && b.value)
        {
            cgPlayer.InspectItem(notes);
        }
        else
        {
            cgPlayer.ClearInspect();
        }
    }
}
