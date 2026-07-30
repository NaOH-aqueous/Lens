using Ink.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForestItemController : FlowController, IDialogueFunctionBinder
{
    private void Start()
    {
        base.SetReady();
    }

    private void GoHome()
    {
        StartCoroutine(GoHomeCoroutine());
    }

    private IEnumerator GoHomeCoroutine()
    {
        yield return null;
        SceneTransitionManager.Instance.LoadScene("Home");
    }

    public void BindFunctions(Story story)
    {
        story.BindExternalFunction("GoHome",
            () => GoHome());
    }
}
