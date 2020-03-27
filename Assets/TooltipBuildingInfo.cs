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
    private StructureType previousDisplayedType = StructureType.None;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StructureType currentType = UIManager.Instance.HoveredStructure;
        childObject.SetActive(currentType != StructureType.None);

        if(currentType != previousDisplayedType)
        {
            previousDisplayedType = currentType;
            if(currentType != StructureType.None)
            {
                StructureBinding binding = TilesDataManager.Instance.GetStructureBindingFromType(currentType);
                _buildingNameText.text = binding.data.structureName;

            }
        }
    }

}
