using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class HomeItemController : MonoBehaviour
{
    public Sprite papertowelSprite;
    public CGItem cgPlayer;
    public Sprite papertowelIcon;

    private const string PAPERTOWEL = "PaperTowel";
    private Item papertowel;
    private bool paperGet;

    private void Start()
    {
        papertowel.item_Name = PAPERTOWEL;
        papertowel.item_Sprite = papertowelSprite;
        papertowel.item_Icon = papertowelIcon;

    }
    private void Update()
    {
        cgPlayer.DisplayItemInfo(papertowel);

        bool have_paper = ((Ink.Runtime.BoolValue)DialogueManager.Instance.
    GetVariableState("have_paper")).value;

        if (have_paper)
        {
            cgPlayer.ClearDisplay();
            if (!paperGet)
            {
                paperGet = true;
                InventoryManager.Instance.QueueItem(papertowel);
            }
        }

    }
}
