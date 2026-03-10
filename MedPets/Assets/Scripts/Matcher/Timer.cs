using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public float readytime = 3;
    public float countdowntime = 90;
    public float remainingtime;

    public TextMeshProUGUI readyText;
    public TextMeshProUGUI countdownText;
    
    public GameObject goText;
    public GameObject finishText;
    public GameObject pausePanel;
    public GameObject resultsPanel;
    public GameObject mouseDetector; 

    public bool isPaused = false;
    public GameGrid gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        remainingtime = countdowntime;
        readyText.text = readytime.ToString("F0");
        countdownText.text = countdowntime.ToString("F0");
        gameGrid.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPaused)
        {
            if(readytime > 0)
            {
                readytime -= Time.deltaTime;
                readyText.text = readytime.ToString("F0");
            }
            if(readytime <= 1)
            {
                readyText.enabled = false;
                goText.SetActive(true);
                readytime -= Time.deltaTime;
            }
        
            if(readytime <= 0)
            {
                gameGrid.enabled = true;
                countdowntime -= Time.deltaTime;
                countdownText.text = countdowntime.ToString("F0");
            }

            if(countdowntime <= 0)
            {
                finishText.SetActive(true);
                countdownText.text = "0";
                gameGrid.enabled = false;
            }

            if(countdowntime <= -3)
            {
                mouseDetector.SetActive(false);
                resultsPanel.SetActive(true);
            }

        }
        
    }

    public void EndTimer()
    {
        countdowntime = 0;
    }

    public void Pause()
    {
        if(isPaused)
        {
            pausePanel.SetActive(false);
            readytime = 3;
            readyText.enabled = true;
            isPaused = false;
        }
        else
        {
            pausePanel.SetActive(true);
            isPaused = true;
        }
    }
}
