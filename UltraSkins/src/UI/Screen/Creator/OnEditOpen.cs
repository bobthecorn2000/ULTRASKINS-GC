using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltraSkins.Configuration;
using System;

namespace UltraSkins.UI.Creator
{
    public class OnEditOpen : MonoBehaviour
    {
        // Start is called before the first frame update
        void OnEnable()
        {
            SettingsManager.Instance.ShowPreviewWireFrame(true);
            SettingsManager.Instance.ShowSettingsAssets(true);
        }
        void OnDisable()
        {
            SettingsManager.Instance.ShowPreviewWireFrame(false);
            SettingsManager.Instance.ShowSettingsAssets(false);
        }
    }
}
