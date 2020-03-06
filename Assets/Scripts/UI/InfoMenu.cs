using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _posText;
    [SerializeField] private TextMeshProUGUI _typeText;

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
        BaseTileData selectedTile = ClickManager.Instance.SelectedTile;
        bool isActive = selectedTile != null;
        SetVisible(isActive);
        if(isActive)
        {
            _posText.text = selectedTile.gridPosition.ToString();
            _typeText.text = selectedTile.type.ToString();
        }
    }

    private void SetVisible(bool visibility)
    { 
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
