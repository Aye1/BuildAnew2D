using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndGameConditionUI : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _genericText;
    [SerializeField] private LayoutGroup panel;
#pragma warning restore 0649
    #endregion
    List<TextMeshProUGUI> conditions;
    private bool isConditionDisplayed = false;
    public void Awake()
    {
        SetVisible(false);
    }

    public void ToggleConditions()
    {
        if(isConditionDisplayed)
        {
            HideConditions();
        }
        else
        {
            DisplayConditions();
        }
    }

    private void DisplayConditions()
    {
        SetVisible(true);
        LevelData levelData = GameManager.Instance.GetLevelData();
        LanguageConstantString language = UIManager.Instance.GetLanguageConstant();

        conditions = new List<TextMeshProUGUI>();
        InstantiateConditions(levelData.GetSuccessConditions(), language.sucessConditions);
        InstantiateConditions(levelData.GetDefeatConditions(), language.defeatConditions);
    }

    private void InstantiateConditions(List<BaseCondition> baseConditions, string titleCondition)
    {
        if(baseConditions != null && baseConditions.Count != 0)
        {
            TextMeshProUGUI title = Instantiate(_genericText, Vector3.zero, Quaternion.identity, panel.transform);
            conditions.Add(title);
            title.text = titleCondition;
            foreach (BaseCondition baseCondition in baseConditions)
            {
                TextMeshProUGUI genericCondition = Instantiate(_genericText, Vector3.zero, Quaternion.identity, panel.transform);
                conditions.Add(genericCondition);
                genericCondition.text = baseCondition.GetText();
            }
        }
    }

    public void HideConditions()
    {
        foreach(TextMeshProUGUI text in conditions)
        {
            Destroy(text.gameObject);
        }
        conditions.Clear();
        SetVisible(false);
    }

    private void SetVisible(bool visibility)
    {
        isConditionDisplayed = visibility;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
