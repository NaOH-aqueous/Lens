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

    private void Start()
    {
        lamp_renderer = GetComponent<SpriteRenderer>();
        lamp_Light = GetComponentInChildren<Light2D>();

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnVariableChanged += HandleVariableChanged;
        }
    }

    private void OnDisable()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.OnVariableChanged -= HandleVariableChanged;
        }
    }

    private void lampOn()
    {
        lamp_renderer.material = lamp_Emission;
        lamp_renderer.sprite = lamp_Lighten;
        lamp_Light.enabled = true;
    }

    private void lampOff()
    {
        lamp_renderer.material = lamp_Normal;
        lamp_renderer.sprite = lamp_Darken;
        lamp_Light.enabled = false;
    }

    private void HandleVariableChanged(string name, Ink.Runtime.Object value)
    {
        if (name != "lamp_switch")
            return;

        if (value is Ink.Runtime.BoolValue boolVal)
        {
            if (boolVal.value)
                lampOn();
            else
                lampOff();
        }
    }
}
