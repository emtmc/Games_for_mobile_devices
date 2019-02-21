using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraClass : MonoBehaviour
{
    private Camera cam = null;
    public float perspectiveZoomSpeed = 0.05f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.050f;        // The rate of change of the orthographic size in orthographic mode.
    public Selectable selected = null;
    private bool gyroEnabled;
    private Gyroscope gyro;
    private GameObject cameraContainer;
    private Quaternion rot;
    public float speed = 0.5f;
    public float rotationRate = 0.5f;
    public bool wasRotating;
    const float pinchTurnRatio = Mathf.PI / 2;
    const float minTurnAngle = 0;
    const float pinchRatio = 0.5f;
    const float minPinchDistance = 0;
    const float panRatio = 0.5f;
    const float minPanDistance = 0;
    public void Start()
    {
        cam = this.GetComponent<Camera>();
        cameraContainer = new GameObject("Camera Container");
        cameraContainer.transform.position = transform.position;
        
    }

    public void Update()
    {
        if (selected != null)
        {
        selected.move();
        selected.ScaleorRotate();
        }
        else
        {
        ZoomorRotate();
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

    
    void ZoomorRotate() {
        float pinchAmount = 0;
        Quaternion desiredRotation = transform.rotation;
 
        Calculate();
 
        if (Mathf.Abs(pinchDistanceDelta) > 0) { // zoom
            pinchAmount = pinchDistanceDelta;
            Zoom();
        }
 
        if (Mathf.Abs(turnAngleDelta) > 0) { // rotate
            Vector3 rotationDeg = Vector3.zero;
            rotationDeg.z = -turnAngleDelta;
            desiredRotation *= Quaternion.Euler(rotationDeg);
            rotate();
        }
 
 
        // not so sure those will work:
        transform.rotation = desiredRotation;
        transform.position += Vector3.forward * pinchAmount;
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

    void rotate()
    {
        if (Input.touchCount == 2)
        {
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
    
    // Differentiating between pinch and rotate
    /// <summary>
	///   The delta of the angle between two touch points
	/// </summary>
	static public float turnAngleDelta;
	/// <summary>
	///   The angle between two touch points
	/// </summary>
	static public float turnAngle;
 
	/// <summary>
	///   The delta of the distance between two touch points that were distancing from each other
	/// </summary>
	static public float pinchDistanceDelta;
	/// <summary>
	///   The distance between two touch points that were distancing from each other
	/// </summary>
	static public float pinchDistance;
 
	/// <summary>
	///   Calculates Pinch and Turn - This should be used inside LateUpdate
	/// </summary>
	static public void Calculate () {
		pinchDistance = pinchDistanceDelta = 0;
		turnAngle = turnAngleDelta = 0;
 
		// if two fingers are touching the screen at the same time ...
		if (Input.touchCount == 2) {
			Touch touch1 = Input.touches[0];
			Touch touch2 = Input.touches[1];
 
			// ... if at least one of them moved ...
			if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved) {
				// ... check the delta distance between them ...
				pinchDistance = Vector2.Distance(touch1.position, touch2.position);
				float prevDistance = Vector2.Distance(touch1.position - touch1.deltaPosition,
				                                      touch2.position - touch2.deltaPosition);
				pinchDistanceDelta = pinchDistance - prevDistance;
 
				// ... if it's greater than a minimum threshold, it's a pinch!
				if (Mathf.Abs(pinchDistanceDelta) > minPinchDistance) {
					pinchDistanceDelta *= pinchRatio;
				} else {
					pinchDistance = pinchDistanceDelta = 0;
				}
 
				// ... or check the delta angle between them ...
				turnAngle = Angle(touch1.position, touch2.position);
				float prevTurn = Angle(touch1.position - touch1.deltaPosition,
				                       touch2.position - touch2.deltaPosition);
				turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);
 
				// ... if it's greater than a minimum threshold, it's a turn!
				if (Mathf.Abs(turnAngleDelta) > minTurnAngle) {
					turnAngleDelta *= pinchTurnRatio;
				} else {
					turnAngle = turnAngleDelta = 0;
				}
			}
		}
	}
 
	static private float Angle (Vector2 pos1, Vector2 pos2) {
		Vector2 from = pos2 - pos1;
		Vector2 to = new Vector2(1, 0);
 
		float result = Vector2.Angle( from, to );
		Vector3 cross = Vector3.Cross( from, to );
 
		if (cross.z > 0) {
			result = 360f - result;
		}
 
		return result;
	}
}
