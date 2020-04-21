using UnityEngine;
using System.Linq;

// Dependecies to other managers:
//   Hard dependencies:
//     LevelManager
//     DialogManager
//   Soft dependencies:
//     MouseManager

public enum TutorialState { Ready, Started, Finished };
public class TutorialManager : Manager
{
    public static TutorialManager Instance { get; private set; }
    private TutorialData _currentTutorialData;
    private int _currentStepIndex = 0;
    private TutorialState _tutorialState = TutorialState.Ready;
    private TutorialView _tutorialView;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitState = InitializationState.Initializing;
            LevelManager.OnLevelNeedReset += Reset;
            MouseManager.OnPlayerClick += ReadNextStep;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Reset();
        DialogManager.Instance.StartDialog(new DialogLine[] { new DialogLine("Yo, je teste les dialogues"), new DialogLine("J'ai deux lignes de texte") });
        InitState = InitializationState.Ready;
    }

    private void OnDestroy()
    {
        MouseManager.OnPlayerClick -= ReadNextStep;
        LevelManager.OnLevelNeedReset -= Reset;
    }

    private void Reset()
    {
        // TODO: find a better way to link with the tutorial view
        // Probably link the prefab and instantiate it directly
        // This won't work if the TutorialView is inactive
        _tutorialView = FindObjectOfType<TutorialView>();
        if (_tutorialView == null)
        {
            Debug.LogError("Tutorial View not found in the scene");
        }
        else
        {
            _tutorialView.gameObject.SetActive(false);
            _currentTutorialData = LevelManager.Instance.GetCurrentLevel().tutorialData;
            _currentStepIndex = 0;
            _tutorialState = TutorialState.Ready;
            ReadNextStep();
        }
    }

    private void ReadNextStep()
    {
        if (_currentTutorialData != null && _tutorialState != TutorialState.Finished)
        {
            TutorialStepData currentStep = _currentTutorialData.GetStep(_currentStepIndex);
            if (currentStep != null)
            {
                _tutorialState = TutorialState.Started;
                _tutorialView.gameObject.SetActive(true);
                _tutorialView.SetText(currentStep.requestText);
                if (currentStep.conditions.All(x => x.IsConditionVerified()))
                {
                    _currentStepIndex++;
                    if (_currentTutorialData.GetStepCount() == _currentStepIndex)
                    {
                        //tutorial is finished
                        _tutorialState = TutorialState.Finished;
                        _tutorialView.gameObject.SetActive(false);
                    }
                    else
                    {
                        ReadNextStep();
                    }
                }
            }
        }
    }
}
