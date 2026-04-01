using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// During tutorial rounds, guides the player through the result screen
/// with a pointer finger that auto-scrolls through collected items,
/// then highlights the Pet button. Disables manual controls during walkthrough.
/// 
/// Setup: Place on the ResultScreen GameObject alongside Result.
/// Assign the pointer finger image, buttons, and Result reference in Inspector.
/// The pointer finger should start disabled in the scene.
/// </summary>
public class ResultWalkthrough : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Result result;
    [SerializeField] private RectTransform pointerFinger;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button petButton;
    [SerializeField] private GameObject petButtonGlow;

    [Header("Timing")]
    [SerializeField] private float initialDelay = 1.5f;
    [SerializeField] private float lingerTime = 1f;
    [SerializeField] private float pointerMoveSpeed = 5f;
    [SerializeField] private float clickBobDistance = 20f;
    [SerializeField] private float clickBobDuration = 0.15f;

    [Header("Tutorial")]
    [SerializeField] private int maxTutorialRound = 3;

    private bool walkthroughActive = false;

    private void Start()
    {
        petButtonGlow.SetActive(false);
        // Only run during tutorial rounds
        // progressionCounter was already incremented by Result.Start(),
        // so check <= maxTutorialRound to cover rounds that started at 0..maxTutorialRound-1
        if (Progression.progressionCounter <= maxTutorialRound)
        {
            StartCoroutine(RunWalkthrough());
        }
    }

    private void OnDisable()
    {
        petButtonGlow.SetActive(false);
    }

    private IEnumerator RunWalkthrough()
    {
        // Wait a frame so Result.Start() finishes and maxPan is set
        yield return null;

        int itemCount = result.MaxPan;
        if (itemCount <= 0)
        {
            yield break;
        }

        walkthroughActive = true;

        // Disable buttons during walkthrough
        rightButton.interactable = false;
        leftButton.interactable = false;
        //petButton.interactable = false;

        // Wait for results to settle on screen
        yield return new WaitForSeconds(initialDelay);

        // Show pointer at the right button
        pointerFinger.gameObject.SetActive(true);
        yield return StartCoroutine(MovePointerTo(rightButton.GetComponent<RectTransform>()));

        // Linger on the first item
        yield return new WaitForSeconds(lingerTime);

        // Click through remaining items
        for (int i = 1; i < itemCount; i++)
        {
            // Animate a click on the right button
            yield return StartCoroutine(AnimateClick());

            result.Right();

            // Wait for scroll animation + linger
            yield return new WaitForSeconds(lingerTime);
        }

        petButtonGlow.SetActive(true);

        // Move pointer to the Pet button
        yield return StartCoroutine(MovePointerTo(petButton.GetComponent<RectTransform>()));
        yield return StartCoroutine(AnimateClick());

        // Re-enable the Pet button so the player can tap it
        petButton.interactable = true;
        // Also re-enable scroll buttons in case they want to look back
        rightButton.interactable = true;
        leftButton.interactable = true;

        walkthroughActive = false;
    }

    private IEnumerator MovePointerTo(RectTransform target)
    {
        Vector3 targetPos = target.position;

        while (Vector3.Distance(pointerFinger.position, targetPos) > 1f)
        {
            pointerFinger.position = Vector3.Lerp(
                pointerFinger.position,
                targetPos,
                Time.deltaTime * pointerMoveSpeed
            );
            yield return null;
        }

        pointerFinger.position = targetPos;
    }

    private IEnumerator AnimateClick()
    {
        // Bob down
        Vector3 startPos = pointerFinger.localPosition;
        Vector3 downPos = startPos + Vector3.down * clickBobDistance;

        float elapsed = 0f;
        while (elapsed < clickBobDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / clickBobDuration;
            pointerFinger.localPosition = Vector3.Lerp(startPos, downPos, t);
            yield return null;
        }

        // Bob back up
        elapsed = 0f;
        while (elapsed < clickBobDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / clickBobDuration;
            pointerFinger.localPosition = Vector3.Lerp(downPos, startPos, t);
            yield return null;
        }

        pointerFinger.localPosition = startPos;
    }
}
