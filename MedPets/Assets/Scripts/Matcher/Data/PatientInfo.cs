using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thin static wrapper that delegates to GameState.
/// Keeps all existing PatientInfo.xxx references compiling without changes.
/// </summary>
public static class PatientInfo
{
    public static int MedId
    {
        get => GameState.Instance.medId;
        set => GameState.Instance.medId = value;
    }
    public static string MedAmount
    {
        get => GameState.Instance.medAmount;
        set => GameState.Instance.medAmount = value;
    }
    public static int[] MatcherIds
    {
        get => GameState.Instance.matcherIds;
        set => GameState.Instance.matcherIds = value;
    }
    public static List<int> Medications
    {
        get => GameState.Instance.medications;
        set => GameState.Instance.medications = value;
    }
    public static string PetName
    {
        get => GameState.Instance.petName;
        set => GameState.Instance.petName = value;
    }
    public static string Dosage
    {
        get => GameState.Instance.dosage;
        set => GameState.Instance.dosage = value;
    }
    public static string Time
    {
        get => GameState.Instance.time;
        set => GameState.Instance.time = value;
    }

    public static string time1;
    public static string time2;

    public static void SetId(int id) => GameState.Instance.SetMedId(id);
    public static void SetMedAmount(string amount) => GameState.Instance.medAmount = amount;
    public static void SetMatcherIds(int[] ids) => GameState.Instance.matcherIds = ids;
    public static int RandomMed() => GameState.Instance.RandomMed();
    public static void SetTime(string t) => GameState.Instance.time = t;
    public static void AddMedication(int id) => GameState.Instance.AddMedication(id);
}