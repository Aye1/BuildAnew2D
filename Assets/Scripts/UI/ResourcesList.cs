using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourcesList : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private ResourceInfo templateResourceInfo;
    [SerializeField] private Transform majorResourcesHolder;
    [SerializeField] private Transform resourcesHolder;
#pragma warning restore 0649
#endregion
    private List<ResourceInfo> resourcesInfo;

    public void CreateResourcesList(List<Cost> resources)
    {
        resourcesInfo = new List<ResourceInfo>();
        foreach (Cost resource in resources)
        {
            Transform positionList = resourcesHolder;
            if(ResourcesManager.Instance.IsMajorResource(resource.type))
            {
                positionList = majorResourcesHolder;
            }
            ResourceInfo resourceCreated = Instantiate(templateResourceInfo, Vector3.zero, Quaternion.identity, positionList);
            resourceCreated.Initialize(resource);
            resourcesInfo.Add(resourceCreated);
        }
    }

    public void RefreshResourcesList(List<Cost> resources)
    {
        foreach(Cost cost in resources)
        {
            ResourceInfo info = resourcesInfo.First(x => x.cost.type == cost.type);
            info.SetAmount(cost.amount);
        }
    }
}
