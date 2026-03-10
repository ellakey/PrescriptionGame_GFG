using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchOver : MonoBehaviour
{

    [SerializeField] DragBehavior drag;
    [SerializeField] Berry berry;

    private void Start()
    {
        drag = GameObject.FindWithTag("Detector").GetComponent<DragBehavior>();
    }

    private void OnMouseOver()
    {
        if (drag.IsDragging && berry.Added == false)
        {
            drag.AddDragged(gameObject);
            berry.Added = true;
            drag.DrawLines();
        }
        
    }

}
