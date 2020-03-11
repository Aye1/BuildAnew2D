using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MoveCamera : MonoBehaviour
{
    private Camera _camera;
    public float cameraMoveSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        ManageInputs();
    }

    private void ManageInputs()
    {
        Vector3 currentPos = transform.position;
        Vector3 newPos = currentPos;
        if(Input.GetKey(KeyCode.UpArrow))
        {
            newPos += Vector3.up * cameraMoveSpeed;
        }
        if(Input.GetKey(KeyCode.DownArrow)) 
        {
            newPos += Vector3.down * cameraMoveSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            newPos += Vector3.left * cameraMoveSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            newPos += Vector3.right * cameraMoveSpeed;
        }
        _camera.transform.position = Vector3.MoveTowards(currentPos, newPos, 1.0f);
    }
}
