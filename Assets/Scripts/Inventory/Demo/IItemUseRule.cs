using JetBrains.Annotations;
using NUnit.Framework.Internal.Execution;
using UnityEngine;

public interface IItemUseRule
{
    bool CanUse(
        Item inventoryItem,
        Item worldItem = null
    );

    void Apply(
        Item inventoryItem,
        Item worldItem = null
    );

    bool ConsumeItem { get; }
}

public class PaperTowelOnWindowRule : IItemUseRule
{
    public bool ConsumeItem => true;
    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "paper towel"
            && worldItem?.item_Name == "Window";
    }

    public void Apply(Item inventoryItem, Item worldItem)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.SetWindowClean();
    }
}

public class FoldPaperRule : IItemUseRule
{
    public bool ConsumeItem => true;
    public bool CanUse(Item inventoryItem,Item worldItem = null)
    {
        return inventoryItem.item_Name == "stack of paper";
    }

    public void Apply(Item inventoryItem, Item worldItem = null)
    {
        {
            var home = GameObject.FindFirstObjectByType<HomeItemController>();
            home.FoldOrigamiFlower();
        }
    }
}

public class PlantFlowerRule : IItemUseRule
{
    public bool ConsumeItem => false;

    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "origami flower"
            && worldItem?.item_Name == "planter";
    }

    public void Apply(Item inventoryItem, Item worldItem = null)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.PlantFlower();
    }
}

public class ShovelOnPlanterRule : IItemUseRule
{
    public bool ConsumeItem => false;
    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "shovel"
            && worldItem?.item_Name == "planter";
    }

    public void Apply(Item inventoryItem, Item worldItem = null)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.PlantFlower();
    }
}

public class MagnifierOnWall : IItemUseRule
{
    public bool ConsumeItem => false;
    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "Magnifier"
            && worldItem?.item_Name == "corner of the room";
    }

    public void Apply(Item inventoryItem, Item worldItem = null)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.MagnifyCorner();
    }
}

public class FruitOnWall : IItemUseRule
{
    public bool ConsumeItem => true;
    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "strange fruit"
            && worldItem?.item_Name == "corner of the room";
    }
    public void Apply(Item inventoryItem, Item worldItem = null)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.HandleFruitFed();
    }

}

public class MagnifierOnWindow: IItemUseRule
{
    public bool ConsumeItem => false;
    public bool CanUse(Item inventoryItem, Item worldItem)
    {
        return inventoryItem.item_Name == "Magnifier"
            && worldItem?.item_Name == "Window";
    }

    public void Apply(Item inventoryItem, Item worldItem = null)
    {
        var home = GameObject.FindFirstObjectByType<HomeItemController>();
        home.MagnifyWindow();
    }
}

