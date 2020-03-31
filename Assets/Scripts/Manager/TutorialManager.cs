using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;


public enum TutorialState { Ready, Started, Finished };
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    private TutorialData _currentTutorialData;
    private int _currentStepIndex = 0;
    private TutorialState _tutorialState = TutorialState.Ready;
    public TextMeshProUGUI _tutorialText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GameManager.OnLevelLoaded += Reset;
        LevelManager.OnLevelNeedReset += Reset;
        MouseManager.OnPlayerClick += ReadNextStep;
        _tutorialText.gameObject.SetActive(false);
    }

    private void Reset()
    {
        _currentTutorialData = GameManager.Instance.GetLevelData()._tutorialData;
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
                _tutorialText.text = currentStep._requestText;
                _tutorialText.gameObject.SetActive(true);
                if (currentStep._conditions.All(x => x.IsConditionVerified()))
                {
                    _currentStepIndex++;
                    if (_currentTutorialData.GetStepCount() == _currentStepIndex)
                    {
                        //tutorial is finished
                        _tutorialState = TutorialState.Finished;
                        _tutorialText.gameObject.SetActive(false);
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
