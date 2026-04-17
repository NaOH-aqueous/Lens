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

    void Start()
    {
        myLight = GetComponentInChildren<Light2D>();
        StartCoroutine(Flicker());
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            float targetIntensity = Random.Range(minIntensity, maxIntensity);
            float duration = Random.Range(minDuration, maxDuration);
            float startIntensity = myLight.intensity;
            float time = 0f;

            while (time < duration)
            {
                // Smoothly lerp between intensities
                myLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            myLight.intensity = targetIntensity; // Ensure final target is set
        }
    }
}
