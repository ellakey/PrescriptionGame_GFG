using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragBehavior : MonoBehaviour
{
    public AudioSource rightaudio;
    public AudioSource wrongaudio;

    public Color correct;
    public Color wrong;

    [Header("Scoring")]
    [SerializeField] int basePointsPerBerry = 100;
    [SerializeField] float chainMultiplier = 1.5f;

    [SerializeField] List<Berry> dragged = new List<Berry>();
    bool dragging = false;
    [SerializeField] GameGrid grid;
    [SerializeField] BerryMover mover;
    [SerializeField] BerryHolder holder;
    [SerializeField] CircleCollider2D col;
    [SerializeField] LineRenderer line;
    [SerializeField] GameObject collectedPrefab;
    [SerializeField] RectTransform parent;

    private Camera mainCam;

    public bool IsDragging => dragging;

    public bool TutorialFingerActive { get; set; }

    public Action<int> OnMatchCompleted;

    void Awake()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        // Skip mouse tracking while tutorial finger is in control
        if (TutorialFingerActive) return;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;

        if (Input.GetMouseButtonDown(0))
        {
            OnDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnUp();
        }
    }

    private int CheckList()
    {
        if (dragged.Count <= 0) return -1;

        Berry first = dragged[0];
        int prevId = first.Id;
        int count = 0;
        int toMatch = first.NumToMatch;
        int[] ids = new int[holder.Size];
        int chains = 1;
        List<string> tagsFound = new List<string>();

        foreach (string tag in first.Tags)
        {
            tagsFound.Add(tag);
        }

        for (int i = 1; i < dragged.Count; i++)
        {
            Berry berry = dragged[i];
            count++;
            if (prevId != berry.Id)
            {
                if (count != toMatch)
                {
                    return -1;
                }
                else
                {
                    ids[prevId] = 1;
                    int[] incomp = berry.Incompatibilities;
                    for (int j = 0; j < incomp.Length; j++)
                    {
                        if (ids[incomp[j]] == 1)
                        {
                            return -1;
                        }
                    }
                    prevId = berry.Id;
                    foreach (string tag in berry.Tags)
                    {
                        tagsFound.Add(tag);
                    }
                    count = 0;
                    chains++;
                    toMatch = berry.NumToMatch;
                }
            }
        }

        count++;
        if (count != toMatch)
        {
            return -1;
        }

        // Tag Handling
        for (int i = 0; i < dragged.Count; i++)
        {
            string[] required = dragged[i].RequiredTags;
            if (required.Length > 0)
            {
                for (int j = 0; j < required.Length; j++)
                {
                    bool found = false;
                    for (int k = 0; k < tagsFound.Count; k++)
                    {
                        if (required[j].Equals(tagsFound[k]))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        return -1;
                    }
                }
            }
        }

        if (chains < 1)
        {
            return -1;
        }
        return chains;
    }

    private int CalculateScore(int berryCount, int chains)
    {
        float multiplier = Mathf.Pow(chainMultiplier, chains - 1);
        return Mathf.RoundToInt(berryCount * basePointsPerBerry * multiplier);
    }

    public void OnUp()
    {
        dragging = false;
        if (col != null) col.enabled = false;
        int chains = CheckList();
        if (chains != -1)
        {
            int berryCount = dragged.Count;
            int[,] loc = new int[dragged.Count, 2];
            for (int i = 0; i < dragged.Count; i++)
            {
                loc[i, 0] = dragged[i].X;
                loc[i, 1] = dragged[i].Y;
            }
            ClearAdded();
            rightaudio.Play();
            grid.AddScore(CalculateScore(berryCount, chains));
            int[] removed = grid.RemoveGroup(loc);
            int[] counts = grid.Session.ItemCounts;
            int count = 0;
            for (int i = 0; i < removed.Length; i++)
            {
                int currentCount = counts[i];
                counts[i] += removed[i];
                if (counts[i] % 9 == 0 && counts[i] != currentCount)
                {
                    StartCoroutine(Spawning(count, i));
                    count++;
                }
            }

            // Notify listeners (tutorial) about the successful match
            OnMatchCompleted?.Invoke(berryCount);

            dragged.Clear();
        }
        else
        {
            ClearAdded();
            wrongaudio.Play();
        }
        ClearAdded();
    }

    IEnumerator Spawning(int i, int id)
    {
        yield return new WaitForSeconds(1.5f * i);
        GameObject collected = Instantiate(collectedPrefab, parent.transform);
        collected.GetComponent<RectTransform>().position = gameObject.transform.position;
        collected.GetComponent<Image>().sprite = holder.GetBerry(id).GetComponent<Image>().sprite;
    }

    public void DrawLines()
    {
        if (CheckList() >= 0)
        {
            line.startColor = correct;
            line.endColor = correct;
        }
        else
        {
            line.startColor = wrong;
            line.endColor = wrong;
        }
        Vector3[] linePos = new Vector3[dragged.Count];
        line.positionCount = dragged.Count;
        for (int i = 0; i < dragged.Count; i++)
        {
            linePos[i] = dragged[i].transform.position;
        }
        line.SetPositions(linePos);
    }

    void ClearAdded()
    {
        for (int i = 0; i < dragged.Count; i++)
        {
            dragged[i].Added = false;
        }
        dragged.Clear();
        if (line != null) line.positionCount = 0;
    }

    public void OnDown()
    {
        dragging = true;
    }

    public void AddDragged(GameObject obj)
    {
        dragged.Add(obj.GetComponent<Berry>());
    }

    /// <summary>
    /// Returns true if the given berry is the second-to-last in the drag chain.
    /// Used by TouchOver to detect when the player drags back to undo.
    /// </summary>
    public bool IsSecondToLast(Berry berry)
    {
        return dragged.Count >= 2 && dragged[dragged.Count - 2] == berry;
    }

    /// <summary>
    /// Removes the last berry from the chain (undo).
    /// Called by TouchOver when the player drags back over the previous berry.
    /// </summary>
    public void UndoLast()
    {
        if (dragged.Count < 2) return;

        Berry last = dragged[dragged.Count - 1];
        last.Added = false;
        dragged.RemoveAt(dragged.Count - 1);
        DrawLines();
    }
}