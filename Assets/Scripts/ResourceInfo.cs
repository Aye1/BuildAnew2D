using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceInfo : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _resourceText;
    [SerializeField] private Image _resourceIcon;
#pragma warning restore 0649
    #endregion
    public Cost cost; 
    
    public void Initialize(Cost resource)
    {
        cost = resource;
        _resourceText.text = resource.amount.ToString();
        _resourceIcon.sprite = ResourcesManager.Instance.GetResourceDataForType(resource.type).icon;
    }

    public void SetAmount(int newAmount)
    {
        cost.amount = newAmount;
        _resourceText.text = newAmount.ToString();
    }
}
