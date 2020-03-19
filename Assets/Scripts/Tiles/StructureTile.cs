
public enum StructureType { None, PowerPlant, Sawmill, PumpingStation };
public enum ActivationState {ActivationPossible, ImpossibleNeedEnergy, ImpossibleMissEnergy, ImpossibleMissingStructure };
public class StructureTile : ActiveTile
{
    public StructureType structureType;
    public bool IsOn;
    public Building building;
    public bool consumesEnergy;
    public bool producesEnergy;

    public override string GetText()
    {
        return structureType == StructureType.None ? "" : structureType.ToString();
    }
    private ActivationState CanToggleStructure()
    {
        ActivationState canToggleStructure = ActivationState.ActivationPossible;

        int energyAvailable = ResourcesManager.Instance.EnergyAvailable;
        if (producesEnergy & IsOn)
        {
            canToggleStructure = energyAvailable > 0 ? ActivationState.ActivationPossible : ActivationState.ImpossibleNeedEnergy;
        }
        else if (consumesEnergy & !IsOn)
        {
            canToggleStructure = energyAvailable > 0 ? ActivationState.ActivationPossible : ActivationState.ImpossibleMissEnergy;

        }

        return canToggleStructure;
    }

    public ActivationState ToggleStructureIfPossible()
    {
        ActivationState activationState = CanToggleStructure();
        if (activationState == ActivationState.ActivationPossible)
        {
            IsOn = !IsOn;
        }
        return activationState;
    }
}
