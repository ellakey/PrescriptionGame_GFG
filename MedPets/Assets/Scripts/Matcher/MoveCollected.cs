using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCollected : MonoBehaviour
{
    public float x;
    public float y;
    public float speed;
    private bool spawned;
    Vector2 direction;

    private void Move()
    {
        direction = (new Vector2(x, y) - gameObject.GetComponent<RectTransform>().anchoredPosition);
        gameObject.GetComponent<RectTransform>().anchoredPosition += direction.normalized * speed;
        if(direction.x < 10 && direction.y < 10 && direction.x > -10 && direction.y > -10)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
    }

    private void Update()
    {
        Move();
        StartCoroutine(spawn());
        if (spawned && direction.x < 2 && direction.y < 2 && direction.x > -2 && direction.y > -2)
        {
            StartCoroutine(despawn());
        }
    }

    IEnumerator spawn()
    {
        yield return new WaitForSeconds(0.3f);
        spawned = true;
    }

    IEnumerator despawn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
