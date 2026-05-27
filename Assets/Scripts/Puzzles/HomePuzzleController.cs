using System.Collections;
using UnityEngine;

public class HomePuzzleController : PuzzleController
{
    public CGItem cgPlayer;
    public GameObject windowPuzzle;
    public GameObject diaryPuzzle;
    public GameObject plantPuzzle;

    private CanvasGroup puzzleCanvas;

    private void Start()
    {
        windowPuzzle.SetActive(false);
        diaryPuzzle.SetActive(false);
        plantPuzzle.SetActive(false);

        puzzleCanvas = GetComponent<CanvasGroup>();
    }

    private void ExitWindowPuzzle()
    {
        windowPuzzle.SetActive(false);

        cgPlayer.ClearDisplay();
    }

    private void ExitDiary()
    {
        diaryPuzzle.SetActive(false);
    }

    private void ExitPlant()
    {
        plantPuzzle.SetActive(false);
    }

    public override void ExitAllPuzzle()
    {
        base.ExitAllPuzzle();

        if(!windowPuzzle.activeSelf && !diaryPuzzle.activeSelf)
        {
            return;
        }

        if (windowPuzzle.activeSelf)
        {
            ExitWindowPuzzle();
        }

        if (diaryPuzzle.activeSelf)
        {
            ExitDiary();
        }

        if (plantPuzzle.activeSelf)
        {
            ExitPlant();
        }

        Close();
    }
}
