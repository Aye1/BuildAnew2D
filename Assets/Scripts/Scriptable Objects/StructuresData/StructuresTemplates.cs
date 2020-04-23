using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;



[System.Serializable]
public class StructureBinding
{
    public StructureType type;
    public BuildingView building;
    public StructureData data;
    [SerializeField] public TileBase buildingTile;
}

public class StructuresTemplates : MonoBehaviour
{

    public List<StructureBinding> _structureTemplates;
    public StructureType GetStructureTypeFromTile(TileBase tile)
    {
        StructureType returnType = StructureType.None;
        StructureBinding binding = GetStructureBindingFromTile(tile);
        if (binding != null)
        {
            returnType = binding.type;
        }
        return returnType;
    }

    public StructureBinding GetStructureBindingFromTile(TileBase tile)
    {
        foreach (StructureBinding structureBinding in _structureTemplates)
        {
            if (structureBinding.buildingTile.name.Equals(tile.name))
            {
                return structureBinding;
            }
        }
        return null;
    }

    public StructureBinding GetStructureBindingFromType(StructureType type)
    {
        foreach (StructureBinding structureBinding in _structureTemplates)
        {
            if (structureBinding.type == type)
            {
                return structureBinding;
            }
        }
        return null;
    }

    public Sprite GetSpriteForStructure(StructureType type)
    {
        StructureBinding structureBinding = GetStructureBindingFromType(type);
        if (structureBinding != null && structureBinding.building != null)
        {
            return structureBinding.building.GetComponent<SpriteRenderer>().sprite;
        }
        return null;
    }

    public StructureData GetDataForStructure(StructureType type)
    {
        StructureBinding element = _structureTemplates.First(x => x.type == type);
        return element?.data;
    }

    public IEnumerable<StructureBinding> GetAllConstructiblesStructures()
    {
        return _structureTemplates.Where(x => x.data.upgradeData != null);
    }

    public List<Cost> CostForStructure(StructureType type)
    {
        return _structureTemplates.First(x => x.type == type).data.GetCreationCost();
    }

}
