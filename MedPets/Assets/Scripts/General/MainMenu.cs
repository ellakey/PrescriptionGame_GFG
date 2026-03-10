using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickaudio;
    private AudioSource audioSource;
    public GameObject mask;
    public GameObject maskPrefab;
    public RectTransform parent;

    void Start()
    {
        if (audioSource != null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = clickaudio;
        }
    }

    public void ToTitle()
    {
        if (audioSource != null) audioSource.Play();
        SceneManager.LoadScene("Title");
    }

    public void ToPet()
    {
        if (audioSource != null) audioSource.Play();
        SceneManager.LoadScene("Pet");
    }

    public void ToLanguage()
    {
        SaveSystem.LoadPet();
        GameState gs = GameState.Instance;
        if (!gs.playedOnce)
        {
            if (audioSource != null) audioSource.Play();
            SceneManager.LoadScene("Language");
        }
        else
        {
            if (audioSource != null) audioSource.Play();
            SceneManager.LoadScene("Pet");
        }
    }

    public void ToTutorial()
    {
        if (audioSource != null) audioSource.Play();
        SceneManager.LoadScene("Tutorial");
    }

    public void ToPuzzle()
    {
        if (audioSource != null) audioSource.Play();
        if (mask != null)
        {
            StartCoroutine(Fade());
        }
        else
        {
            mask = Instantiate(maskPrefab, parent);
            StartCoroutine(Fade());
        }
        GameState gs = GameState.Instance;
        gs.food -= Random.Range(10, 20);
        gs.energy += Random.Range(20, 40);
        gs.blood -= Random.Range(20, 40);
    }

    public void ToSettings()
    {
        if (audioSource != null) audioSource.Play();
        SceneManager.LoadScene("Settings");
    }

    public void ToInventory()
    {
        if (audioSource != null) audioSource.Play();
        SceneManager.LoadScene("Inventory");
    }

    public void ToGuidebook()
    {
        if (audioSource != null) audioSource.Play();
        SceneManager.LoadScene("Guidebook");
    }

    public void ToInfo()
    {
        if (audioSource != null) audioSource.Play();
    }

    IEnumerator Fade()
    {
        mask.SetActive(true);
        yield return new WaitForSeconds(2.1f);
        SceneManager.LoadScene("Matcher");
    }

    public void ResetData()
    {
        SaveSystem.Reset();
    }

    public void SetLanguage(int language)
    {
        PlayerPrefs.SetInt("Language", language);
        GameState.Instance.language = language;
    }
}