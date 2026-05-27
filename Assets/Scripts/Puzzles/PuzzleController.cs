using System.Collections;
using UnityEngine;

public abstract class PuzzleController : MonoBehaviour
{
    public bool IsOpen { get; private set; }



    public virtual void Open()
    {
        IsOpen = true;
        GameManager.instance.PushState(GameStateType.Puzzle);
    }

    public virtual void Close()
    {
        IsOpen = false;
        GameManager.instance.PopState(GameStateType.Puzzle);
        InputManager.Instance.RegisterInteractPressed();
        InputManager.Instance.RegisterSubmitPressed();
    }

    public virtual void ExitAllPuzzle()
    {

    }
}
