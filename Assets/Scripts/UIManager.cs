using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _woodText;
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TMP_Dropdown _buildTypeDropdown;
#pragma warning restore 0649
    #endregion

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _woodText.text = ResourcesManager.Instance.WoodAmount.ToString();
        _energyText.text = ResourcesManager.Instance.EnergyAvailable.ToString() + "/" + ResourcesManager.Instance.EnergyTotal.ToString();
    }

    public StructureType GetSelectedStructureType()
    {
        int selectedValue = _buildTypeDropdown.value;
        StructureType returnType = StructureType.None;
        if (selectedValue == 1)
        {
            returnType = StructureType.PowerPlant;
        } 
        if(selectedValue == 2)
        {
            returnType = StructureType.Sawmill;
        }
        if (selectedValue == 3)
        {
            returnType = StructureType.PumpingStation;
        }
        return returnType;
    }
}
