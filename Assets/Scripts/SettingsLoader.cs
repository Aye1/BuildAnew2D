using UnityEngine;
using FMODUnity;

public static class SettingsLoader
{
    public const string soundOnKey = "SOUNDON";

    public static bool isSoundOn;

    public static void LoadInitialSettings()
    {
        LoadSoundSettings();
    }

    static void LoadSoundSettings()
    {
        isSoundOn = !(PlayerPrefs.GetInt(soundOnKey, 1) == 0);
        SetSoundOn(isSoundOn);
    }

    // TODO: move this in another dedicated class?
    // Or rename this class in SettingsManager for example
    public static void SetSoundOn(bool value)
    {
        isSoundOn = value;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.audioMasterMute = !value;
#else
        RuntimeManager.MuteAllEvents(!value);
#endif
        PlayerPrefs.SetInt(soundOnKey, value ? 1 : 0);
    }
}
