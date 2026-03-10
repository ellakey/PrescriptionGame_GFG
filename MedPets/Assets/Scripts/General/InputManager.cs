using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public event StartTouchEvent OnStartTouch;
    public delegate void EndTouchEvent(Vector2 position, float time);
    public event EndTouchEvent OnEndTouch; 

    private TouchControls touchControls;
    private DragBehavior drag;
    private bool dragStarted;

    private void Awake(){
        touchControls = new TouchControls();
        drag = new DragBehavior();
    }

    private void Update()
    {
        if (dragStarted)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);
            if (hit.collider != null)
            {
                if (drag.isDragging() && hit.collider.gameObject.GetComponent<Berry>().getAdded() == false)
                {
                    drag.addDragged(gameObject);
                    hit.collider.gameObject.GetComponent<Berry>().setAdded(true);
                    drag.drawLines();
                }
            }
        }
    }

    private void OnEnable(){
        touchControls.Enable();
    }

    private void OnDisable(){
        touchControls.Disable();
    }

    private void Start(){
        
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context)
    {
        drag.onDown();
        dragStarted = true;
        Debug.Log("Touch started" + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        if(OnStartTouch != null) OnStartTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        drag.onUp();
        dragStarted = false;
        Debug.Log("Touch ended" + touchControls.Touch.TouchPosition.ReadValue<Vector2>());
        if(OnEndTouch != null) OnEndTouch(touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
    }
}