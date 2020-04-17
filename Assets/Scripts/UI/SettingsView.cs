using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsView : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Toggle _soundOnToggle;
#pragma warning restore 0649

    private void Start()
    {
        // TODO: move this elsewhere
        SettingsLoader.LoadInitialSettings();

        _soundOnToggle.onValueChanged.AddListener(OnSoundCheckboxChanged);
        _soundOnToggle.isOn = SettingsLoader.isSoundOn;
    }

    public void OnSoundCheckboxChanged(bool value)
    {
        SettingsLoader.SetSoundOn(value);
    }

    public void CloseView()
    {
        Destroy(gameObject);
    }
}
