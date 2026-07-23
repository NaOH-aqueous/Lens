using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Transform player;
    [SerializeField] private CameraTargetFollow target;
    [SerializeField] private CinemachineCamera cineCam;

    private void Awake()
    {
        Instance = this;
    }

    public void FocusOn(Transform focus)
    {
        target.SetOverride(true);
        target.FocusPosition(focus.position);
    }

    public void ReturnToPlayer()
    {
        target.SetOverride(false);
    }


    public IEnumerator Zoom(float targetSize, float duration)
    {
        float startSize = cineCam.Lens.OrthographicSize;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            cineCam.Lens.OrthographicSize =
                Mathf.Lerp(startSize, targetSize, t / duration);

            yield return null;
        }

        cineCam.Lens.OrthographicSize = targetSize;
    }
}

