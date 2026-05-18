using System.Collections;
using UnityEngine;

public class HomePuzzleController : PuzzleController
{
    public CGItem cgPlayer;
    public GameObject windowPuzzle;
    public GameObject diaryPuzzle;



    private void Start()
    {
        windowPuzzle.SetActive(false);
        diaryPuzzle.SetActive(false);
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

        Close();
    }
}
