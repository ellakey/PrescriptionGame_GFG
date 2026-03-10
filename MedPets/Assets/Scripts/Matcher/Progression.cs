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

    public int[] GetProgression()
    {
        if (progressionCounter <= 0)
        {
            progressionPanel1.SetActive(true);
            return new int[] { PatientInfo.RandomMed(), GetIncrease() };
        }
        else if (progressionCounter <= 1)
        {
            progressionPanel2.SetActive(true);
            return new int[] { PatientInfo.RandomMed(), GetIncrease(), GetDecrease() };
        }
        else if (progressionCounter <= 2)
        {
            progressionPanel3.SetActive(true);
            return new int[] { PatientInfo.RandomMed(), GetIncrease(), GetDecrease(), GetNeutral() };
        }
        else
        {
            return new int[] { PatientInfo.RandomMed(), GetIncrease(), GetDecrease(), GetNeutral() };
        }
    }

    private int GetIncrease()
    {
        List<int> increases = new List<int>();
        for (int i = 0; i < holder.Size; i++)
        {
            Berry berry = holder.GetBerry(i).GetComponent<Berry>();
            if (berry.bloodAmount > 0 && berry.Tags[0].Equals("Food"))
            {
                increases.Add(berry.Id);
            }
        }
        return increases[Random.Range(0, increases.Count)];
    }

    private int GetDecrease()
    {
        List<int> decreases = new List<int>();
        for (int i = 0; i < holder.Size; i++)
        {
            Berry berry = holder.GetBerry(i).GetComponent<Berry>();
            if (berry.bloodAmount < 0 && berry.Tags[0].Equals("Food"))
            {
                decreases.Add(berry.Id);
            }
        }
        return decreases[Random.Range(0, decreases.Count)];
    }

    private int GetNeutral()
    {
        List<int> neutrals = new List<int>();
        for (int i = 0; i < holder.Size; i++)
        {
            Berry berry = holder.GetBerry(i).GetComponent<Berry>();
            if (berry.bloodAmount == 0 && berry.Tags[0].Equals("Food"))
            {
                neutrals.Add(berry.Id);
            }
        }
        return neutrals[Random.Range(0, neutrals.Count)];
    }
}