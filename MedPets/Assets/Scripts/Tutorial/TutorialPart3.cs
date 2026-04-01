using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPart3 : MonoBehaviour
{
    private enum State
    {
        Inactive,
        Intro,
        InventoryExplain,
        WaitForInventoryOpen,
        MedPrompt,
        WaitForMed,
        MedResult,
        WaitForFood,
        FoodResult,
        CloseInventory,
        WrapUp,
        Congrats,
        Done
    }

    [Header("UI")]
    [SerializeField] private GameObject overlayRoot;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject fingerPointer;

    [Header("Scene References")]
    [SerializeField] private PetController petController;
    [SerializeField] private InventoryButton inventoryButton;
    [SerializeField] private Inventory inventory;
    [SerializeField] private RectTransform inventoryIconTarget;
    [SerializeField] private RectTransform backButtonTarget;

    [Header("Buttons to Disable During Tutorial")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button guidebookButton;

    [Header("Finger Animation")]
    [SerializeField] private float bobDistance = 15f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] private float fingerOffsetY = 80f;
    [SerializeField] private float fingerMoveDuration = 0.8f;

    [Header("Glow")]
    [SerializeField] private GameObject inventoryGlow;
    [SerializeField] private float pulseSpeed = 3f;

    // State
    [SerializeField] private State currentState = State.Inactive;
    private int partStartRow;
    private bool medSucceeded;
    private bool foodSucceeded;
    private bool fingerBobbing;
    private bool fingerMoving;
    private Vector2 fingerBasePos;
    private CanvasGroup overlayCanvasGroup;

    // ===== LIFECYCLE =====

    private void Start()
    {
        GameState gs = GameState.Instance;

        if (gs == null || gs.tutorialStep != 3)
        {
            if (overlayRoot != null && gs.tutorialStep != 1) overlayRoot.SetActive(false);
            currentState = State.Inactive;
            enabled = false;
            return;
        }

        if (gs.tutorialPartStarts == null || gs.tutorialPartStarts.Length < 3 || gs.script == null)
        {
            Debug.LogWarning("TutorialPart3: CSV part data not found.");
            if (overlayRoot != null) overlayRoot.SetActive(false);
            currentState = State.Inactive;
            enabled = false;
            return;
        }

        partStartRow = gs.tutorialPartStarts[2];

        // Ensure CanvasGroup exists for raycast toggling
        overlayCanvasGroup = overlayRoot.GetComponent<CanvasGroup>();
        if (overlayCanvasGroup == null)
            overlayCanvasGroup = overlayRoot.AddComponent<CanvasGroup>();

        fingerPointer.SetActive(false);
        if (inventoryGlow != null) inventoryGlow.SetActive(false);

        if (playButton != null) playButton.interactable = false;
        if (guidebookButton != null) guidebookButton.interactable = false;

        ConsumableUser.OnItemUsed += OnItemUsed;

        EnterState(State.Intro);
    }

    private void OnDestroy()
    {
        ConsumableUser.OnItemUsed -= OnItemUsed;
    }

    private void Update()
    {
        // Finger bob animation
        if (fingerBobbing && fingerPointer.activeSelf && !fingerMoving)
        {
            RectTransform rt = fingerPointer.GetComponent<RectTransform>();
            float offset = Mathf.Sin(Time.time * bobSpeed) * bobDistance;
            rt.anchoredPosition = fingerBasePos + new Vector2(0, offset);
        }

        PulseGlow(inventoryGlow);

        // Handle tap-to-advance for dialogue states
        if (Input.GetMouseButtonDown(0))
        {
            switch (currentState)
            {
                case State.Intro:
                    EnterState(State.InventoryExplain);
                    break;
                case State.InventoryExplain:
                    EnterState(State.WaitForInventoryOpen);
                    break;
                case State.MedPrompt:
                    EnterState(State.WaitForMed);
                    break;
                case State.MedResult:
                    if (medSucceeded)
                        EnterState(State.WaitForFood);
                    else
                        EnterState(State.WaitForMed); // retry
                    break;
                case State.FoodResult:
                    if (foodSucceeded)
                        EnterState(State.CloseInventory);
                    else
                        EnterState(State.WaitForFood); // retry
                    break;
                case State.WrapUp:
                    EnterState(State.Congrats);
                    break;
                case State.Congrats:
                    EnterState(State.Done);
                    break;
            }
        }

        // Check if inventory closed during CloseInventory state
        if (currentState == State.CloseInventory)
        {
            if (inventoryButton != null && !inventoryButton.anim.GetBool("isIn"))
            {
                EnterState(State.WrapUp);
            }
        }
    }

    // ===== STATE MACHINE =====

    private void EnterState(State newState)
    {
        currentState = newState;
        Debug.Log($"[TutorialPart3] → {newState}");

        switch (newState)
        {
            case State.Intro:
                overlayRoot.SetActive(true);
                SetOverlayBlocksRaycasts(true);
                ShowLine(0);
                HideFinger();
                break;

            case State.InventoryExplain:
                SetOverlayBlocksRaycasts(true);
                ShowLine(1);
                break;

            case State.WaitForInventoryOpen:
                dialoguePanel.SetActive(false);
                SetOverlayBlocksRaycasts(false); // let taps reach inventory button
                if (inventoryGlow != null) inventoryGlow.SetActive(true);
                HideFinger();
                break;

            case State.MedPrompt:
                if (inventoryGlow != null) inventoryGlow.SetActive(false);
                SetOverlayBlocksRaycasts(true); // block taps for dialogue
                ShowLine(2); 
                HideFinger();
                break;

            case State.WaitForMed:
                dialoguePanel.SetActive(false);
                SetOverlayBlocksRaycasts(false); // let taps reach items
                PointFingerAtItem(true); // find and point at medication
                break;

            case State.MedResult:
                SetOverlayBlocksRaycasts(true);
                HideFinger();
                if (medSucceeded)
                {
                    ShowLine(3);
                    if (petController != null) petController.Happy();
                }
                else
                {
                    ShowLine(4); 
                }
                break;

            case State.WaitForFood:
                dialoguePanel.SetActive(false);
                SetOverlayBlocksRaycasts(false);
                PointFingerAtItem(false); // find and point at food
                break;

            case State.FoodResult:
                SetOverlayBlocksRaycasts(true);
                HideFinger();
                if (foodSucceeded)
                {
                    ShowLine(5);
                    if (petController != null) petController.Happy();
                }
                else
                {
                    ShowLine(6);
                }
                break;

            case State.CloseInventory:
                dialoguePanel.SetActive(false);
                SetOverlayBlocksRaycasts(false);
                PointFingerAt(backButtonTarget);
                break;

            case State.WrapUp:
                SetOverlayBlocksRaycasts(true);
                HideFinger();
                ShowLine(7);
                break;

            case State.Congrats:
                ShowLine(8);
                if (petController != null) petController.Happy();
                break;

            case State.Done:
                FinishTutorial();
                break;
        }
    }

    // ===== ITEM USED EVENT =====

    private void OnItemUsed(Berry berry)
    {
        if (berry == null) return;

        // Dismiss new item popup immediately so it doesn't block
        if (inventory != null && inventory.newItemPopup.activeSelf)
        {
            inventory.CloseNewItemPopup();
        }
        
        // Reset flag
        berry.data.hasBeenUsed = false;

        // Give the item back so the player can't run out during tutorial
        GameState.Instance.items[berry.Id]++;
        SaveSystem.SavePet();

        bool isMed = IsMedication(berry);
        Debug.Log($"[TutorialPart3] Item used: {berry.DataName}, isMed={isMed}, state={currentState}");

        if (currentState == State.WaitForMed)
        {
            medSucceeded = isMed;
            EnterState(State.MedResult);
        }
        else if (currentState == State.WaitForFood)
        {
            foodSucceeded = !isMed;
            EnterState(State.FoodResult);
        }
    }

    // ===== INVENTORY OPEN DETECTION =====

    public void OnInventoryOpened()
    {
        if(GameState.Instance.tutorialStep != 3) return; // ignore if somehow triggered outside of this tutorial
        if (currentState == State.WaitForInventoryOpen)
        {
            // small delay to let inventory animation play
            StartCoroutine(DelayedEnterState(State.MedPrompt, 0.8f));
        }
    }

    private IEnumerator DelayedEnterState(State state, float delay)
    {
        yield return new WaitForSeconds(delay);
        EnterState(state);
    }

    // ===== FINISH =====

    private void FinishTutorial()
    {
        GameState gs = GameState.Instance;
        gs.tutorialStep = 4;
        SaveSystem.SavePet();

        if (playButton != null) playButton.interactable = true;
        if (guidebookButton != null) guidebookButton.interactable = true;

        HideFinger();
        dialoguePanel.SetActive(false);
        overlayRoot.SetActive(false);
        enabled = false;
    }

    // ===== FINGER HELPERS =====

    private void PointFingerAtItem(bool wantMedication)
    {
        RectTransform target = FindInventoryItem(wantMedication);
        if (target != null)
        {
            Debug.Log($"[TutorialPart3] Found {(wantMedication ? "medication" : "food")} item: {target.name}");
            PointFingerAt(target);
        }
        else
        {
            Debug.LogWarning($"[TutorialPart3] Could not find {(wantMedication ? "medication" : "food")} in inventory!");
            HideFinger();
        }
    }

    private void PointFingerAt(RectTransform target)
    {
        fingerPointer.SetActive(true);
        fingerBobbing = false;
        fingerMoving = false;
        StartCoroutine(MoveFingerCoroutine(target));
    }

    private void HideFinger()
    {
        fingerPointer.SetActive(false);
        fingerBobbing = false;
        fingerMoving = false;
    }

    // ===== FIND INVENTORY ITEMS =====

    private RectTransform FindInventoryItem(bool wantMedication)
    {
        if (inventory == null)
        {
            Debug.LogWarning("[TutorialPart3] Inventory reference is null!");
            return null;
        }

        Debug.Log($"[TutorialPart3] Scanning {inventory.transform.childCount} inventory children...");

        foreach (Transform child in inventory.transform)
        {
            ConsumableUser consumable = child.GetComponent<ConsumableUser>();
            if (consumable == null)
            {
                Debug.Log($"  Child '{child.name}' — no ConsumableUser, skipping");
                continue;
            }
            if (consumable.item == null)
            {
                Debug.Log($"  Child '{child.name}' — ConsumableUser.item is null, skipping");
                continue;
            }

            Berry berry = consumable.item.GetComponent<Berry>();
            if (berry == null || berry.Tags == null || berry.Tags.Length == 0)
            {
                Debug.Log($"  Child '{child.name}' — no Berry or no Tags, skipping");
                continue;
            }

            bool isMed = berry.Tags[0].Equals("Medication", StringComparison.OrdinalIgnoreCase);
            Debug.Log($"  Child '{child.name}' — tag='{berry.Tags[0]}', isMed={isMed}, want={wantMedication}");

            if (wantMedication == isMed)
            {
                return child.GetComponent<RectTransform>();
            }
        }

        Debug.LogWarning($"[TutorialPart3] No matching item found (wantMedication={wantMedication})");
        return null;
    }

    // ===== DIALOGUE =====

    private void ShowLine(int lineIndex)
    {
        GameState gs = GameState.Instance;
        int row = partStartRow + lineIndex;

        string line = "";
        if (row < gs.script.GetLength(0))
        {
            line = gs.script[row, gs.language];
        }

        // Strip [Success] / [Fail] prefixes
        if (line.StartsWith("[Success]", StringComparison.OrdinalIgnoreCase))
            line = line.Substring("[Success]".Length).TrimStart();
        else if (line.StartsWith("[Fail]", StringComparison.OrdinalIgnoreCase))
            line = line.Substring("[Fail]".Length).TrimStart();

        // Replace pet name
        if (!string.IsNullOrEmpty(gs.petName))
        {
            line = line.Replace("Fido", gs.petName);
            line = line.Replace("PET NAME", gs.petName);
            line = line.Replace("PET NAMES", gs.petName + "'s");
        }

        dialogueText.text = line;
        dialoguePanel.SetActive(true);
    }

    // ===== RAYCAST CONTROL =====

    private void SetOverlayBlocksRaycasts(bool blocks)
    {
        if (overlayCanvasGroup != null)
        {
            overlayCanvasGroup.blocksRaycasts = blocks;
        }
    }

    // ===== FINGER ANIMATION =====

    private IEnumerator MoveFingerCoroutine(RectTransform target)
    {
        fingerMoving = true;
        RectTransform fingerRT = fingerPointer.GetComponent<RectTransform>();
        RectTransform canvasRT = overlayRoot.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 startPos = fingerRT.anchoredPosition;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, null, out Vector2 targetPos);
        targetPos += new Vector2(0, fingerOffsetY);

        // Arc movement
        Vector2 mid = (startPos + targetPos) * 0.5f;
        Vector2 perp = new Vector2(-(targetPos.y - startPos.y), targetPos.x - startPos.x).normalized;
        float arcHeight = Vector2.Distance(startPos, targetPos) * 0.3f;
        Vector2 control = mid + perp * arcHeight;

        float elapsed = 0f;
        while (elapsed < fingerMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - (elapsed / fingerMoveDuration), 2.5f);
            float omt = 1f - t;
            fingerRT.anchoredPosition = omt * omt * startPos + 2f * omt * t * control + t * t * targetPos;
            yield return null;
        }

        fingerRT.anchoredPosition = targetPos;
        fingerBasePos = targetPos;
        fingerMoving = false;
        fingerBobbing = true;
    }

    // ===== GLOW =====

    private void PulseGlow(GameObject glow)
    {
        if (glow == null || !glow.activeSelf) return;
        Image img = glow.GetComponent<Image>();
        if (img == null) return;

        float alpha = (125f + 50f * Mathf.Sin(Time.time * pulseSpeed)) / 255f;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private bool IsMedication(Berry berry)
    {
        if (berry == null || berry.Tags == null || berry.Tags.Length == 0) return false;
        return berry.Tags[0].Equals("Medication", StringComparison.OrdinalIgnoreCase);
    }
}