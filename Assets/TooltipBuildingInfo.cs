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
    [SerializeField] private LayoutGroup panel;
    [SerializeField] private ResourceInfo templateResourceInfo;
#pragma warning restore 0649
    #endregion
    private StructureType previousDisplayedType = StructureType.None;
    private List<ResourceInfo> resourcesInfo;


    // Update is called once per frame
    void Update()
    {
        StructureType currentType = UIManager.Instance.HoveredStructure;
        childObject.SetActive(currentType != StructureType.None);
        if(currentType != StructureType.None)
        {
            Vector3 pos = MouseManager.Instance.GetMouseScreenPos();
            childObject.transform.position = pos;
            if(currentType != previousDisplayedType)
            {
                previousDisplayedType = currentType;
                 StructureBinding binding = TilesDataManager.Instance.GetStructureBindingFromType(currentType);
                _buildingNameText.text = binding.data.structureName;
                List<Cost> creationCost = binding.data.GetCreationCost();
                CreateResourcesList(creationCost);
            }
        }
    }

    public void CreateResourcesList(List<Cost> resources)
    {
        if(resourcesInfo != null && resourcesInfo.Count > 0)
        {
            foreach(ResourceInfo resourceInfo in resourcesInfo)
            {
                Destroy(resourceInfo.gameObject);
            }
            resourcesInfo.Clear();
        }
        resourcesInfo = new List<ResourceInfo>();
        foreach (Cost resource in resources)
        {
            ResourceInfo resourceCreated = Instantiate(templateResourceInfo, Vector3.zero, Quaternion.identity, panel.transform);
            resourceCreated.Initialize(resource);
            resourcesInfo.Add(resourceCreated);
        }
    }

}
