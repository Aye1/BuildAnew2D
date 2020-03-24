using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LanguageConstantStringData", order = 1)]
public class LanguageConstantString : ScriptableObject
{
    [Header("EndGame text")]
    public string winText;
    public string lossText;


    [Header("Conditions text")]
    public string sucessConditions;
    public string defeatConditions;
    public string getResources;
    public string buildingActivated;

    [Header("Resources text")]
    public string resourcesTitle;

}
