using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourcesList : MonoBehaviour
{
    [SerializeField] private ResourceInfo templateResourceInfo;
    private List<ResourceInfo> resourcesInfo;

    public void CreateResourcesList(List<Cost> resources)
    {
        resourcesInfo = new List<ResourceInfo>();
        foreach (Cost resource in resources)
        {
            ResourceInfo resourceCreated = Instantiate(templateResourceInfo, Vector3.zero, Quaternion.identity, transform);
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
