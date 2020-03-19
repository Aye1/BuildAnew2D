using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BuildingSelectorCell : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Image _iconImage;
#pragma warning restore 0649

    private Button _button;
    public BuildingBinding building;
    public delegate void ClickDelegate(BuildingSelectorCell sender);

    // Start is called before the first frame update
    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void InitWithBuilding(BuildingBinding building)
    {
        this.building = building;
        GetComponentInChildren<TextMeshProUGUI>().text = building.data.structureName;
        _iconImage.sprite = building.data.icon;
    }

    // Update is called once per frame
    void Update()
    {
        _button.interactable = ResourcesManager.Instance.CanPay(building.data.costs);
    }

    public void RegisterButtonOnClick(ClickDelegate methodToCall) 
    {
        _button.onClick.AddListener(() => methodToCall(this));
    }

    public void UnregisterButtonOnClick(ClickDelegate methodToCall)
    {
        _button.onClick.RemoveListener(() => methodToCall(this));
    }
}
