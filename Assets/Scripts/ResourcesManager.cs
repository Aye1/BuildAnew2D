using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    public int WoodAmount { get; private set; }
    public int EnergyTotal { get; private set; }
    public int EnergyAvailable { get; private set; }

    private List<PowerPlantTile> _energyCreatingStructures;
    private List<StructureTile> _energyConsumingStructures;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        _energyCreatingStructures = new List<PowerPlantTile>();
        _energyConsumingStructures = new List<StructureTile>();
    }

    private void Update()
    {
        UpdateEnergyValues();
    }

    public void AddWood(int amount)
    {
        if (amount > 0)
        {
            WoodAmount += amount;
        }
    }

    public void RemoveWood(int amount)
    {
        if (WoodAmount - amount >= 0)
        {
            WoodAmount = Mathf.Max(WoodAmount - amount, 0);
        }
    }

    public void RegisterPowerPlant(PowerPlantTile powerPlant)
    {
        // Add() allows duplicates, so we just check the structure isn't already in the list
        if (!_energyCreatingStructures.Contains(powerPlant))
        {
            _energyCreatingStructures.Add(powerPlant);
        }
    }

    public void UnregisterPowerPlant(PowerPlantTile powerPlant)
    {
        _energyCreatingStructures.Remove(powerPlant);
    }

    public void RegisterConsumingEnergyStructure(StructureTile structure)
    {
        // Add() allows duplicates, so we just check the structure isn't already in the list
        if (!_energyConsumingStructures.Contains(structure))
        {
            _energyConsumingStructures.Add(structure);
        }
    }

    public void UnregisterConsumingEnergyStructure(StructureTile structure)
    {
        _energyConsumingStructures.Remove(structure);
    }

    private void UpdateEnergyValues()
    {
        // Basic energy count at the moment
        EnergyTotal = _energyCreatingStructures.Count(x => x.IsOn);
        EnergyAvailable = EnergyTotal - _energyConsumingStructures.Count(x => x.IsOn);
    }
}
