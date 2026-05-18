using UnityEngine;

public interface IItemUseRule
{
	bool CanUse(Item inventoryItem, Item worldItem);
    void Apply(Item inventoryItem, Item worldItem);
}

public class PaperTowelOnWindowRule : IItemUseRule
{
    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "PaperTowel"
            && worldItem.item_Name == "Window";
    }

    public void Apply(Item inventoryItem, Item worldItem)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.SetWindowClean();
    }
}