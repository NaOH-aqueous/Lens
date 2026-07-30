using System.Collections.Generic;
using UnityEngine;

public enum InputLayer
{
    Gameplay,
    UI,
    Cutscene, //blocks everything
    InventoryBlock
}

public class InputRouter : MonoBehaviour
{
    public static InputRouter Instance { get; private set; }

    private InputLayer rootLayer = InputLayer.Gameplay;
    private Stack<InputLayer> overlayStack = new();

    public InputLayer CurrentLayer
    {
        get
        {
            if (overlayStack.Count > 0)
                return overlayStack.Peek();

            return GameManager.instance.CurrentState switch
            {
                GameStateType.Playing => InputLayer.Gameplay,
                GameStateType.Inventory => InputLayer.UI,
                GameStateType.ItemDisplay => InputLayer.UI,
                GameStateType.Puzzle => InputLayer.InventoryBlock,
                GameStateType.Cutscene => InputLayer.Cutscene,
                GameStateType.Lens => InputLayer.InventoryBlock,
                _ => InputLayer.Gameplay
            };
        }
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PushLayer(InputLayer layer)
    {
        overlayStack.Push(layer);
        Debug.Log("pushed input:" + layer);
    }

    public void PopLayer(InputLayer expected)
    {
        if (overlayStack.Count == 0)
        {
            Debug.LogWarning("stack is empty");
            return;
        }

        if (overlayStack.Peek() != expected)
        {
            Debug.LogWarning(
                $"PopLayer mismatch! Expected {expected} but top is {overlayStack.Peek()}"
            );
            return;
        }

        overlayStack.Pop();
        Debug.Log("popped input -> now: " + CurrentLayer);
    }

    public void SetRootLayer(InputLayer layer)
    {
        rootLayer = layer;
    }

    public bool AllowsGameplayInput()
    {
        return CurrentLayer == InputLayer.Gameplay;
    }

    public bool BlocksEverything()
    {
        return CurrentLayer == InputLayer.Cutscene;
    }

    public bool AllowsUIInput()
    {
        return CurrentLayer == InputLayer.UI;

    }

    public bool AllowsInventoryInput()
    {
        return CurrentLayer != InputLayer.InventoryBlock;
    }

    public void Clear()
    {
        overlayStack.Clear();
    }
}