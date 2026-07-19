using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class SnapCinemachineOnStart : MonoBehaviour
{
    public CinemachineGroupFraming groupFraming;
    public float gameplayDamping = 0.3f; // your normal damping value

    IEnumerator Start()
    {
        // Disable damping for the very first frame
        groupFraming.Damping = 0f;

        // Let Cinemachine calculate the correct initial frame
        yield return null;

        // Restore damping for normal gameplay
        groupFraming.Damping = gameplayDamping;
    }
}
