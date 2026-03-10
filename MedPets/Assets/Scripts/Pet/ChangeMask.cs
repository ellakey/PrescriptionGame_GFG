using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMask : MonoBehaviour
{
    public Animator anim;
    public bool isOut;

    private void Start()
    {
        anim.SetBool("Pet", isOut);
    }
}
