﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TutorialStepData", order = 1)]
public class TutorialStepData : ScriptableObject
{
    public List<BaseCondition> _conditions;
    public string _requestText;
}