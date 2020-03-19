using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoMenu : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _posText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _structureText;
    [SerializeField] private bool _activateToggle;
    [SerializeField] private Button _toggleButton;
#pragma warning restore 0649
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
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
        if(isActive)
        {
            _posText.text = selectedTile.gridPosition.ToString();
            _typeText.text = selectedTile.GetTerrainText();
            _structureText.text = selectedTile.GetStructureText();
            _activateToggle = selectedTile.IsStructureOn();
            if(selectedTile.structureTile != null)
            {
                _structureText.enabled = true;
                _toggleButton.gameObject.SetActive(true);
            }
            else
            {
                _structureText.enabled = false;
                _toggleButton.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        selectedTile.structureTile.IsOn = !selectedTile.structureTile.IsOn;
    }

    private void SetVisible(bool visibility)
    { 
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
