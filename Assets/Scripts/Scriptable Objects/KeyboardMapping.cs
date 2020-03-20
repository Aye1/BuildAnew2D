using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/KeyboardMapping", order = 1)]
public class KeyboardMapping : ScriptableObject
{
    [Header("Camera")]
    public KeyCode moveCameraUp = KeyCode.UpArrow;
    public KeyCode moveCameraDown = KeyCode.DownArrow;
    public KeyCode moveCameraLeft = KeyCode.LeftArrow;
    public KeyCode moveCameraRight = KeyCode.RightArrow;
    public KeyCode zoomCameraIn = KeyCode.Plus;
    public KeyCode zoomCameraOut = KeyCode.Minus;
}
