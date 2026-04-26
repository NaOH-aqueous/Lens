using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampLightingController : MonoBehaviour
{
    [SerializeField] private Sprite lamp_Darken;
    [SerializeField] private Sprite lamp_Lighten;
    [SerializeField] private Material lamp_Normal;
    [SerializeField] private Material lamp_Emission;

    private Light2D lamp_Light;
    private SpriteRenderer lamp_renderer;
    private bool _isLampOn;

    private void Start()
    {
        lamp_renderer = GetComponent<SpriteRenderer>();
        lamp_Light = GetComponentInChildren<Light2D>();

        lampOn();
    }

    private void Update()
    {
        bool lamp_On = ((Ink.Runtime.BoolValue)DialogueManager.Instance.
            GetVariableState("lamp_switch")).value;

        if (lamp_On && !_isLampOn)
        {
            lampOn();
        }
        else if (!lamp_On && _isLampOn)
        {
            lampOff();
        }
    }

    private void lampOn()
    {
        lamp_renderer.material = lamp_Emission;
        lamp_renderer.sprite = lamp_Lighten;
        lamp_Light.enabled = true;
        _isLampOn = true;
    }

    private void lampOff()
    {
        lamp_renderer.material = lamp_Normal;
        lamp_renderer.sprite = lamp_Darken;
        lamp_Light.enabled = false;
        _isLampOn = false;
    }
}
