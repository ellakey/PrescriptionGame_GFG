using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PatientInfo
{
    public static int medId = 0;
    public static string medAmount;
    public static int[] matcherIds;
    public static List<int> medications = new List<int>();
    public static string time1;
    public static string time2;
    public static string petName;
    public static string dosage;
    public static string time;


    public static void setId(int id)
    {
        medId = id;
        matcherIds = new int[] { id, 7, 10, 4, 5};
    }

    public static void setMedAmount(string amount)
    {
        medAmount = amount;
    }

    public static void setMatcherIds(int[] ids)
    {
        matcherIds = ids;
    }

    public static int randomMed()
    {
        if (medications.Count != 0) 
        {
            return medications[Random.Range(0, PatientInfo.medications.Count)];
        }
        else
        {
            return 0;
        }
        
    }

    public static void setTime(string t)
    {
        time = t;
    }

    public static void addMedication(int id)
    {
        bool changed = false;
        for(int i = 0; i < medications.Count; i++)
        {
            if (medications[i] == id)
            {
                medications.Remove(id);
                Debug.Log("Removed " + id);
                changed = true;
            }
        }
        if(!changed)
        {
            medications.Add(id);
            Debug.Log("Added " + id);
        }
        

        for (int i = 0; i < medications.Count; i++)
        {
            Debug.Log(medications[i] + " Spot " + i);
        }

        SaveSystem.SavePet();
    }
}
