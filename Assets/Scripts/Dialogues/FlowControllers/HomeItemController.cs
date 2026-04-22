using UnityEngine;

public class HomeItemController : MonoBehaviour
{
    public Sprite papertowelSprite;
    public CGItem cgPlayer;
    public Sprite papertowelIcon;

    private const string PAPERTOWEL = "PaperTowel";
    private Item papertowel;

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
            if (!InventoryManager.Instance.GetItem(papertowel))
            {
                InventoryManager.Instance._AddItem(papertowel);
            }
        }
    }
}
