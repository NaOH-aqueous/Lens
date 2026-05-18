using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CurtainLightingController : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite window_Closed;
    [SerializeField] private Sprite window_Open;
    [SerializeField] private Sprite bed_darken;
    [SerializeField] private Sprite bed_brighten;

    [Header("Material")]
    [SerializeField] private Material window_Default;
    [SerializeField] private Material window_Emission;
    [SerializeField] private Material mirror_Default;
    [SerializeField] private Material mirror_Brighten;

    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer windowRenderer;
    [SerializeField] private SpriteRenderer bedRenderer;
    [SerializeField] private SpriteRenderer mirrorRenderer;

    [Header("2D Light")]
    [SerializeField] private Light2D curtain_light;
    [SerializeField] private GameObject floor_Light;
    [SerializeField] private ParticleSystem dust;

    private bool lightingPerformed = false;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!lightingPerformed)
        {
            CurtainOpened();
        }
    }

    private void Init() //initiate the sprites, lighting and materials
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

    //change the sprites, materials & lighting after opening the curtain
    private void CurtainOpened()
    {
        bool curtain_open = ((Ink.Runtime.BoolValue)DialogueManager.Instance.
            GetVariableState("curtain_open")).value;

        if (curtain_open)
        {
            windowRenderer.sprite = window_Open;
            windowRenderer.material = window_Emission;
            mirrorRenderer.material = mirror_Brighten;
            bedRenderer.sprite = bed_brighten;
            dust.Play();

            //enable the lighting
            curtain_light.enabled = true;
            floor_Light.SetActive(true);

            lightingPerformed = true;
        }
    }
}
