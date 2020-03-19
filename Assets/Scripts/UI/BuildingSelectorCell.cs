using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class BuildingSelectorCell : MonoBehaviour
{
    private Button _button;
    [SerializeField] private Image _iconImage;
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
        GetComponentInChildren<TextMeshProUGUI>().text = building.data.StructureName;
        _iconImage.sprite = building.data.icon;
    }

    // Update is called once per frame
    void Update()
    {
        _button.interactable = ResourcesManager.Instance.CanPay(CostManager.CostForStructure(building.type));
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
