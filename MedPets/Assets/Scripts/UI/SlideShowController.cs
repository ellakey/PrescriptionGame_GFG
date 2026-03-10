using UnityEngine;
using UnityEngine.UI;

public class SlideshowController : MonoBehaviour
{
    public Sprite[] slides;
    public Image slideImage;
    //public Toggle[] toggles;
    public int index; 

    private void Start()
    {
        // Set the initial slide
        SetSlide();
        //index = 0; 
    }

    public void SetSlide()
    {
        index += 1;
        if (index >= slides.Length) //check to see if needs to restart!
        {
            index = 0;
        }

        slideImage.sprite = slides[index];
    }
}