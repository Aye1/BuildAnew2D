using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TextMeshProUGUI _endGameText;
    [SerializeField] private GameObject _endGamePanel;
    [SerializeField] private Button _undoButton;
    [SerializeField] private LanguageConstantString _languageTexts;
    [SerializeField] private ResourcesList _resourceList;
    [SerializeField] private Button _nextTurnButton;

    [Header("UI References")]
    [SerializeField] private InfoMenu _infoMenu;
    [SerializeField] private TooltipBuildingInfo _tooltipBuildingInfo;
    [SerializeField] private EndGameConditionUI _endGameConditionsUI;
    [SerializeField] private Transform _buildingPanel;

#pragma warning restore 0649
    #endregion

    public StructureType HoveredStructure { get; set; }
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
        _endGameText.enabled = false;
    } 

    private void Start()
    {
        ResetUI();
        InitializeResourcesLayout();
        InitButtons();
        ResourcesManager.OnResourcesModification += OnResourcesModification;
    }

    private void Update()
    {
        UpdateUI();
    }

    private void InitButtons()
    {
        _nextTurnButton.onClick.AddListener(TurnManager.Instance.NextTurn);
    }

    private void UpdateUI()
    {
        _energyText.text = ResourcesManager.Instance.EnergyAvailable.ToString() + "/" + ResourcesManager.Instance.EnergyTotal.ToString();
        _undoButton.interactable = CommandManager.Instance.CanUndoLastCommand();
    }

    public void ResetUI()
    {
        _infoMenu.Hide();
        _tooltipBuildingInfo.SetVisible(false);
        _endGameConditionsUI.HideConditions();
        _endGameText.enabled = false;
        _endGamePanel.SetActive(false);
    }

    public void ToggleBuildMode()
    {
        BuildingManager.Instance.IsInBuildMode = !BuildingManager.Instance.IsInBuildMode;
    }

    public void ToggleBuildingSelector()
    {
        _buildingPanel.gameObject.SetActive(!_buildingPanel.gameObject.activeInHierarchy);
    }

    public void TriggerGameOver()
    {
        _endGamePanel.SetActive(true);
        _endGameText.enabled = true;
        _endGameText.text = _languageTexts.lossText;
    }
    public void TriggerGameSuccess()
    {
        _endGamePanel.SetActive(true);
        _endGameText.enabled = true;
        _endGameText.text = _languageTexts.winText;
    }

    public LanguageConstantString GetLanguageConstant()
    {
        return _languageTexts;
    }

    public void InitializeResourcesLayout()
	{
        _resourceList.CreateResourcesList(ResourcesManager.Instance.GetCurrentResource());
    }

    private void OnResourcesModification()
    {
        _resourceList.RefreshResourcesList(ResourcesManager.Instance.GetCurrentResource());
	}
}
