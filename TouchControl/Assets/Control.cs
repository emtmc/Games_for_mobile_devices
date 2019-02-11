using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    private float dist;
    private bool dragging = false;
    private Transform toDrag;
    private bool isTap;
    public Selector cam;

    void Update()
    {
       Vector3 position;    
                   
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
            
                if(Physics.Raycast(ray, out hit))
                {
                   // Scale objectTouched = hit.collider.GetComponent<Scale>();
                   // if (objectTouched) objectTouched.Update();                 
                    toDrag = hit.transform;
                    dist = hit.transform.position.z - Camera.main.transform.position.z;
             
                    dragging = true;
                    if (Input.touchCount == 2)
                    {
                        touch = Input.GetTouch(1);
                    }
                }
    
        }
        if (dragging && touch.phase == TouchPhase.Moved) {
           position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
           position = Camera.main.ScreenToWorldPoint(position);
           toDrag.position = position;
        }
        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)) {
            dragging = false;
        }
    }
}
