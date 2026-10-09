using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UltraSkins.UI.Settings
{
    public class PropertyMaker : MonoBehaviour
    {
public enum Mode
    {
        Settings,
        Title
    }

    [Header("Mode Selection")]
    public Mode currentMode;

    [Header("Shared References")]
    public GameObject backdrop;

    [Header("Settings Mode")]
    public GameObject[] selectableComponents; // Only one can be active at a time
    public GameObject Reload;
    public GameObject Disclaimer;
    public TextMeshProUGUI settingName;

    [Header("Title Mode")]
    public TextMeshProUGUI CatLabel;

    [Header("Settings Mode Options")]
    public int activeComponentIndex = 0;
    public bool enableReload = false;
    public bool enableDisclaimer = false;
    public string settingsLabelText = "Default Settings Name";
    public string DisclaimerText = "Default";
    public bool enableBackdrop = true;

    [Header("Title Mode Options")]
    public string titleText = "-- Temp --\n<size=30>small temp</size>";
    
    [Header("Setting ID")]
    public string settingID;
private void Start()
{



    // Add more component types if needed
}

    private void OnValidate()
        {
            ApplyMode();
        }

    public void ApplyMode()
    {
        if (currentMode == Mode.Settings)
        {
            // Enable selected component, disable others
            for (int i = 0; i < selectableComponents.Length; i++)
            {
                if (selectableComponents[i] != null)
                    selectableComponents[i].SetActive((i == activeComponentIndex)); 
            }

            if (Reload != null)
                Reload.SetActive(enableReload);

                if (Disclaimer != null)
                {
                    Disclaimer.SetActive(enableDisclaimer);
                    Disclaimer.GetComponent<TextMeshProUGUI>().text = DisclaimerText;
                }

            if (settingName != null)
                        settingName.text = settingsLabelText;

            if (backdrop != null)
                backdrop.SetActive(enableBackdrop);

            if (CatLabel != null)
                CatLabel.gameObject.SetActive(false); // Hide title label in settings mode
        }
        else if (currentMode == Mode.Title)
        {
            // Disable all configurable settings components
            foreach (var comp in selectableComponents)
                if (comp != null)
                    comp.SetActive(false);
            
            if (Reload != null)
                Reload.SetActive(false);

            if (Disclaimer != null)
                Disclaimer.SetActive(false);

            if (backdrop != null)
                backdrop.SetActive(false);

            if (settingName != null)
                settingName.text = ""; // Hide label text in title mode

            if (CatLabel != null)
            {
                CatLabel.text = titleText;
                CatLabel.gameObject.SetActive(true);
            }
        }
    }
}
}
