using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Result : MonoBehaviour
{
    public TextMeshProUGUI scoretext;
    public GameObject textPrefab;
    public BerryHolder holder;
    public Transform parent;
    private int count;

    public float spawnX;
    public float spawnY;
    public float scaleX;
    public float scaleY;
    public float spacing;
    public float scrollSpeed;

    public int[] results;

    private int maxPan;
    private int pan;
    private float posX;
    private float currentPosX;

    void Start()
    {
        posX = parent.localPosition.x;
        int[] items = GameState.Instance.items;
        results = new int[holder.Size];

        for (int i = 0; i < results.Length; i++)
        {
            results[i] = DragBehavior.itemCounts[i] / 9;
            items[i] += results[i];
            if (results[i] > 0)
            {
                maxPan++;
                float xPos = spawnX + (count * spacing);

                // Berry display item — cache components
                GameObject berryObj = Instantiate(holder.GetBerry(i), new Vector2(xPos, spawnY), Quaternion.identity, parent);
                berryObj.GetComponent<Berry>().enabled = false;
                berryObj.GetComponent<TouchOver>().enabled = false;
                berryObj.GetComponent<CircleCollider2D>().enabled = false;
                berryObj.GetComponent<GravityChecker>().enabled = false;
                RectTransform berryRect = berryObj.GetComponent<RectTransform>();
                berryRect.localScale = new Vector2(scaleX, scaleY);
                berryRect.localPosition = new Vector2(xPos, spawnY);

                // Count label
                GameObject textObj = Instantiate(textPrefab, new Vector2(xPos, spawnY), Quaternion.identity, parent);
                textObj.GetComponent<RectTransform>().localPosition = new Vector2(xPos + 175, spawnY - 100);
                textObj.GetComponent<TextMeshProUGUI>().text += results[i];
                count++;
            }
        }

        scoretext.text = GameGrid.score.ToString("D6");
        Progression.progressionCounter++;
        SaveSystem.SavePet();
    }

    public void Right()
    {
        if (pan < maxPan - 1)
        {
            posX -= spacing;
            pan++;
        }
    }

    public void Left()
    {
        if (pan > 0)
        {
            posX += spacing;
            pan--;
        }
    }

    private void Update()
    {
        parent.localPosition = new Vector2(currentPosX, parent.localPosition.y);
        float diff = currentPosX - posX;

        if (currentPosX > posX)
            currentPosX -= scrollSpeed;
        else if (currentPosX < posX)
            currentPosX += scrollSpeed;

        if (diff < 10 && diff > -10)
            currentPosX = posX;
    }
}