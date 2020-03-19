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

    private List<StructureTile> _energyProducingStructures;
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
        _energyProducingStructures = new List<StructureTile>();
        _energyConsumingStructures = new List<StructureTile>();
        WoodAmount = 10000;
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

    public bool CanPay(List<Cost> costs)
    {
        return costs.All(CanPay);
    }

    public bool CanPay(Cost cost)
    {
        int amountToCompare = int.MinValue;
        switch(cost.type)
        {
            case ResourceType.Wood:
                amountToCompare = WoodAmount;
                break;
        }
        return amountToCompare >= cost.amount;
    }

    public void Pay(List<Cost> costs)
    {
        costs.ForEach(Pay);
    }

    public void Pay(Cost cost)
    {
        switch(cost.type)
        {
            case ResourceType.Wood:
                RemoveWood(cost.amount);
                break;
        }
    }

    public void Repay(List<Cost> costs)
    {
        costs.ForEach(Repay);
    }

    public void Repay(Cost cost)
    {
        switch(cost.type)
        {
            case ResourceType.Wood:
                AddWood(cost.amount);
                break;
        }
    }

    public void RegisterStructure(StructureTile structure)
    {
        if (structure.structureData.ProducesEnergy)
        {
            RegisterProducingEnergyStructure(structure);
        }

        if (structure.structureData.ConsumesEnergy)
        {
            RegisterConsumingEnergyStructure(structure);
        }
    }

    public void UnregisterStructure(StructureTile structure)
    {
        if (structure.structureData.ProducesEnergy)
        {
            UnregisterProducingEnergyStructure(structure);
        }

        if (structure.structureData.ConsumesEnergy)
        {
            UnregisterProducingEnergyStructure(structure);
        }
    }

    private void RegisterProducingEnergyStructure(StructureTile structure)
    { 
        // Add() allows duplicates, so we just check the structure isn't already in the list
        if (!_energyProducingStructures.Contains(structure))
        {
            _energyProducingStructures.Add(structure);
        }
    }

    private void UnregisterProducingEnergyStructure(StructureTile structure)
    {
        _energyProducingStructures.Remove(structure);
    }


    private void RegisterConsumingEnergyStructure(StructureTile structure)
    {
        // Add() allows duplicates, so we just check the structure isn't already in the list
        if (!_energyConsumingStructures.Contains(structure))
        {
            _energyConsumingStructures.Add(structure);
        }
    }

    private void UnregisterConsumingEnergyStructure(StructureTile structure)
    {
        _energyConsumingStructures.Remove(structure);
    }

    private void UpdateEnergyValues()
    {
        // Basic energy count at the moment
        EnergyTotal = _energyProducingStructures.Where(x => x.IsOn)
                                                .Sum(x => x.structureData.producedEnergyAmount);
        EnergyAvailable = EnergyTotal - _energyConsumingStructures.Where(x => x.IsOn)
                                                                  .Sum(x => x.structureData.consumedEnergyAmount);
    }
}
