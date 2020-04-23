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
    [Header("Module Upgrade Section")]
    [SerializeField] private ModuleUpgrade _genericModuleUpgrade;
    [SerializeField] private HorizontalLayoutGroup _moduleGroup;

    [Header("Energy Section")]
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

    private List<ModuleUpgrade> modulesUpgrade;

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
            DisplayButton(_upgradeButton, _structure.CanUpgradeStructure(), _structure.HasNextLevelUpgrade());
            DisplayButton(_sellButton, _structure.CanSellStructure(), _structure.HasSellingData());
            DisplayModuleUpgrade();
            _energyInfo.SetAmount(_structure.structureData._energyAmount);
            _energyIndicator.IsOn = _structure.IsOn;
        }
    }

    private void DisplayButton(Button button, bool isEnabled, bool isVisible)
    {
        button.enabled = isEnabled;
        button.gameObject.SetActive(isVisible);
        if(isEnabled)
        {
            button.interactable = !UIManager.Instance.IsBlocked;
        }
    }
    private void DisplayModuleUpgrade()
    {
        if(modulesUpgrade != null && modulesUpgrade.Count > 0)
        {
            foreach(ModuleUpgrade upgrade in modulesUpgrade)
            {
                Destroy(upgrade.gameObject);
            }
        }
        modulesUpgrade = new List<ModuleUpgrade>();
        foreach(AbstractModuleScriptable abstractModuleData in _structure.structureData.availableModules)
        {
            ModuleUpgrade newModule = Instantiate(_genericModuleUpgrade, Vector3.zero, Quaternion.identity, _moduleGroup.transform);
            newModule.InitModule(abstractModuleData, _structure);
            modulesUpgrade.Add(newModule);
        }
    }
    private void OnStructurePropertyChanged(string propertyName)
    {
        UpdateUI();
    }

}
