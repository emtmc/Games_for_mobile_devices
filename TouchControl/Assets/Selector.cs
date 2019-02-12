using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Selector : MonoBehaviour
{
    private Camera cam = null;
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.050f;        // The rate of change of the orthographic size in orthographic mode.
    public Selectable selected = null;
    private bool gyroEnabled;
    private Gyroscope gyro;
    private GameObject cameraContainer;
    private Quaternion rot;
    public float speed = 0.1F;
    
    public void Start()
    {
        cam = this.GetComponent<Camera>();
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        
    }

    public void Update()
    {
        //If an object is selected
        if (selected != null)
        {
        selected.scale();
        selected.move();
        }
        else
        {
        Zoom();
        CameraMove();
       // Accelerometer();
       // Gyroscope();
        }
  
        //if the screen is being touched 
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
          //  Debug.Log("touch detected");
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
               // Debug.Log("Object hit");
                Selectable s = hit.transform.GetComponent<Selectable>();
                if (s != null)
                {
                  //  Debug.Log("Selectable Found");
                    if(this.selected != null) this.selected.onDeselect();
                    this.selected = s;
                    s.onSelect();
                }
            }
            else
            {
                if(this.selected != null) this.selected.onDeselect();
                    this.selected = null;                
            }
            
        }
    }

    void Zoom()
    {
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

             
            if (cam.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
            }
            else
            {
                // Otherwise change the field of view based on the change in distance between the touches.
                cam.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;

                // Clamp the field of view to make sure it's between 0 and 180.
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
            }
        }
    }

    void Accelerometer()
    {
        transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
    }
    
    
   private bool EnableGyro()
    {
        
 
            if (SystemInfo.supportsGyroscope)
            {
                gyro = Input.gyro;
                gyro.enabled = true;
                cameraContainer.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
                rot = new Quaternion(0, 0, 1, 0);
                return true;
            }

            return false;
        
    }
    // Update is called once per frame
    void Gyroscope()
    {
        
        transform.SetParent(cameraContainer.transform); 
        gyroEnabled = EnableGyro();
        
        if (gyroEnabled)
        {
            transform.localRotation = gyro.attitude * rot;
        }
    }


    void CameraMove()
    {
        if (selected == null)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                transform.Translate(-touchDeltaPosition.x * speed * Time.deltaTime,
                    -touchDeltaPosition.y * speed * Time.deltaTime, 0);
            }
        }
    }    
}
