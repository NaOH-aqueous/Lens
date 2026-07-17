using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public CinemachineTargetGroup targetGroup;
    public CinemachineGroupFraming camFrame;
    public Transform player;

    private void Awake()
    {
        Instance = this;
    }
    public void ChangeWeight(int newWeight, Transform targetToUpdate)
    {
        var targets = targetGroup.Targets;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].Object == targetToUpdate)
            {
                // Create a temporary struct to modify the weight
                var currentTarget = targets[i];
                currentTarget.Weight = newWeight;
                targets[i] = currentTarget;

                targetGroup.Targets = targets;
                break;
            }
        }

    }

    public void ChangeFrameSize(float targetFramingSize)
    {
        camFrame.FramingSize = targetFramingSize;
    }

    public void ChangeDamping(int damping)
    {
        camFrame.Damping = damping;
    }

    public void ChangePlayerWeight(int newWeight)
    {
        ChangeWeight(newWeight, player);
    }
}

