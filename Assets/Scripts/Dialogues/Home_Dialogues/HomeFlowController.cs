using Ink.Parsed;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HomeFlowController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite window_Closed;
    [SerializeField] private Sprite window_Open;
    [SerializeField] private Sprite bed_darken;
    [SerializeField] private Sprite bed_brighten;

    [Header("Material")]
    public Material window_Default;
    public Material window_Emission;
    [SerializeField] private Material mirror_Default;
    [SerializeField] private Material mirror_Brighten;

    [Header("Sprite Renderer")]
    public SpriteRenderer windowRenderer;
    [SerializeField] private SpriteRenderer bedRenderer;
    [SerializeField] private SpriteRenderer mirrorRenderer;

    [Header("2D Light")]
    public Light2D curtain_light;
    public GameObject floor_Light;
    public ParticleSystem dust;

    private void Start()
    {
        windowRenderer.sprite = window_Closed;
        windowRenderer.material = window_Default;
        mirrorRenderer.material = mirror_Default;
        bedRenderer.sprite = bed_darken;
        dust.Stop();

        //disabled the lighting
        curtain_light.enabled = false;
        floor_Light.SetActive(false);
    }

    private void Update()
    {
        bool curtain_open = ((Ink.Runtime.BoolValue)DialogueManager.Instance.
            GetVariableState("curtain_open")).value;
        Debug.Log("Curtain" + curtain_open);

        if (curtain_open)
        {
            windowRenderer.sprite = window_Open;
            windowRenderer.material = window_Emission;
            mirrorRenderer.material = mirror_Brighten;
            bedRenderer.sprite = bed_brighten;
            dust.Play();

            //enable the lighting
            curtain_light.enabled = true;
            floor_Light.SetActive (true);
        }
    }
}
