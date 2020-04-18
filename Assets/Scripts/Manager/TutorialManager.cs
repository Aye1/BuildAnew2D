using UnityEngine;
using System.Linq;

public enum TutorialState { Ready, Started, Finished };
public class TutorialManager : Manager
{
    public static TutorialManager Instance { get; private set; }
    private TutorialData _currentTutorialData;
    private int _currentStepIndex = 0;
    private TutorialState _tutorialState = TutorialState.Ready;

#pragma warning disable 0649
    [SerializeField] private TutorialView _tutorialView;
#pragma warning restore 0649

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        GameManager.OnLevelLoaded += Reset;
    }

    void Start()
    {
        //LevelManager.OnLevelNeedReset += Reset;
        MouseManager.OnPlayerClick += ReadNextStep;
        DialogManager.Instance.StartDialog(new DialogLine[] { new DialogLine("Yo, je teste les dialogues"), new DialogLine("J'ai deux lignes de texte") });
    }

    private void Reset()
    {
        _tutorialView.gameObject.SetActive(false);
        _currentTutorialData = GameManager.Instance.GetLevelData().tutorialData;
        _currentStepIndex = 0;
        _tutorialState = TutorialState.Ready;
        ReadNextStep();
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
