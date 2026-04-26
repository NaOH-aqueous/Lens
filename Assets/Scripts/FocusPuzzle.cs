using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class FocusPuzzle : MonoBehaviour
{
    public Image image1;
    public Image image2;
    private float minAlpha = 0f; //set the alpha to 0
    private float maxAlpha = 1f; //set the alpha to 1
    private Color _color;

    void Start()
    {
        //image1 = GetComponent<Image>();
        //use this line only, when getting this gameObject's own object

        //read the current color
        //Color c = image1.color;
        
        //c.a = minAlpha;
        _color = image1.color;
    }


    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {

            //Debug.Log("alpha" + image1.color.a);
            //maxAlpha = maxAlpha - 1f * Time.deltaTime;

            //change speed
            float t = Mathf.PingPong(Time.time, 1f);
            //Ping-pong between minAlpha and maxAlpha;
            _color.a = Mathf.Lerp(minAlpha,maxAlpha,t);
            //apply new color back
            image1.color = _color;

        }
    }

}
