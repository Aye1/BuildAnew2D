using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class ZoomCamera : MonoBehaviour
{
    public float zoomSpeed = 0.05f;

    private Camera _camera;

    // Use this for initialization
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
        Vector3 currentScale = transform.localScale;
        Vector3 newScale = currentScale;

        if (InputManager.Instance.GetKey("zoomCameraOut"))
        {
            newScale += Vector3.one * zoomSpeed; ;
        }
        if (InputManager.Instance.GetKey("zoomCameraIn"))
        {
            newScale += Vector3.one * -1 * zoomSpeed;
        }

        _camera.transform.localScale = Vector3.MoveTowards(currentScale, newScale, 1.0f);
    }
}
