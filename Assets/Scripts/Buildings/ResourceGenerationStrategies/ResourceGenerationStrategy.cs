using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceGenerationStrategy 
{
    public abstract int GenerateResource(IEnumerable<ResourceTile> tiles, int resourceAmount);
}
