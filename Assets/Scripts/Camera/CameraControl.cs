using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private int _sensitivity = 5;

    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.eulerAngles = transform.eulerAngles - (_sensitivity * new Vector3(0, -Input.GetAxis("Mouse X"), 0f));
    }
}
