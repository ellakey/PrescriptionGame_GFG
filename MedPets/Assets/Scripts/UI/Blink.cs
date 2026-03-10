using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    public float blinkSpeed = 0.1f;
    public float maxOffset = 0;
    public Image selectedImage;

    private float current = -1;

    // Update is called once per frame
    void Update()
    {
        if(current >= 2 * Mathf.PI)
        {
            current = 0;
        }

        current += blinkSpeed;

        selectedImage.color = new Color(selectedImage.color.r, selectedImage.color.g, selectedImage.color.b, Mathf.Cos(current) - maxOffset);
    }
}
