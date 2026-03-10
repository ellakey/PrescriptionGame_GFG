using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thin static wrapper that delegates to GameState.
/// Keeps all existing PatientInfo.xxx references compiling without changes.
/// </summary>
public static class PatientInfo
{
    public static int medId
    {
        get => GameState.Instance.medId;
        set => GameState.Instance.medId = value;
    }
    public static string medAmount
    {
        get => GameState.Instance.medAmount;
        set => GameState.Instance.medAmount = value;
    }
    public static int[] matcherIds
    {
        get => GameState.Instance.matcherIds;
        set => GameState.Instance.matcherIds = value;
    }
    public static List<int> medications
    {
        get => GameState.Instance.medications;
        set => GameState.Instance.medications = value;
    }
    public static string petName
    {
        get => GameState.Instance.petName;
        set => GameState.Instance.petName = value;
    }
    public static string dosage
    {
        get => GameState.Instance.dosage;
        set => GameState.Instance.dosage = value;
    }
    public static string time
    {
        get => GameState.Instance.time;
        set => GameState.Instance.time = value;
    }

    // These were never set by anything in the codebase, but keep them for SetInfo.cs
    public static string time1;
    public static string time2;

    public static void setId(int id) => GameState.Instance.SetMedId(id);
    public static void setMedAmount(string amount) => GameState.Instance.medAmount = amount;
    public static void setMatcherIds(int[] ids) => GameState.Instance.matcherIds = ids;
    public static int randomMed() => GameState.Instance.RandomMed();
    public static void setTime(string t) => GameState.Instance.time = t;
    public static void addMedication(int id) => GameState.Instance.AddMedication(id);
}