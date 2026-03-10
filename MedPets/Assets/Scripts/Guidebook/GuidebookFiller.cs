using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidebookFiller : MonoBehaviour
{
    public BerryHolder holder;
    public GameObject prefab;
    public List<GameObject> prefabHolder;
    public bool changing;
    public Image plusButtonImage;
    public Sprite plusSprite;
    public Sprite checkSprite;

    // Start is called before the first frame update
    void Start()
    {
        prefabHolder = new List<GameObject>();
        for (int i = 0; i < holder.getSize(); i++)
        {
            if (holder.getBerry(i).GetComponent<Berry>().getTags()[0].Equals("Medication"))
            {
                GameObject current = Instantiate(prefab, transform);
                current.transform.GetChild(0).GetComponentInChildren<Image>().sprite = holder.getBerry(i).GetComponent<Image>().sprite;
                current.GetComponentInChildren<GuidebookItem>().guidebookID = i;
                prefabHolder.Add(current);
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < prefabHolder.Count; i++)
        {
            bool colored = false;
            Image image = prefabHolder[i].transform.GetChild(0).GetComponent<Image>();
            for (int j = 0; j < PatientInfo.medications.Count; j++)
            {
                if (!colored && prefabHolder[i].GetComponent<GuidebookItem>().guidebookID == PatientInfo.medications[j])
                {
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
                    colored = true;
                }
            }
            if (!colored)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.3f);
            }
        }
        
    }

    public void addMedsButton()
    {
        for(int i = 0; i < prefabHolder.Count; i++)
        {
            if (changing)
            {
                prefabHolder[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                prefabHolder[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        if (changing)
        {
            changing = false;
            plusButtonImage.sprite = plusSprite;
        }
        else
        {
            changing = true;
            plusButtonImage.sprite = checkSprite;
        }
    }
}
