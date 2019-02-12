using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject gObj;
    private float dist;
    private bool dragging = false;
    private Transform toDrag;
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.050f;        // The rate of change of the orthographic size in orthographic mode.
    public Transform obj;
    
    
    void Start()
    {
        obj = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount < 1)
        {
            return;
        }

        Vector3 position;
        var touch = Input.touches[0];
        Vector2 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log(hit.transform.name);

               // gObj = hit.transform.gameObject;
                
                toDrag = hit.transform;
                dist = hit.transform.position.z - Camera.main.transform.position.z;

                dragging = true;

                
            }
        }
        scale();
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


        void scale()
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

                obj.localScale += Vector3.one * deltaMagnitudeDiff * orthoZoomSpeed;
            }

        }    
    }
}

