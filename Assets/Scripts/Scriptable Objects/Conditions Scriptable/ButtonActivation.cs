using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Condition/ButtonActivationConditionData", order = 1)]

//TODO find a way to reference button into scriptable object
public class ButtonActivation : BaseCondition
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private Button _button;
#pragma warning restore 0649
    #endregion
    private bool _isButtonClicked = false;

    public void Awake()
    {
        //_button.onClick.AddListener(() => OnButtonClick());
    }
    private void OnButtonClick()
    {
        _isButtonClicked = true;
    }
    public override string GetText()
    {
        return "Click on the button";
    }

    public override bool IsConditionVerified()
    {
        return _isButtonClicked;
    }
}
