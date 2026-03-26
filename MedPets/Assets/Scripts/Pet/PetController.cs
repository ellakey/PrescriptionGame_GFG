using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PetController : MonoBehaviour
{
    public Animator petAnimator;

    [Header("Sprite Swapping (optional)")]
    public SpriteRenderer petSprite;
    public Sprite happySprite;
    public Sprite hungrySprite;

    [Header("Tap Reaction")]
    public float happyDuration = 1.5f;

    private Coroutine tapRoutine;

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

    public void ClickPet()
    {
        Debug.Log("Pet clicked!");
        if (tapRoutine != null)
            StopCoroutine(tapRoutine);
        tapRoutine = StartCoroutine(TapReaction());
    }

    private IEnumerator TapReaction()
    {
        Happy();
        yield return new WaitForSeconds(happyDuration);

        // Let NeedsController pick the correct state on the next frame
        // by re-triggering based on current needs
        tapRoutine = null;
    }

    public void Test()
    {
        Debug.Log("Test method called on PetController!");
    }
}