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
        results = new int[holder.getSize()];
        for (int i = 0; i < results.Length; i++)
        {
            Debug.Log("outside");
            results[i] = DragBehavior.itemCounts[i] / 9;
            Inventory.items[i] += results[i];
            if (results[i] > 0)
            {
                Debug.Log("inside");
                maxPan++;
                GameObject current = Instantiate(holder.getBerry(i), new Vector2(spawnX + (count * spacing), spawnY), Quaternion.identity, parent);
                current.GetComponent<Berry>().enabled = false;
                current.GetComponent<TouchOver>().enabled = false;
                current.GetComponent<CircleCollider2D>().enabled = false;
                current.GetComponent<GravityChecker>().enabled = false;
                current.GetComponent<RectTransform>().localScale = new Vector2(scaleX, scaleY);
                current.GetComponent<RectTransform>().localPosition = new Vector2(spawnX + (count * spacing), spawnY);
                current = Instantiate(textPrefab, new Vector2(spawnX + (count * spacing), spawnY), Quaternion.identity, parent);
                current.GetComponent<RectTransform>().localPosition = new Vector2(spawnX + (count * spacing) + 175, spawnY - 100);
                current.GetComponent<TextMeshProUGUI>().text += results[i];
                count++;
            }
        }
        scoretext.text = GameGrid.score.ToString("D6");
        Progression.progressionCounter++;

        SaveSystem.SavePet();
    }

    public void right()
    {
        if(pan < maxPan - 1)
        {
            posX = posX - spacing;
            pan++;
        }
    }

    public void left()
    {
        if (pan > 0)
        {
            posX = posX + spacing;
            pan--;
        }
    }

    private void Update()
    {
        parent.localPosition = new Vector2(currentPosX, parent.localPosition.y);
        if(currentPosX > posX)
        {
            currentPosX -= scrollSpeed;
        }
        else if(currentPosX < posX)
        {
            currentPosX += scrollSpeed;
        }

        if ((currentPosX - posX < 10 && currentPosX - posX >= 0) || (currentPosX - posX > -10 && currentPosX - posX <= 0))
        {
            currentPosX = posX;
        }
    }

}
