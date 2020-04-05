using TMPro;
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
    //[SerializeField] private Button _toggleButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private ResourceInfo _energyInfo;
    [SerializeField] private List<ErrorText> _errors;
    [SerializeField] private EnergyIndicator _energyIndicator;
#pragma warning restore 0649
    #endregion

    private Dictionary<ActivationState, string> _errorTextDico;
    private BaseTileData _previousTile;


    private bool _needRefresh = false;
    // Start is called before the first frame update
    void Start()
    {
        _errorText.gameObject.SetActive(false);
        _errorTextDico = new Dictionary<ActivationState, string>();
        foreach (ErrorText errorText in _errors)
        {
            _errorTextDico.Add(errorText.state, errorText.errorText);
        }
        _previousTile = null;
        MouseManager.OnPlayerClick += Refresh;
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (_needRefresh)
        {
            UpdateUI();
            _needRefresh = false;
        }
    }

    private void UpdateUI()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        bool isActive = selectedTile != null;
        SetVisible(isActive);
        if (isActive)
        {
            _posText.text = selectedTile.gridPosition.ToString();
            _typeText.text = selectedTile.GetTerrainText();
            _structureText.text = selectedTile.GetStructureText();
            //_toggleButton.gameObject.SetActive(selectedTile.structureTile != null);
            _upgradeButton.gameObject.SetActive(selectedTile.structureTile != null && selectedTile.structureTile.CanUpgradeStructure());
            _sellButton.gameObject.SetActive(selectedTile.structureTile != null && selectedTile.structureTile.CanSellStructure());
            _energyInfo.gameObject.SetActive(selectedTile.structureTile != null && selectedTile.structureTile.structureData.ConsumesEnergy);
            _energyIndicator.gameObject.SetActive(selectedTile.structureTile != null);
            if (selectedTile.structureTile != null)
            {
                _energyInfo.SetAmount(selectedTile.structureTile.structureData.consumedEnergyAmount);
                _energyIndicator.IsOn = selectedTile.structureTile.IsOn;
            }
        }
        _errorText.gameObject.SetActive(false);
        _previousTile = selectedTile;
    }

    public void ToggleStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        ActivationState activationState = selectedTile.ToggleStructureIfPossible();
        string currentErrorText;
        if (_errorTextDico.TryGetValue(activationState, out currentErrorText))
        {
            _errorText.gameObject.SetActive(true);
            _errorText.text = currentErrorText;
        }
        else
        {
            _errorText.gameObject.SetActive(false);
        }
        _energyIndicator.IsOn = selectedTile.structureTile.IsOn;
    }
    public void UpgradeStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        selectedTile.structureTile.UpgradeStructure();
        Refresh();
    }

    public void SellStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        selectedTile.structureTile.SellStructure(selectedTile.gridPosition);
        Refresh();
    }

    public void Refresh()
    {
        _needRefresh = true;
    }

    public void Hide()
    {
        SetVisible(false);
        Refresh();
    }
    private void SetVisible(bool visibility)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
