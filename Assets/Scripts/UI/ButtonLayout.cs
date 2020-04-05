using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonLayout : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _nextLevelButton;
#pragma warning restore 0649
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _resetButton.onClick.AddListener(LevelManager.Instance.ResetCurrentLevel);
        _nextLevelButton.onClick.AddListener(LevelManager.Instance.LoadNextLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
