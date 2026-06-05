using UnityEngine;

public class LensController : MonoBehaviour
{
    public virtual void EnableLens()
    {
        GameManager.instance.PushState(GameStateType.Lens);
    }

    public virtual void DisableLens()
    {
        GameManager.instance.PopState(GameStateType.Lens);
    }
}
