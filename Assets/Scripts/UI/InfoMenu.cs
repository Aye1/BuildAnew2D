using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoMenu : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _posText;
    [SerializeField] private TextMeshProUGUI _typeText;
    [SerializeField] private TextMeshProUGUI _structureText;
    [SerializeField] private Button _toggleButton;
#pragma warning restore 0649
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        bool isActive = selectedTile != null;
        SetVisible(isActive);
        if(isActive)
        {
            _posText.text = selectedTile.gridPosition.ToString();
            _typeText.text = selectedTile.GetTerrainText();
            _structureText.text = selectedTile.GetStructureText();
            _toggleButton.gameObject.SetActive(selectedTile.structureTile != null);
        }
    }

    public void ToggleStructure()
    {
        BaseTileData selectedTile = MouseManager.Instance.SelectedTile;
        selectedTile.ToggleStructureIfPossible();
    }

    private void SetVisible(bool visibility)
    { 
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
