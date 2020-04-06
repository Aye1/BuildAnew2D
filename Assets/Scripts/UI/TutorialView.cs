using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialView : MonoBehaviour
{
    #region Editor fields
    [SerializeField] private TextMeshProUGUI _tutorialText;
    #endregion

    public void SetText(string text)
    {
        _tutorialText.text = text;
    }
}
