using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private EndGameView _endGamePanel;
    [SerializeField] private Button _undoButton;
    [SerializeField] private LanguageConstantString _languageTexts;
    [SerializeField] private ResourcesList _resourceList;
    [SerializeField] private Button _nextTurnButton;
    [SerializeField] private Transform _buildingPanel;
    [SerializeField] private Transform viewsHolder;

    [Header("Prefab bindings")]
    [SerializeField] private InfoMenu _infoMenu;
    [SerializeField] private TooltipBuildingInfo _tooltipBuildingInfo;
    [SerializeField] private EndGameConditionUI _endGameConditionsUI;
    [SerializeField] private BuildingSelector _buildingSelector;
    [SerializeField] private PauseMenuView _pauseMenuView;
    [SerializeField] private SettingsView _settingsView;
#pragma warning restore 0649
    #endregion

    public StructureType HoveredStructure { get; set; }
    public static UIManager Instance { get; private set; }

    private float _unblockTime;
    private PauseMenuView _instantiatedPauseMenu;
    public bool IsBlocked;


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
        ResetUI();
        InitializeResourcesLayout();
        InitButtons();
        ResourcesManager.OnResourcesModification += OnResourcesModification;
    }

    private void Update()
    {
        CatchInputs();
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
        _endGamePanel.gameObject.SetActive(false);
        HideBuildingSelector();
    }

    private void CatchInputs()
    {
        if(InputManager.Instance.GetKeyDown("buildMenu"))
        {
            ToggleBuildingSelectorVisibility();
        }

        if(InputManager.Instance.GetKeyDown("pauseMenu"))
        {
            if (_instantiatedPauseMenu == null)
            {
                OpenPauseMenu();
            }
            else
            {
                _instantiatedPauseMenu.CloseView();
            }
        }
    }

    public void RequestBlockUI(float timeSeconds)
    {
        _unblockTime = Mathf.Max(_unblockTime, Time.time + timeSeconds);
        StopAllCoroutines();
        StartCoroutine(BlockUICoroutine());
    }

    private IEnumerator BlockUICoroutine()
    {
        IsBlocked = true;
        while(Time.time < _unblockTime)
        {
            yield return new WaitForSeconds(0.1f);
        }
        IsBlocked = false;
    }

    public void ShowBuildingSelector()
    {
        _buildingSelector.gameObject.SetActive(true);
        _buildingPanel.gameObject.SetActive(true);
        TacticalViewManager.Instance.TriggerShowConstructibleView();
    }

    public void HideBuildingSelector()
    {
        // Only disabling the panel does not call OnDisable() on the selector
        // Feel free to investigate if you want
        // I guess the best solution would have to manage everything in another script on the panel itself
        _buildingSelector.gameObject.SetActive(false);
        _buildingPanel.gameObject.SetActive(false);
        TacticalViewManager.Instance.TriggerHideConstructibleView();
    }

    public void ToggleBuildingSelectorVisibility()
    {
        if(_buildingPanel.gameObject.activeInHierarchy)
        {
            HideBuildingSelector();
        }
        else
        {
            ShowBuildingSelector();
        }
    }

    public void TriggerEndGame()
    {
        _endGamePanel.gameObject.SetActive(true);
    }

    public void OpenPauseMenu()
    {
        _instantiatedPauseMenu = Instantiate(_pauseMenuView, Vector3.zero, Quaternion.identity, viewsHolder);
        _instantiatedPauseMenu.transform.localPosition = Vector3.zero;
    }

    public void OpenSettingsView()
    {
        SettingsView view = Instantiate(_settingsView, Vector3.zero, Quaternion.identity, viewsHolder);
        view.transform.localPosition = Vector3.zero;
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
