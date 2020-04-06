using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingSelector : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private BuildingSelectorCell _cellTemplate;
#pragma warning restore 0649

    public StructureType SelectedStructure { get; private set; }
    private IEnumerable<StructureBinding> _structures;
    private BuildingSelectorCell _selectedCell;

    // Start is called before the first frame update
    void Start()
    {
        _structures = TilesDataManager.Instance.GetAllConstructiblesStructures();
        InstantiateCells();
        BuildingManager.OnBuildDone += OnBuildDone;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void InstantiateCells()
    {
        foreach (StructureBinding building in _structures)
        {
            BuildingSelectorCell cell = Instantiate(_cellTemplate, Vector3.zero, Quaternion.identity, transform);
            cell.InitWithBuilding(building);
            cell.RegisterButtonOnClick(OnCellClicked);
        }
        Destroy(_cellTemplate.gameObject);
    }

    void OnCellClicked(BuildingSelectorCell cellClicked)
    {
        _selectedCell = cellClicked;
        BuildingManager.Instance.StructureToBuildSelected(_selectedCell.building.type);
    }

    private void OnBuildDone()
    {
        _selectedCell = null;
    }

    private void OnDestroy()
    {
        BuildingManager.OnBuildDone -= OnBuildDone;
    }
}
