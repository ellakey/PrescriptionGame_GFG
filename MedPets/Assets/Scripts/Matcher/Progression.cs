using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progression : MonoBehaviour
{
    public static int progressionCounter
    {
        get => GameState.Instance.progressionCounter;
        set => GameState.Instance.progressionCounter = value;
    }

    public BerryHolder holder;
    public SetLanguageText progressionText;
    public GameObject progressionPanel1;
    public GameObject progressionPanel2;
    public GameObject progressionPanel3;
    public GameObject progressionPanel4;

    public int[] getProgression()
    {
        if (progressionCounter <= 0)
        {
            progressionPanel1.SetActive(true);
            return new int[] { PatientInfo.randomMed(), getIncrease() };
        }
        else if (progressionCounter <= 1)
        {
            progressionPanel2.SetActive(true);
            return new int[] { PatientInfo.randomMed(), getIncrease(), getDecrease() };
        }
        else if (progressionCounter <= 2)
        {
            progressionPanel3.SetActive(true);
            return new int[] { PatientInfo.randomMed(), getIncrease(), getDecrease(), getNeutral() };
        }
        else
        {
            return new int[] { PatientInfo.randomMed(), getIncrease(), getDecrease(), getNeutral() };
        }
    }

    private int getIncrease()
    {
        List<int> increases = new List<int>();
        for (int i = 0; i < holder.getSize(); i++)
        {
            Berry berry = holder.getBerry(i).GetComponent<Berry>();
            if (berry.bloodAmount > 0 && berry.getTags()[0].Equals("Food"))
            {
                increases.Add(berry.getId());
            }
        }
        return increases[Random.Range(0, increases.Count)];
    }

    private int getDecrease()
    {
        List<int> decreases = new List<int>();
        for (int i = 0; i < holder.getSize(); i++)
        {
            Berry berry = holder.getBerry(i).GetComponent<Berry>();
            if (berry.bloodAmount < 0 && berry.getTags()[0].Equals("Food"))
            {
                decreases.Add(berry.getId());
            }
        }
        return decreases[Random.Range(0, decreases.Count)];
    }

    private int getNeutral()
    {
        List<int> neutrals = new List<int>();
        for (int i = 0; i < holder.getSize(); i++)
        {
            Berry berry = holder.getBerry(i).GetComponent<Berry>();
            if (berry.bloodAmount == 0 && berry.getTags()[0].Equals("Food"))
            {
                neutrals.Add(berry.getId());
            }
        }
        return neutrals[Random.Range(0, neutrals.Count)];
    }
}