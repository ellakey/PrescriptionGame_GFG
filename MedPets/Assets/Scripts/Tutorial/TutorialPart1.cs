using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPart1 : MonoBehaviour
{
    [Header("UI")]
    public GameObject overlayRoot;
    [Tooltip("The dialogue box panel (gets hidden when finger shows). " +
             "Finger must NOT be a child of this.")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    [Tooltip("Finger image — must be a child of overlayRoot, NOT dialoguePanel.")]
    public GameObject fingerPointer;

    [Header("Scene References")]
    public PetController petController;
    public GameObject bloodSugarGlow;
    public RectTransform playButton;
    [Tooltip("A glow Image behind/around the Play button. Start disabled.")]
    public GameObject playButtonGlow;

    [Header("Finger Animation")]
    public float bobDistance = 15f;
    public float bobSpeed = 3f;
    public float fingerOffsetY = 80f;
    [Tooltip("How long the finger takes to travel to the play button.")]
    public float fingerMoveDuration = 0.8f;

    [Header("Glow Pulse")]
    public float pulseSpeed = 3f;

    private int currentLine;
    private int startRow;
    private int lineCount;
    private bool waitingForTap;
    private bool showingFinger;
    private bool fingerMoving;
    private Vector2 fingerBasePos;

    private void Start()
    {
        GameState gs = GameState.Instance;

        if (gs == null || gs.tutorialStep != 1)
        {
            if (overlayRoot != null) overlayRoot.SetActive(false);
            enabled = false;
            return;
        }

        // Make sure blood sugar is high for the tutorial
        if (gs.blood < 180) gs.blood = 200;

        // Get Part 1 line range
        if (gs.tutorialPartStarts == null || gs.tutorialPartStarts.Length < 1 || gs.script == null)
        {
            Debug.LogWarning("TutorialPart1: CSV data not found.");
            if (overlayRoot != null) overlayRoot.SetActive(false);
            enabled = false;
            return;
        }

        startRow = gs.tutorialPartStarts[0];
        if (gs.tutorialPartStarts.Length > 1)
            lineCount = gs.tutorialPartStarts[1] - startRow;
        else
            lineCount = gs.script.GetLength(0) - startRow;

        overlayRoot.SetActive(true);
        dialoguePanel.SetActive(true);
        fingerPointer.SetActive(false);
        if (bloodSugarGlow != null) bloodSugarGlow.SetActive(false);
        if (playButtonGlow != null) playButtonGlow.SetActive(false);

        currentLine = 0;
        waitingForTap = false;
        showingFinger = false;
        fingerMoving = false;

        ShowLine(0);
        waitingForTap = true;
    }

    private void Update()
    {
        // Finger bob (only after it has arrived)
        if (fingerPointer.activeSelf && !fingerMoving)
        {
            RectTransform rt = fingerPointer.GetComponent<RectTransform>();
            float offset = Mathf.Sin(Time.time * bobSpeed) * bobDistance;
            rt.anchoredPosition = fingerBasePos + new Vector2(0, offset);
        }

        // Blood sugar glow alpha pulse (75–175 out of 255)
        PulseGlow(bloodSugarGlow);

        // Play button glow alpha pulse (same range)
        PulseGlow(playButtonGlow);

        // Tap to advance dialogue
        if (waitingForTap && Input.GetMouseButtonDown(0))
        {
            waitingForTap = false;
            AdvanceDialogue();
        }
    }

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

    private void AdvanceDialogue()
    {
        currentLine++;

        if (currentLine == 1)
        {
            // After first line: play LowBloodSugar anim, show glow, then show line 2
            StartCoroutine(AnimThenNextLine());
        }
        else if (currentLine < lineCount)
        {
            ShowLine(currentLine);
            waitingForTap = true;
        }
        else
        {
            // All lines done — hide dialogue, show finger + play button glow
            dialoguePanel.SetActive(false);
            if (playButtonGlow != null) playButtonGlow.SetActive(true);
            ShowFinger();
        }
    }

    private IEnumerator AnimThenNextLine()
    {
        // Hide dialogue briefly while animation plays
        dialoguePanel.SetActive(false);

        // Trigger the pet animation
        if (petController != null)
            petController.LowBloodSugar();

        yield return new WaitForSeconds(1.5f);

        // Show blood sugar glow
        if (bloodSugarGlow != null)
            bloodSugarGlow.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        // Show next dialogue line
        dialoguePanel.SetActive(true);
        ShowLine(currentLine);
        waitingForTap = true;
    }

    private void ShowLine(int index)
    {
        GameState gs = GameState.Instance;
        int row = startRow + index;
        if (row >= gs.script.GetLength(0)) return;

        string line = gs.script[row, gs.language];

        if (!string.IsNullOrEmpty(gs.petName))
        {
            line = line.Replace("Fido", gs.petName);
            line = line.Replace("PET NAME", gs.petName);
            line = line.Replace("PET NAMES", gs.petName + "'s");
        }

        dialogueText.text = line;
    }

    private void ShowFinger()
    {
        showingFinger = true;
        fingerMoving = true;
        fingerPointer.SetActive(true);

        RectTransform fingerRT = fingerPointer.GetComponent<RectTransform>();
        RectTransform canvasRT = overlayRoot.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // Start from wherever the finger is placed in the editor
        Vector2 startPos = fingerRT.anchoredPosition;

        // Convert play button position to local canvas position
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, playButton.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, null, out Vector2 targetPos);
        targetPos += new Vector2(0, fingerOffsetY);

        StartCoroutine(MoveFingerArc(fingerRT, startPos, targetPos));

        // Listen for the Play button press
        Button btn = playButton.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnPlayPressed);
    }

    private IEnumerator MoveFingerArc(RectTransform fingerRT, Vector2 from, Vector2 to)
    {
        // Control point: offset to the right and above the midpoint for a natural sweep
        Vector2 mid = (from + to) * 0.5f;
        Vector2 perpendicular = new Vector2(-(to.y - from.y), to.x - from.x).normalized;
        float arcHeight = Vector2.Distance(from, to) * 0.3f;
        Vector2 control = mid + perpendicular * arcHeight;

        float elapsed = 0f;

        while (elapsed < fingerMoveDuration)
        {
            elapsed += Time.deltaTime;
            // Ease-out curve — starts fast, decelerates into position
            float t = 1f - Mathf.Pow(1f - (elapsed / fingerMoveDuration), 2.5f);

            // Quadratic bezier: (1-t)²·from + 2(1-t)t·control + t²·to
            float oneMinusT = 1f - t;
            Vector2 pos = oneMinusT * oneMinusT * from
                        + 2f * oneMinusT * t * control
                        + t * t * to;

            fingerRT.anchoredPosition = pos;
            yield return null;
        }

        fingerRT.anchoredPosition = to;
        fingerBasePos = to;
        fingerMoving = false;
    }

    private void OnPlayPressed()
    {
        Button btn = playButton.GetComponent<Button>();
        if (btn != null)
            btn.onClick.RemoveListener(OnPlayPressed);

        GameState gs = GameState.Instance;
        gs.tutorialStep = 2;
        SaveSystem.SavePet();

        fingerPointer.SetActive(false);
        if (playButtonGlow != null) playButtonGlow.SetActive(false);
        overlayRoot.SetActive(false);
        enabled = false;
    }
}