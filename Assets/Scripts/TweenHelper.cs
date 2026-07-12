using UnityEngine;
using DG.Tweening;

public static class TweenHelper
{
    // Kills any existing tweens on the target CanvasGroup
    public static Tween FadeCanvasGroup(CanvasGroup canvas, float targetAlpha, float duration, bool unscaled = true, Ease ease = Ease.OutQuad)
    {
        if (canvas == null) return null;
        DOTween.Kill(canvas);

        // Use DOTween.To on the CanvasGroup alpha to avoid depending on extension methods.
        var tween = DOTween.To(() => canvas.alpha, x => canvas.alpha = x, targetAlpha, duration)
            .SetEase(ease)
            .SetUpdate(unscaled)
            .SetTarget(canvas);

        tween.OnStart(() =>
        {
            canvas.blocksRaycasts = true;
            canvas.interactable = false;
        });

        tween.OnComplete(() =>
        {
            canvas.blocksRaycasts = targetAlpha > 0f;
            canvas.interactable = targetAlpha > 0f;
        });

        return tween;
    }

    // Cancel any tweens running on the CanvasGroup
    public static void CancelFade(CanvasGroup cg)
    {
        if (cg == null) return;
        DOTween.Kill(cg);
    }
}
