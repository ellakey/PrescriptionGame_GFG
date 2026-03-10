using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySoon : MonoBehaviour
{
    public float timer;

    private void Start()
    {
        StartCoroutine(delete());
    }

    IEnumerator delete()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
