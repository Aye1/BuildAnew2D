using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _woodText;
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TMP_Dropdown _buildTypeDropdown;
    [SerializeField] private TextMeshProUGUI _buildButtonText;
    [SerializeField] private Button _undoButton;
    private Dictionary<int, BuildingBinding> _optionsDico;

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

    private void Start()
    {
        CreateDropdownList();
    }

    private void CreateDropdownList()
    {
        IEnumerable<BuildingBinding> allConstructibleBuildings = TilesDataManager.Instance.GetAllConstructiblesStructures();
        _buildTypeDropdown.ClearOptions();
        int index = 0; //Index of dropdow starts at 1
        List<string> texts = new List<string>();
        _optionsDico = new Dictionary<int, BuildingBinding>();
        foreach (BuildingBinding binding in allConstructibleBuildings)
        {
            _optionsDico.Add(index, binding);
            texts.Add(binding.data.structureName);
            index++;
        }
        _buildTypeDropdown.AddOptions(texts);
    }
    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _woodText.text = ResourcesManager.Instance.WoodAmount.ToString();
        _energyText.text = ResourcesManager.Instance.EnergyAvailable.ToString() + "/" + ResourcesManager.Instance.EnergyTotal.ToString();
        _buildButtonText.text = BuildingManager.Instance.IsInBuildMode ? "Stop building" : "Start building";
        _undoButton.interactable = CommandManager.Instance.CanUndoLastCommand();
    }

    /*public StructureType GetSelectedStructureType()
    {
        StructureType returnType = StructureType.None;
        BuildingBinding buildingBinding;
        if(_optionsDico.TryGetValue(_buildTypeDropdown.value, out buildingBinding))
        {
            returnType = buildingBinding.type;
        }
        return returnType;
    }*/

    public void ToggleBuildMode()
    {
        BuildingManager.Instance.IsInBuildMode = !BuildingManager.Instance.IsInBuildMode;
    }
}
