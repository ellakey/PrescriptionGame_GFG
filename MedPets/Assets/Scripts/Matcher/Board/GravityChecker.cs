using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChecker : MonoBehaviour
{
    [SerializeField] Berry berry;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D col;
    [SerializeField] BerryMover mover;

    private void Start()
    {
        mover = GameObject.FindWithTag("Manager").GetComponent<BerryMover>();
    }
    private void FixedUpdate()
    {
        if (GetComponent<RectTransform>().localPosition.y <= mover.ConvertY(berry.Y)){
            GetComponent<RectTransform>().localPosition = new Vector2(GetComponent<RectTransform>().localPosition.x, mover.ConvertY(berry.Y));
        }
        else
        {
            //rb.isKinematic=false;
            //rb.gravityScale = 0.4f;
            //col.enabled = false;
            GetComponent<RectTransform>().localPosition = GetComponent<RectTransform>().localPosition - (new Vector3(0, 9.8f));
        }
    }
}
