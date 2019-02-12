using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    private float _sensitivity = 1f;

    private Vector3 _mouseReference;

    private Vector3 _mouseOffset;

    private Vector3 _rotation = Vector3.zero;

    private bool _isRotating;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isRotating)
        {
            _mouseOffset = (Input.mousePosition - _mouseReference);

            _rotation.y = -(_mouseOffset.x + _mouseOffset.y) * _sensitivity;

            gameObject.transform.Rotate(_rotation);

            _mouseReference = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        _isRotating = true;
        _mouseReference = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        _isRotating = false;
    }
}
