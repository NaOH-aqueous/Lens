using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditorInternal.VersionControl.ListControl;

public class BlurEffect : MonoBehaviour
{
    private Volume blurVFX;
    private Animator _anim;
    private GameStateType gameState;


    private void Start()
    {
        blurVFX = GetComponent<Volume>();
        _anim = GetComponent<Animator>();
    }

    public IEnumerator IntroTransition()
    {
        _anim.SetTrigger("Reveal");
        while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Reveal"))
            yield return null;
        blurVFX.enabled = false;
    }

    public IEnumerator OutroTransition()
    {
        blurVFX.enabled = true;
        _anim.SetTrigger("Blur");
        while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Blur"))
            yield return null;
    }
}
