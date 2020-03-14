using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private TextMeshProUGUI _woodText;
    [SerializeField] private TextMeshProUGUI _energyText;
#pragma warning restore 0649
    #endregion

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _woodText.text = ResourcesManager.Instance.WoodAmount.ToString();
        _energyText.text = ResourcesManager.Instance.EnergyAvailable.ToString() + "/" + ResourcesManager.Instance.EnergyTotal.ToString();
    }

}
