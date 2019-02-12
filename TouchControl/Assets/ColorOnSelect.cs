using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ColorOnSelect : Selectable
{
    public Transform sphere;
    public Selector cam;
    private Camera camera;
    private float dist;
    private bool dragging = false;
    private Transform toDrag;
    private bool isTap;
    private bool isSelected = false;
    public float orthoZoomSpeed = 0.050f;   
    public float rotationRate = 1f;
    public bool wasRotating;
    
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    public override void onSelect()
    {
        isSelected = true;
        this.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void onDeselect()
    {
        isSelected = false;
        this.GetComponent<Renderer>().material.color = Color.white;
    }
    
    public override void scale()
    {
        if (isSelected)
        {
            // If there are two touches on the device...
            if (Input.touchCount == 2)
            {
                // Store both touches.
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Find the position in the previous frame of each touch.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // Find the magnitude of the vector (the distance) between the touches in each frame.
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                if (cam.selected != null)
                    cam.selected.transform.localScale += Vector3.one * deltaMagnitudeDiff * orthoZoomSpeed;            
            }
        }
    }

    public override void move()
    {        
        if (isSelected == true)
        {
            Vector3 position;
            
        //if there's less then 1 touch, there's no dragging
            if (Input.touchCount < 1)
            {
                dragging = false;
                return;
            }

            var touch = Input.touches[0];
            Vector2 pos = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(pos);
                if (Physics.Raycast(ray, out hit))
                {          
                    toDrag = hit.transform;
                    dist = hit.transform.position.z - Camera.main.transform.position.z;
                    dragging = true;
                    if (Input.touchCount == 2)
                    {
                        touch = Input.GetTouch(1);
                    }
                }
            }
            if (dragging && touch.phase == TouchPhase.Moved)
            {
                position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
                position = Camera.main.ScreenToWorldPoint(position);
                toDrag.position = position;
            }
            if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                dragging = false;
            }
        }
    }


    public override void rotateObj()
    {
        if (isSelected == true)
        {
            if (Input.touchCount == 2)
            {
                  Touch touchzero = Input.GetTouch(0);
                  Touch touchone = Input.GetTouch(1);

                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    wasRotating = false;
                }

                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    transform.Rotate(0, Input.touches[0].deltaPosition.x * rotationRate, 0, Space.World);
                    wasRotating = true;
                }
            
            }
        }
    }
}
