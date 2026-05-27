using System.Collections.Generic;
using UnityEngine;

public enum InputLayer
{
    Gameplay,
    UI,
    Cutscene, //blocks everything
    Dialogue, //blocks everything except confirm key
    InventoryBlock
}

public class InputRouter : MonoBehaviour
{
    public static InputRouter Instance { get; private set; }

    public InputLayer CurrentLayer => overlayStack.Count > 0 ? overlayStack.Peek(): rootLayer;

    private InputLayer rootLayer = InputLayer.Gameplay;
    private Stack<InputLayer> overlayStack = new();

    private InputLayer previousLayer;

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

    public void PopLayer()
    {
        if (overlayStack.Count > 0)
            overlayStack.Pop();
        Debug.Log("poped input -> now:" + CurrentLayer);
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
        return CurrentLayer == InputLayer.Dialogue||
               CurrentLayer == InputLayer.UI;
    }

    public bool AllowsInventoryInput()
    {
        return CurrentLayer != InputLayer.InventoryBlock;
    }


}