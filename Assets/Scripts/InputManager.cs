using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public KeyboardMapping keyboardMapping;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public KeyCode GetKeyCode(string name)
    {
        try
        {
            return (KeyCode)keyboardMapping.GetType().GetField(name).GetValue(keyboardMapping);
        }
        catch (Exception e)
        {
            Debug.LogError("Binding not found for key " + name);
            return KeyCode.None;
        }
    }

    public bool GetKey(string name)
    {
        return Input.GetKey(GetKeyCode(name));
    }

    public bool GetKeyDown(string name)
    {
        return Input.GetKeyDown(GetKeyCode(name));
    }

    public bool GetKeyUp(string name)
    {
        return Input.GetKeyUp(GetKeyCode(name));
    }
}
