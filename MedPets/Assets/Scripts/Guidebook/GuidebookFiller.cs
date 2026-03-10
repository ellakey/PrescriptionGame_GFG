using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookFiller : MonoBehaviour
{
    public BerryHolder holder;
    public GameObject prefab;
    public bool changing;
    public Image plusButtonImage;
    public Sprite plusSprite;
    public Sprite checkSprite;

    // Cached references so Update doesn't call GetComponent every frame
    private List<Image> itemImages = new List<Image>();
    private List<int> itemIds = new List<int>();
    private List<GameObject> toggleChildren = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < holder.getSize(); i++)
        {
            GameObject berryObj = holder.getBerry(i);
            Berry berry = berryObj.GetComponent<Berry>();
            if (berry.getTags()[0].Equals("Medication"))
            {
                GameObject current = Instantiate(prefab, transform);
                Image icon = current.transform.GetChild(0).GetComponentInChildren<Image>();
                icon.sprite = berryObj.GetComponent<Image>().sprite;

                GuidebookItem item = current.GetComponentInChildren<GuidebookItem>();
                item.guidebookID = i;

                // Cache what we need for Update and addMedsButton
                itemImages.Add(icon);
                itemIds.Add(i);
                toggleChildren.Add(current.transform.GetChild(1).gameObject);
            }
        }
    }

    private void Update()
    {
        List<int> meds = PatientInfo.medications;
        for (int i = 0; i < itemImages.Count; i++)
        {
            bool colored = false;
            for (int j = 0; j < meds.Count; j++)
            {
                if (itemIds[i] == meds[j])
                {
                    Image img = itemImages[i];
                    img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
                    colored = true;
                    break;
                }
            }
            if (!colored)
            {
                Image img = itemImages[i];
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0.3f);
            }
        }
    }

    public void addMedsButton()
    {
        for (int i = 0; i < toggleChildren.Count; i++)
        {
            toggleChildren[i].SetActive(!changing);
        }

        changing = !changing;
        plusButtonImage.sprite = changing ? checkSprite : plusSprite;
    }
}