using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragBehavior : MonoBehaviour
{
    public AudioSource rightaudio;
    public AudioSource wrongaudio;

    public static int[] itemCounts;

    public Color correct;
    public Color wrong;

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

    void Awake()
    {
        itemCounts = new int[BerryHolder.itemCount];
        mainCam = Camera.main;
    }

    private void Update()
    {
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;

        if (Input.GetMouseButtonDown(0))
        {
            onDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            onUp();
        }
    }

    private int checkList()
    {
        if (dragged.Count <= 0) return -1;

        Berry first = dragged[0];
        int prevId = first.getId();
        int count = 0;
        int toMatch = first.getNum2Match();
        int[] ids = new int[holder.getSize()];
        int chains = 1;
        List<string> tagsFound = new List<string>();

        foreach (string tag in first.getTags())
        {
            tagsFound.Add(tag);
        }

        for (int i = 1; i < dragged.Count; i++)
        {
            Berry berry = dragged[i];
            count++;
            if (prevId != berry.getId())
            {
                if (count != toMatch)
                {
                    return -1;
                }
                else
                {
                    ids[prevId] = 1;
                    int[] incomp = berry.getIncomp();
                    for (int j = 0; j < incomp.Length; j++)
                    {
                        if (ids[incomp[j]] == 1)
                        {
                            return -1;
                        }
                    }
                    prevId = berry.getId();
                    foreach (string tag in berry.getTags())
                    {
                        tagsFound.Add(tag);
                    }
                    count = 0;
                    chains++;
                    toMatch = berry.getNum2Match();
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
            string[] required = dragged[i].getRequiredTags();
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

    public void onUp()
    {
        dragging = false;
        if(col != null) col.enabled = false;
        int chains = checkList();
        if (chains != -1)
        {
            int[,] loc = new int[dragged.Count, 2];
            for (int i = 0; i < dragged.Count; i++)
            {
                loc[i, 0] = dragged[i].getX();
                loc[i, 1] = dragged[i].getY();
            }
            clearAdded();
            rightaudio.Play();
            grid.addScore(dragged.Count * 100);
            int[] removed = grid.removeGroup(loc);
            int count = 0;
            for (int i = 0; i < removed.Length; i++)
            {
                int currentCount = itemCounts[i];
                itemCounts[i] += removed[i];
                if (itemCounts[i] % 9 == 0 && itemCounts[i] != currentCount)
                {
                    StartCoroutine(spawning(count, i));
                    count++;
                }
            }
            dragged.Clear();
        }
        else
        {
            clearAdded();
            wrongaudio.Play();
        }
        clearAdded();
    }

    IEnumerator spawning(int i, int id)
    {
        yield return new WaitForSeconds(1.5f * i);
        GameObject collected = Instantiate(collectedPrefab, parent.transform);
        collected.GetComponent<RectTransform>().position = gameObject.transform.position;
        collected.GetComponent<Image>().sprite = holder.getBerry(id).GetComponent<Image>().sprite;
    }

    public void drawLines()
    {
        if (checkList() >= 0)
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

    void clearAdded()
    {
        for (int i = 0; i < dragged.Count; i++)
        {
            dragged[i].setAdded(false);
        }
        dragged.Clear();
        if(line != null) line.positionCount = 0;
    }

    public void onDown()
    {
        dragging = true;
    }

    public void addDragged(GameObject obj)
    {
        dragged.Add(obj.GetComponent<Berry>());
    }

    public bool isDragging()
    {
        return dragging;
    }
}