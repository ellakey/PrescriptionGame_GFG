using UnityEngine;

public class TimingManager : MonoBehaviour
{
    private float gameHourTimer;

    public float hourLength;

    public void Update()
    {
        if (gameHourTimer <= 0)
        {
            gameHourTimer = hourLength;
        }
        else
        {
            gameHourTimer -= Time.deltaTime;
        }
    }
}
