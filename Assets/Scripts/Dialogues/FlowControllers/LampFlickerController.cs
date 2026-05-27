using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampFlickerController : MonoBehaviour
{
    private Light2D myLight;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    public float minDuration = 0.05f;
    public float maxDuration = 0.2f;
    public float updateInterval = 0f;

    private Coroutine flickerCoroutine;

    void Start()
    {
        myLight = GetComponentInChildren<Light2D>();
        if (myLight == null)
        {
            Debug.LogWarning($"{nameof(LampFlickerController)}: no Light2D found in children on '{gameObject.name}'");
            return;
        }

        flickerCoroutine = StartCoroutine(Flicker());
    }

    void OnDisable()
    {
        if (flickerCoroutine != null)
            StopCoroutine(flickerCoroutine);
        flickerCoroutine = null;
    }

    private IEnumerator Flicker()
    {
        WaitForSeconds wait = updateInterval > 0f ? new WaitForSeconds(updateInterval) : null;

        while (true)
        {
            if (myLight == null) yield break;

            float targetIntensity = Random.Range(minIntensity, maxIntensity);
            float duration = Mathf.Max(0.0001f, Random.Range(minDuration, maxDuration));
            float startIntensity = myLight.intensity;
            float time = 0f;

            while (time < duration)
            {
                // Smoothly lerp between intensities
                myLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, time / duration);
                time += (wait != null) ? updateInterval : Time.deltaTime;
                if (wait != null)
                    yield return wait;
                else
                    yield return null;
            }

            myLight.intensity = targetIntensity;
        }
    }
}
