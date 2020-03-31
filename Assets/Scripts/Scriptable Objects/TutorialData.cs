using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TutorialData", order = 1)]
public class TutorialData : ScriptableObject
{
    public List<TutorialStepData> _tutorialSetps;
    public TutorialStepData GetStep(int index)
    {
        TutorialStepData returnStep = null;
        if(index < _tutorialSetps.Count)
        {
            returnStep = _tutorialSetps[index];
        }
        return returnStep;
    }
    public int GetStepCount()
    {
        return _tutorialSetps.Count;
    }
}
