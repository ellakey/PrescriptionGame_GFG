using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAnim : MonoBehaviour
{
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        if (anim != null)
        {
            anim.SetBool("Pet", true);
        }
    }
}
