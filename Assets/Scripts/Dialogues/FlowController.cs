using UnityEngine;

public abstract class FlowController: MonoBehaviour
{
    public bool IsReady { get; private set; }

    public virtual void SetReady()
    {
        IsReady = true;
    }

    public virtual void SetNotReady()
    {
        IsReady = false;
    }
}