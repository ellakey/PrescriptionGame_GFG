using UnityEngine;
using TMPro;

/// <summary>
/// Displays the pet's name on a TextMeshProUGUI element.
/// Replaces both ChangeName.cs (set once) and BottleName.cs (continuous update).
/// Toggle 'continuous' for elements that need to update live (e.g. the bottle label
/// while the player is typing a new name).
/// </summary>
public class PetNameDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;
    public bool continuous = false;

    void Start()
    {
        text.text = PatientInfo.PetName;
    }

    void Update()
    {
        if (continuous)
        {
            text.text = PatientInfo.PetName;
        }
    }
}