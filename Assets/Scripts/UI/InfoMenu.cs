﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ErrorText
{
    public ActivationState state;
    public string errorText;
}

public class InfoMenu : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _posText;
    [SerializeField] private TextMeshProUGUI _structureText;
    [SerializeField] private TextMeshProUGUI _errorText;
    [SerializeField] private Button _toggleButton;
    [SerializeField] private List<ErrorText> _errors;
    private Dictionary<ActivationState, string> _errorTextDico;
    private BaseTileData _previousTile;
#pragma warning restore 0649
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _errorText.text = "";
        _errorTextDico = new Dictionary<ActivationState, string>();
        foreach (ErrorText errorText in _errors)
        {
            _errorTextDico.Add(errorText.state, errorText.errorText);
        }
        _previousTile = null; 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
            bool isActive = selectedTile != null;
            SetVisible(isActive);
        if (_previousTile != selectedTile)
        {
            if (isActive)
            {
                _posText.text = selectedTile.gridPosition.ToString();
                _typeText.text = selectedTile.GetTerrainText();
                _structureText.text = selectedTile.GetStructureText();
                _toggleButton.gameObject.SetActive(selectedTile.structureTile != null);
                
            }
            _errorText.text = "";
            _previousTile = selectedTile;
        }
    }

    public void ToggleStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        ActivationState activationState = selectedTile.ToggleStructureIfPossible();
        string currentErrorText;
        if(_errorTextDico.TryGetValue(activationState, out currentErrorText))
        {
            _errorText.text = currentErrorText;
        }
        else
        {
            _errorText.text = "";
        }
    }

    private void SetVisible(bool visibility)
    { 
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
