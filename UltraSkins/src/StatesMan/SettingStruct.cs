using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraSkins.Configuration
{
[System.Serializable]
public class SettingEntry
{
    public string type; // "toggle", "slider", etc.
    public string key;
    public string label;
    public bool defaultBool;
    public float defaultFloat;
    public float min;
    public float max;
}

[System.Serializable]
public class SettingListWrapper
{
    public List<SettingEntry> entries;
}
}
