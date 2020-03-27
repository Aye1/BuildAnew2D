using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TooltipBuildingInfo : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _buildingNameText;
    [SerializeField] private GameObject childObject;
#pragma warning restore 0649
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        childObject.SetActive(UIManager.Instance.HoveredStructure != StructureType.None);
    }

}
