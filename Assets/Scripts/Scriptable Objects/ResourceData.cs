using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourceData", order = 1)]
public class ResourceData : ScriptableObject
{
    public ResourceType resourceType;
    public Sprite icon;
    public string name;

}