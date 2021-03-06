﻿using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Dependecies to other managers:
// None

public enum ResourceType { None, Wood, Stone, Eponium, Energy }

public class ResourcesManager : Manager
{
    public static ResourcesManager Instance { get; private set; }

    public int EnergyTotal { get; private set; }
    public int EnergyAvailable { get; private set; }

    private List<StructureTile> _energyProducingStructures;
    private List<StructureTile> _energyConsumingStructures;
    private List<Cost> _currentResources;

    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private List<ResourceData> _resourceDatas;
#pragma warning restore 0649
    #endregion

    #region Events
    public delegate void ResourcesModification();
    public static event ResourcesModification OnResourcesModification;
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitState = InitializationState.Initializing;
            _energyProducingStructures = new List<StructureTile>();
            _energyConsumingStructures = new List<StructureTile>();
            LevelManager.OnLevelNeedReset += InitializeResourcesForCurrentLevel;
            InitResourcesTypes();
            InitState = InitializationState.Ready;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // LevelManager is already loaded when this manager is created
        InitializeResourcesForCurrentLevel();
    }

    private void OnDestroy()
    {
        LevelManager.OnLevelNeedReset -= InitializeResourcesForCurrentLevel;
    }

    private void InitResourcesTypes()
    {
        _currentResources = new List<Cost>();
        foreach (ResourceData resourceData in _resourceDatas)
        {
            _currentResources.Add(new Cost(0, resourceData.resourceType));
        }
    }

    public List<Cost> GetCurrentResource()
    {
        return _currentResources;

    }

    private void Update()
    {
        UpdateEnergyValues();
        if (InputManager.Instance.GetKeyDown("cheatResources"))
        {
            CHEATGiveResources();
        }
    }

    public void CHEATGiveResources()
    {
        foreach (Cost cost in _currentResources)
        {
            cost.amount += 1000;
        }
        OnResourcesModification?.Invoke();
    }

    public bool IsMajorResource(ResourceType type)
    {
        return GetResourceDataForType(type).isMajorResource;
    }

    public ResourceData GetResourceDataForType(ResourceType type)
    {
        return _resourceDatas.Find(x => x.resourceType == type);
    }

    public Cost GetResourceForType(ResourceType type)
    {
        return _currentResources.Find(x => x.type == type);
    }

    public void AddResource(Cost resource)
    {
        Cost modifiedResource = GetResourceForType(resource.type);
        if (modifiedResource != null)
        {
            modifiedResource.amount += resource.amount;
        }
        else
        {
            Debug.LogWarning("Resource is not properly initialized");
        }
        OnResourcesModification?.Invoke();
    }

    public void RemoveResource(Cost resource)
    {
        Cost modifiedResource = GetResourceForType(resource.type);
        if (modifiedResource != null)
        {

            modifiedResource.amount -= resource.amount;
            if (modifiedResource.amount < 0)
            {
                Debug.LogWarning("Resource amount is below zero, something went wrong");
                modifiedResource.amount = 0;

            }
        }
        OnResourcesModification?.Invoke();
    }

    public bool CanPay(List<Cost> costs)
    {
        bool canPay = false;
        if (costs != null)
        {
            canPay = costs.All(CanPay);
        }
        return canPay;
    }

    public bool CanPay(Cost cost)
    {
        Cost modifiedResource = GetResourceForType(cost.type);
        return modifiedResource.amount >= cost.amount;
    }

    public void Pay(List<Cost> costs)
    {
        costs.ForEach(Pay);
    }

    public void Pay(Cost cost)
    {
        RemoveResource(cost);
    }

    public void Repay(List<Cost> costs)
    {
        costs.ForEach(Repay);
    }

    public void Repay(Cost cost)
    {
        AddResource(cost);
    }

    private void InitializeResourcesForCurrentLevel()
    {
        InitializeResources(LevelManager.Instance.GetCurrentLevel().GetInitialResources());
    }

    private void InitializeResources(List<Cost> newResources)
    {
        InitState = InitializationState.Initializing;
        foreach (Cost currentResource in _currentResources)
        {
            int newAmout = 0;
            Cost resourceToAdd = newResources.Find(x => x.type == currentResource.type);
            if (resourceToAdd != null)
            {
                newAmout = resourceToAdd.amount;
            }
            currentResource.amount = newAmout;

        }
        OnResourcesModification?.Invoke();
        InitState = InitializationState.Ready;
    }

    #region Energy Management
    public void RegisterStructure(StructureTile structure)
    {
        if (structure._structureData.ProducesEnergy)
        {
            RegisterProducingEnergyStructure(structure);
        }

        if (structure._structureData.ConsumesEnergy)
        {
            RegisterConsumingEnergyStructure(structure);
        }
    }

    public void UnregisterStructure(StructureTile structure)
    {
        if (structure._structureData.ProducesEnergy)
        {
            UnregisterProducingEnergyStructure(structure);
        }

        if (structure._structureData.ConsumesEnergy)
        {
            UnregisterConsumingEnergyStructure(structure);
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
                                                .Sum(x => x._structureData.GetCurrentEnergyAmountForLevel(x.GetStructureLevel()));
        EnergyAvailable = EnergyTotal - _energyConsumingStructures.Where(x => x.IsOn)
                                                                  .Sum(x => x._structureData.GetCurrentEnergyAmountForLevel(x.GetStructureLevel()));
    }

    public void RecomputeActiveStructure()
    {
        UpdateEnergyValues();
        bool mustDeactivateStructure = false;
        int energyConsumed = 0;
        foreach (StructureTile structure in _energyConsumingStructures)
        {
            if (!mustDeactivateStructure)
            {
                if (structure.IsOn)
                {
                    energyConsumed += structure._structureData.GetCurrentEnergyAmountForLevel(structure.GetStructureLevel());
                    if (energyConsumed > EnergyTotal) //Structure must be deactivated
                    {
                        structure.DeactivateStructureIfPossible();
                        mustDeactivateStructure = true;
                    }
                }
            }
            else
            {
                structure.DeactivateStructureIfPossible();
            }
        }
    }
    #endregion
}
