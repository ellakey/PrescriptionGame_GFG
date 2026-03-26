using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialPart2 : MonoBehaviour
{
    [Header("UI")]
    public GameObject overlayRoot;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public GameObject fingerPointer;
    public GameObject countdownAnimation;
    public GameObject pauseButton;

    [Header("Matcher References")]
    public GameGrid grid;
    public DragBehavior drag;
    public Timer timer;

    [Header("Results Screen")]
    public GameObject resultsPanel;
    public RectTransform petButton;

    [Header("Finger Animation")]
    public float fingerMoveSpeed = 0.15f;
    public float fingerPauseBetween = 0.1f;
    public float bobDistance = 15f;
    public float bobSpeed = 3f;
    public float fingerOffsetY = 80f;
    public float fingerMoveDuration = 0.8f;

    // Internal state
    private int partStartRow;
    private int partLineCount;
    private bool playerMatched;
    private int requiredMatchSize;
    private bool tutorialActive;
    private bool showingResultsFinger;
    private bool fingerBobbing;
    private bool fingerIsMatching;
    private Vector2 fingerBasePos;

    private int typeA;
    private int typeB;

    private void Start()
    {
        countdownAnimation.SetActive(false);
        
        GameState gs = GameState.Instance;

        if (gs == null || gs.tutorialStep != 2)
        {
            if (overlayRoot != null) overlayRoot.SetActive(false);
            enabled = false;
            return;
        }

        // Get Part 2 CSV lines
        if (gs.tutorialPartStarts == null || gs.tutorialPartStarts.Length < 2 || gs.script == null)
        {
            Debug.LogWarning("TutorialPart2: CSV part data not found.");
            if (overlayRoot != null) overlayRoot.SetActive(false);
            enabled = false;
            return;
        }

        partStartRow = gs.tutorialPartStarts[1]; // PART 2 is index 1
        if (gs.tutorialPartStarts.Length > 2)
            partLineCount = gs.tutorialPartStarts[2] - partStartRow;
        else
            partLineCount = gs.script.GetLength(0) - partStartRow;

        tutorialActive = true;
        overlayRoot.SetActive(true);
        fingerPointer.SetActive(false);

        // Show first dialogue immediately
        dialoguePanel.SetActive(true);
        ShowDialogue(0);

        // Pause timer immediately to prevent countdown
        timer.isPaused = true;

        if (pauseButton != null) pauseButton.SetActive(false);

        drag.OnMatchCompleted += OnPlayerMatch;

        StartCoroutine(TutorialSequence());
    }

    private void OnDestroy()
    {
        if (drag != null)
            drag.OnMatchCompleted -= OnPlayerMatch;
    }

    private void Update()
    {
        // Finger bob
        if (fingerBobbing && fingerPointer.activeSelf)
        {
            RectTransform rt = fingerPointer.GetComponent<RectTransform>();
            float offset = Mathf.Sin(Time.time * bobSpeed) * bobDistance;
            rt.anchoredPosition = fingerBasePos + new Vector2(0, offset);
        }

        // Show finger on results screen when it appears
        if (tutorialActive && !showingResultsFinger &&
            resultsPanel != null && resultsPanel.activeInHierarchy)
        {
            showingResultsFinger = true;
            StartCoroutine(ShowResultsFinger());
        }
    }

    // ===== MAIN SEQUENCE =====

    private IEnumerator TutorialSequence()
    {
        // Wait one frame so Timer.Start() and GameGrid.Start() finish
        yield return null;

        // Bypass countdown — pause immediately
        timer.isPaused = true;
        timer.readytime = 0;
        if (timer.readyText != null) timer.readyText.enabled = false;
        if (timer.goText != null) timer.goText.SetActive(false);

        // Wait for board to settle
        grid.enabled = true;
        yield return new WaitForSeconds(1f);

        // Get berry types
        int[] berries = grid.Berries;
        if (berries == null || berries.Length == 0)
        {
            Debug.LogWarning("TutorialPart2: No berry types found.");
            yield break;
        }
        typeA = berries[0];
        typeB = (berries.Length > 1) ? berries[1] : berries[0];

        // ===== STAGE 1: Match of 3 =====
        Vector2Int[] fingerPath3 = RigStage3();
        ShowDialogue(0);

        yield return new WaitForSeconds(0.5f);
        yield return FingerDoMatch(fingerPath3);

        requiredMatchSize = 3;
        playerMatched = false;
        yield return new WaitUntil(() => playerMatched);
        yield return new WaitForSeconds(1.5f);

        // ===== STAGE 2: Chain of 6 =====
        Vector2Int[] fingerPath6 = RigStage6();
        ShowDialogue(1);

        yield return new WaitForSeconds(0.5f);
        yield return FingerDoMatch(fingerPath6);

        requiredMatchSize = 6;
        playerMatched = false;
        yield return new WaitUntil(() => playerMatched);
        yield return new WaitForSeconds(1.5f);

        // ===== STAGE 3: Chain of 9 =====
        Vector2Int[] fingerPath9 = RigStage9();
        ShowDialogue(2);

        yield return new WaitForSeconds(0.5f);
        yield return FingerDoMatch(fingerPath9);

        requiredMatchSize = 9;
        playerMatched = false;
        yield return new WaitUntil(() => playerMatched);

        // ===== FREE PLAY =====
        dialoguePanel.SetActive(false);
        fingerPointer.SetActive(false);
        overlayRoot.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(true);
        timer.readytime = 0;
        timer.isPaused = false;

        // Tutorial continues — waiting for results screen in Update
    }

    // ===== PLAYER MATCH CALLBACK =====

    private void OnPlayerMatch(int berryCount)
    {
        // Ignore matches made by the finger
        if (fingerIsMatching) return;

        if (berryCount >= requiredMatchSize)
        {
            playerMatched = true;
        }
    }

    // ===== BOARD RIGGING =====

    private Vector2Int[] RigStage3()
    {
        // Finger: vertical, col 1, rows 0–2
        grid.ReplaceTile(1, 0, typeA);
        grid.ReplaceTile(1, 1, typeA);
        grid.ReplaceTile(1, 2, typeA);

        // Player: horizontal, row 4, cols 3–5
        int py = Mathf.Min(4, grid.Height - 1);
        for (int x = 3; x < Mathf.Min(6, grid.Width); x++)
            grid.ReplaceTile(x, py, typeA);

        return new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, 2)
        };
    }

    private Vector2Int[] RigStage6()
    {
        // Finger: diagonal AAA at (0,0)(1,1)(2,2), then horizontal BBB at (3,2)(4,2)(5,2)
        grid.ReplaceTile(0, 0, typeA);
        grid.ReplaceTile(1, 1, typeA);
        grid.ReplaceTile(2, 2, typeA);
        grid.ReplaceTile(3, 2, typeB);
        grid.ReplaceTile(4, 2, typeB);
        int lastX = Mathf.Min(5, grid.Width - 1);
        grid.ReplaceTile(lastX, 2, typeB);

        // Player: horizontal 6, row 5
        int py = Mathf.Min(5, grid.Height - 1);
        for (int x = 0; x < Mathf.Min(6, grid.Width); x++)
            grid.ReplaceTile(x, py, x < 3 ? typeA : typeB);

        return new Vector2Int[]
        {
            new Vector2Int(0, 0),
            new Vector2Int(1, 1),
            new Vector2Int(2, 2),
            new Vector2Int(3, 2),
            new Vector2Int(4, 2),
            new Vector2Int(lastX, 2)
        };
    }

    private Vector2Int[] RigStage9()
    {
        int maxX = grid.Width - 1;
        int maxY = grid.Height - 1;

        // Finger: horizontal AAA at (0,0)(1,0)(2,0)
        grid.ReplaceTile(0, 0, typeA);
        grid.ReplaceTile(1, 0, typeA);
        grid.ReplaceTile(2, 0, typeA);

        // Then vertical BBB at (2,1)(2,2)(2,3) — connects from (2,0)
        grid.ReplaceTile(2, 1, typeB);
        grid.ReplaceTile(2, 2, typeB);
        grid.ReplaceTile(2, Mathf.Min(3, maxY), typeB);

        // Then zigzag AAA: down-right, up-right, down-right
        // (3,4) → (4,3) → (5,4) — shows you can change direction freely
        int dx0 = Mathf.Min(3, maxX);
        int dy0 = Mathf.Min(4, maxY);
        int dx1 = Mathf.Min(4, maxX);
        int dy1 = Mathf.Min(3, maxY);
        int dx2 = Mathf.Min(5, maxX);
        int dy2 = Mathf.Min(4, maxY);
        grid.ReplaceTile(dx0, dy0, typeA);
        grid.ReplaceTile(dx1, dy1, typeA);
        grid.ReplaceTile(dx2, dy2, typeA);

        // Player: two-row chain near bottom
        int pr0 = Mathf.Min(grid.Height - 2, maxY);
        int pr1 = Mathf.Min(grid.Height - 1, maxY);
        for (int x = 0; x < Mathf.Min(6, grid.Width); x++)
            grid.ReplaceTile(x, pr0, x < 3 ? typeA : typeB);
        for (int i = 0; i < 3; i++)
        {
            int px = Mathf.Min(5 - i, maxX);
            grid.ReplaceTile(px, pr1, typeA);
        }

        return new Vector2Int[]
        {
            // Horizontal right
            new Vector2Int(0, 0),
            new Vector2Int(1, 0),
            new Vector2Int(2, 0),
            // Vertical down
            new Vector2Int(2, 1),
            new Vector2Int(2, 2),
            new Vector2Int(2, Mathf.Min(3, maxY)),
            // Zigzag diagonal
            new Vector2Int(dx0, dy0),
            new Vector2Int(dx1, dy1),
            new Vector2Int(dx2, dy2)
        };
    }

    // ===== FINGER MATCH ANIMATION =====

    private IEnumerator FingerDoMatch(Vector2Int[] path)
    {
        fingerIsMatching = true;
        fingerBobbing = false;
        fingerPointer.SetActive(true);
        drag.TutorialFingerActive = true;

        // Arc-move from current idle position to the first tile
        RectTransform fingerRT = fingerPointer.GetComponent<RectTransform>();
        RectTransform canvasRT = overlayRoot.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Vector2 startPos = fingerRT.anchoredPosition;

        GameObject firstTile = grid.Get(path[0].x, path[0].y);
        Vector3 firstTileWorld = firstTile != null ? firstTile.transform.position : GetTileWorldPos(path[0].x, path[0].y);

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, firstTileWorld);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, null, out Vector2 targetCanvasPos);

        yield return MoveFingerArc(fingerRT, startPos, targetCanvasPos);

        // Snap to world position for the drag phase
        fingerPointer.transform.position = firstTileWorld;

        yield return new WaitForSeconds(0.2f);

        // Start the drag
        drag.OnDown();

        // Add first tile
        if (firstTile != null)
        {
            drag.AddDragged(firstTile);
            firstTile.GetComponent<Berry>().Added = true;
            drag.DrawLines();
        }

        // Animate through remaining tiles
        for (int i = 1; i < path.Length; i++)
        {
            Vector3 from = fingerPointer.transform.position;
            GameObject tile = grid.Get(path[i].x, path[i].y);
            Vector3 to = tile != null ? tile.transform.position : GetTileWorldPos(path[i].x, path[i].y);

            float elapsed = 0f;
            while (elapsed < fingerMoveSpeed)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fingerMoveSpeed;
                fingerPointer.transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }
            fingerPointer.transform.position = to;

            // Add this tile to the drag chain
            if (tile != null)
            {
                drag.AddDragged(tile);
                tile.GetComponent<Berry>().Added = true;
                drag.DrawLines();
            }

            yield return new WaitForSeconds(fingerPauseBetween);
        }

        yield return new WaitForSeconds(0.2f);

        // Release — triggers match validation, scoring, removal, and refill
        drag.OnUp();
        drag.TutorialFingerActive = false;
        fingerIsMatching = false;

        fingerPointer.SetActive(false);

        // Wait for gravity refill
        yield return new WaitForSeconds(1f);
    }

    private Vector3 GetTileWorldPos(int x, int y)
    {
        GameObject tile = grid.Get(x, y);
        if (tile != null)
            return tile.transform.position;

        // Fallback: compute from mover coordinates
        BerryMover mover = grid.Mover;
        Vector2 local = new Vector2(mover.ConvertX(x), mover.ConvertY(y));
        return grid.Parent.TransformPoint(local);
    }

    // ===== DIALOGUE =====

    private void ShowDialogue(int lineIndex)
    {
        if (lineIndex >= partLineCount) return;

        GameState gs = GameState.Instance;
        int row = partStartRow + lineIndex;
        if (row >= gs.script.GetLength(0)) return;

        string line = gs.script[row, gs.language];

        if (!string.IsNullOrEmpty(gs.petName))
        {
            line = line.Replace("Fido", gs.petName);
            line = line.Replace("PET NAME", gs.petName);
            line = line.Replace("PET NAMES", gs.petName + "'s");
        }

        dialogueText.text = line;
        dialoguePanel.SetActive(true);
    }

    // ===== RESULTS SCREEN =====

    private IEnumerator ShowResultsFinger()
    {
        yield return new WaitForSeconds(1f);

        if (petButton == null) yield break;

        fingerPointer.SetActive(true);
        fingerBobbing = false;

        // Animate finger to Pet button
        RectTransform fingerRT = fingerPointer.GetComponent<RectTransform>();
        RectTransform canvasRT = overlayRoot.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 startPos = fingerRT.anchoredPosition;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, petButton.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT, screenPos, null, out Vector2 targetPos);
        targetPos += new Vector2(0, fingerOffsetY);

        // Arc move
        yield return MoveFingerArc(fingerRT, startPos, targetPos);

        // Bob in place
        fingerBasePos = targetPos;
        fingerBobbing = true;

        // Listen for Pet button
        Button btn = petButton.GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnPetButtonPressed);
    }

    private IEnumerator MoveFingerArc(RectTransform fingerRT, Vector2 from, Vector2 to)
    {
        Vector2 mid = (from + to) * 0.5f;
        Vector2 perp = new Vector2(-(to.y - from.y), to.x - from.x).normalized;
        float arcHeight = Vector2.Distance(from, to) * 0.3f;
        Vector2 control = mid + perp * arcHeight;

        float elapsed = 0f;
        while (elapsed < fingerMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Pow(1f - (elapsed / fingerMoveDuration), 2.5f);
            float omt = 1f - t;
            fingerRT.anchoredPosition = omt * omt * from + 2f * omt * t * control + t * t * to;
            yield return null;
        }

        fingerRT.anchoredPosition = to;
    }

    private void OnPetButtonPressed()
    {
        Button btn = petButton.GetComponent<Button>();
        if (btn != null)
            btn.onClick.RemoveListener(OnPetButtonPressed);

        GameState gs = GameState.Instance;
        gs.tutorialStep = 3;
        SaveSystem.SavePet();

        fingerPointer.SetActive(false);
        overlayRoot.SetActive(false);
        tutorialActive = false;
        enabled = false;
    }
}