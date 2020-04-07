using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StructureControlView : MonoBehaviour
{
    #region Editor UI Bindings
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private ResourceInfo _energyInfo;
    [SerializeField] private EnergyIndicator _energyIndicator;
#pragma warning restore 0649
    #endregion

    #region Properties
    private StructureTile _structure;
    public StructureTile Structure
    {
        get
        {
            return _structure;
        }
        set
        {
            if (_structure != value)
            {
                if(_structure != null)
                {
                    // Unsuscribe from previous structure event
                    _structure.OnPropertyChanged -= OnStructurePropertyChanged;
                }
                _structure = value;
                if (_structure != null)
                {
                    _structure.OnPropertyChanged += OnStructurePropertyChanged;
                }
                UpdateUI();
            }
        }
    }
    #endregion

    private void Awake()
    {
        _upgradeButton.interactable = false;
        _sellButton.interactable = false;
    }

    private void UpdateUI()
    {
        if (_structure != null)
        {
            _typeText.text = _structure.GetText();
            _upgradeButton.interactable = _structure.CanUpgradeStructure() && !UIManager.Instance.IsBlocked;
            _sellButton.interactable = _structure.CanSellStructure() && !UIManager.Instance.IsBlocked;
            _energyInfo.SetAmount(_structure.structureData.consumedEnergyAmount);
            _energyIndicator.IsOn = _structure.IsOn;
        }
    }

    private void OnStructurePropertyChanged(string propertyName)
    {
        UpdateUI();
    }

    public void ToggleStructure()
    {
        _structure.IsOn = !_structure.IsOn;
    }
}
