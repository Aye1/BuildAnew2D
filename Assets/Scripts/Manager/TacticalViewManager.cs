using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dependecies to other managers:
//   Hard dependencies:
//     InputManager (could be moved in another manager)

public class TacticalViewManager : Manager
{
    public static TacticalViewManager Instance { get; private set; }

    #region Events
    public delegate void ShowConstructibleView();
    public static event ShowConstructibleView OnShowConstructibleView;

    public delegate void HideConstructibleView();
    public static event HideConstructibleView OnHideConstructibleView;
    #endregion

    private bool _isConstructibleViewVisible = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitState = InitializationState.Ready;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.Instance.GetKeyDown("constructibleView"))
        {
            ToggleConstructibleView();
        }
    }

    public void ToggleConstructibleView()
    {
        if(_isConstructibleViewVisible)
        {
            TriggerHideConstructibleView();
        }
        else
        {
            TriggerShowConstructibleView();
        }
    }

    public bool IsConstructibleViewVisible()
    {
        return _isConstructibleViewVisible;
    }
    public void TriggerShowConstructibleView()
    {
        _isConstructibleViewVisible = true;
        OnShowConstructibleView?.Invoke();
    }
    public void TriggerHideConstructibleView()
    {
        _isConstructibleViewVisible = false;
        OnHideConstructibleView?.Invoke();
    }
}
