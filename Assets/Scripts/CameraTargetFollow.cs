using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public Transform player;

    private bool overrideMode = false;

    public void SetOverride(bool value)
    {
        overrideMode = value;
    }

    public void FocusPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    void LateUpdate()
    {
        if (overrideMode) return;

        transform.position = player.position;
    }
}
