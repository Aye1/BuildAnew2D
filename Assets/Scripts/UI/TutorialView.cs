using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialView : MonoBehaviour
{
    #region Editor Bindings
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _tutorialText;
#pragma warning restore 0649
    #endregion

    public void SetText(string text)
    {
        _tutorialText.text = text;
    }
}
