using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimControl : MonoBehaviour
{
    Animation anim;
    //Drag and drop your Slider into this variable.
    public Slider slider;
    // Use this for initialization
    void Start()
        {
        anim = GetComponent<Animation>();
        //Make sure you have attached your animation in the Animations attribute
        anim.Play("SunPath");
        anim["SunPath"].speed = 0;
        }
    void Update()
        {
        anim["SunPath"].normalizedTime = slider.value;
        }
}