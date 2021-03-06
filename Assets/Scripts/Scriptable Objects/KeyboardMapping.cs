﻿using UnityEngine;

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
    [Header("Tactical View")]
    public KeyCode constructibleView = KeyCode.C;
    [Header("Menus")]
    public KeyCode buildMenu = KeyCode.B;
    public KeyCode pauseMenu = KeyCode.Escape;
    [Header("Buildings")]
    public KeyCode buildPowerPlant = KeyCode.P;
    public KeyCode buildSawmill = KeyCode.S;
    public KeyCode buildRelay = KeyCode.R;
    public KeyCode buildPumpingStation = KeyCode.W;
    public KeyCode buildMine = KeyCode.M;
    [Header("Other")]
    public KeyCode nextTurn = KeyCode.Space;
    [Header("CheatCode")]
    public KeyCode cheatResources = KeyCode.Equals;
}
