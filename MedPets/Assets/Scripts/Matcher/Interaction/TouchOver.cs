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
        if (!drag.IsDragging) return;
        if (drag.TutorialFingerActive) return;

        if (!berry.Added)
        {
            // Normal case: add this berry to the chain
            drag.AddDragged(gameObject);
            berry.Added = true;
            drag.DrawLines();
        }
        else if (drag.IsSecondToLast(berry))
        {
            // Player dragged back over the previous berry — undo the last one
            drag.UndoLast();
        }
    }
}