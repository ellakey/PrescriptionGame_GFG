using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public Animator petAnimator;

    [Header("Sprite Swapping (optional)")]
    public SpriteRenderer petSprite;
    public Sprite happySprite;
    public Sprite hungrySprite;
 
    public void Bored()
    {
        petAnimator.SetTrigger("Bored");
    }
    public void Happy()
    {
        petAnimator.SetTrigger("Happy");
    }

    public void Sad()
    {
        petAnimator.SetTrigger("Sad");
    }

    public void Hungry()
    {
        petAnimator.SetTrigger("Hungry");
    }
    public void LowBloodSugar()
    {
        petAnimator.SetTrigger("LowBloodSugar");
    }
    public void HighBloodSugar()
    {
        petAnimator.SetTrigger("HighBloodSugar");
    }

    public void UpdateSprite(float food)
    {
        if (petSprite == null) return;
        petSprite.sprite = (food >= 50) ? happySprite : hungrySprite;
    }
}
