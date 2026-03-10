using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSprite : MonoBehaviour
{
    public SpriteRenderer dog;
    public Sprite hungrysprite;
    public Sprite happysprite;

    void Start()
    {
        dog.sprite = happysprite;
    }

    void Update()
    {
        if (GameState.Instance.food >= 50)
        {
            dog.sprite = happysprite;
        }
        else
        {
            dog.sprite = hungrysprite;
        }
    }
}