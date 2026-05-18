using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BlurEffect : MonoBehaviour
{
    private Volume blurVFX;
    private Animator _anim;


    private void Start()
    {
        blurVFX = GetComponent<Volume>();
        _anim = GetComponent<Animator>();
    }

    public IEnumerator IntroTransition()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Outro"))
        {
            yield break;
        }

        _anim.SetTrigger("Reveal");
        while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Outro"))
            yield return null;
        blurVFX.enabled = false;
    }

    public IEnumerator OutroTransition()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Intro"))
        {
            yield break;
        }
        blurVFX.enabled = true;
        _anim.SetTrigger("Blur");
        while (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Intro"))
            yield return null;
    }
}
