using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenericButton : MonoBehaviour
{
    public TextMeshProUGUI _text;
    public Button _button;
    public delegate void ClickDelegate(GenericButton sender);
    public void SetText(string text)
    {
        _text.text = text;
    }
    public void SetOnClickCallback(ClickDelegate methodToCall) 
    {
         _button.onClick.AddListener(() => methodToCall(this));
    }
}
