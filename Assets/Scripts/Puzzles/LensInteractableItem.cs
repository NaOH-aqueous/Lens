using System.Collections.Generic;
using UnityEngine;

public class LensInteractableItem : MonoBehaviour
{
    public static readonly List<LensInteractableItem> All = new();

    [SerializeField] private Item lensItem;

    public Item LensItem => lensItem;

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }
}
