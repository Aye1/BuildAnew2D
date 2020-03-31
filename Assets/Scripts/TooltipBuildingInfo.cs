using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TooltipBuildingInfo : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _buildingNameText;
    [SerializeField] private GameObject childObject;
    [SerializeField] private LayoutGroup panel;
    [SerializeField] private ResourceInfo templateResourceInfo;
    [SerializeField] private ResourceInfo energyResourceInfo;
#pragma warning restore 0649
    #endregion
    private StructureType previousDisplayedType = StructureType.None;
    private List<ResourceInfo> resourcesInfo;


    // Update is called once per frame
    void Update()
    {
        StructureType currentType = UIManager.Instance.HoveredStructure;

        if(currentType != StructureType.None)
        {
            childObject.SetActive(true);
            Vector3 pos = MouseManager.Instance.GetMouseScreenPos();
            childObject.transform.position = pos;
            if (currentType != previousDisplayedType)
            {
                DisplayStructureInfo(currentType);
            }
        }
        else
        {
           // if(currentType != previousDisplayedType)
            {
                childObject.SetActive(false);
                //StartCoroutine("Fade");
            }
        }
        
        previousDisplayedType = currentType;
    }
    IEnumerator Fade()
    {
        for (float ft = 1f; ft >= 0; ft -= 0.25f)
        {
            Material material = childObject.gameObject.GetComponent<Renderer>().material;
            Color c = material.color;
            c.a =ft;
            material.color = c;
            yield return new WaitForSeconds(.1f);
        }
    }
    private void DisplayStructureInfo(StructureType structureType)
    {
        StructureBinding binding = TilesDataManager.Instance.GetStructureBindingFromType(structureType);
        _buildingNameText.text = binding.data.structureName;
        List<Cost> creationCost = binding.data.GetCreationCost();
        CreateResourcesList(creationCost);
        energyResourceInfo.SetAmount(binding.data.consumedEnergyAmount);
        energyResourceInfo.gameObject.SetActive(binding.data.ConsumesEnergy);
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
    public void SetVisible(bool visibility)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
