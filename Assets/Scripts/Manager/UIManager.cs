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
    [SerializeField] private TextMeshProUGUI _endGameText;
    [SerializeField] private Button _undoButton;
    [SerializeField] private LanguageConstantString _languageTexts;

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
        _endGameText.enabled = false;
    } 

    private void Start()
    {
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _woodText.text = ResourcesManager.Instance.GetResourceForType(ResourceType.Wood).amount.ToString();
        _energyText.text = ResourcesManager.Instance.EnergyAvailable.ToString() + "/" + ResourcesManager.Instance.EnergyTotal.ToString();
        _undoButton.interactable = CommandManager.Instance.CanUndoLastCommand();
    }

    public void ToggleBuildMode()
    {
        BuildingManager.Instance.IsInBuildMode = !BuildingManager.Instance.IsInBuildMode;
    }

    public void TriggerGameOver()
    {
        _endGameText.enabled = true;
        _endGameText.text = _languageTexts.lossText;
    }
    public void TriggerGameSuccess()
    {
        _endGameText.enabled = true;
        _endGameText.text = _languageTexts.winText;
    }
}
