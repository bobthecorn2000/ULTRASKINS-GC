using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UltraSkins;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UltraSkins.UI.Widgets;
using UltraSkins.UI.Layout;
using Newtonsoft.Json.Linq;
using UltraSkins.UI.Settings;
// for PropertyMaker

namespace UltraSkins.Configuration
{
    public class SettingsManager : MonoBehaviour
    {
        private readonly int userdataversion = 1;

        public static SettingsManager Instance { get; private set; }
        public SettingsButtonScripts SBS;
        public AboutMenu aboutMenu;

        public GameObject ESCobject;

        [Header("Prefab & UI Setup")]
        [Tooltip("Prefab with PropertyMaker component")]
        public GameObject propertyBlockPrefab;

        [Tooltip("Parent transform where setting blocks will be instantiated")]
        public Transform settingsParent;



        // Parsed settings loaded from JSON
        private List<SettingEntry> settingsData = new();
        public List<SavedSetting> SavedSettings = new();


        [Header("Menu and System")]

        public GameObject wireframe;

        public GameObject PreviewCam;
        public GameObject CameraModels;
        public GameObject Canvas;
        public TextAsset DefaultSettings;
        [SerializeField] private GameObject pagesContainer;
        [SerializeField] private List<GameObject> pages;
        [SerializeField] private List<Button> backButtons;

        public string SaveSettingPath;
        public string SaveSettingsLocation;
        private void Awake()
        {
            Shader.SetGlobalColor("_US_ECP", new Color(0, 0, 0, 1));
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Loads and parses JSON settings file, caching results.
        /// </summary>
        /// <param name="jsonPath">Full path to JSON file</param>
        public void LoadSettingsFromJson()
        {


            try
            {

                settingsData = JsonConvert.DeserializeObject<List<SettingEntry>>(DefaultSettings.text);
                ClearSettingsUI();
            }
            catch (JsonException ex)
            {
                Debug.LogError($"Error parsing settings USGC: {ex.Message}");
            }
        }
        public void loadUserSettings(string savesettingslocation)
        {
            SaveSettingsLocation = savesettingslocation;
            string path = Path.Combine(SaveSettingsLocation, "Settings.USGC");
            if (!File.Exists(path))
            {
                Debug.LogError($"Saved settings file not found: {path}");
                SavedSettings.Clear();
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                SaveSettingPath = path;
                UserData userData = new UserData();
                var token = JToken.Parse(json);
                if (token.Type == JTokenType.Array)
                {
                    userData = V0LegacyUpdater(json);
                }
                else if (token.Type == JTokenType.Object)
                {
                    var versionToken = token["version"];
                    int version = versionToken.Value<int>();

                    if (version == userdataversion)
                    {
                        
                        userData = JsonConvert.DeserializeObject<UserData>(json);
                    }
                    else
                    {
                        
                        Debug.LogError($"Made for a different version of Ultraskins");
                    }
                }


            }
            catch (JsonException ex)
            {
                Debug.LogError($"Failed to parse saved settings: {ex.Message}");

            }
        }
        private UserData V0LegacyUpdater(string json)
        {
            UserData upgraded = new UserData();
            upgraded.Version = userdataversion;
            List<v0SS> v0data = JsonConvert.DeserializeObject<List<v0SS>>(json);
            foreach (v0SS data in v0data)
            {
                SavedSetting savedSetting = new SavedSetting();
                savedSetting.ID = data.ID;
                if (data.SavedBool != null)
                {
                    savedSetting.SavedValue = data.SavedBool;
                }
                else if (data.SavedInt != null)
                {
                    savedSetting.SavedValue = data.SavedInt;
                }
                else if (data.SavedOption != null)
                {
                    savedSetting.SavedValue = data.SavedOption;
                }
                else
                {
                    savedSetting.SavedValue = null;
                }

                upgraded.savedSettings.Add(savedSetting);

            }
            return upgraded;
        }




        /// <summary>
        /// Clears current UI and builds settings menu based on loaded data.
        /// </summary>
        public void BuildSettingsUI()
        {
            if (settingsParent == null)
            {
                Debug.LogError("SettingsManager: settingsParent not assigned.");
                return;
            }

            ClearSettingsUI();

            foreach (var setting in settingsData)
            {
                var go = Instantiate(propertyBlockPrefab, settingsParent);
                var propMaker = go.GetComponent<PropertyMaker>();
                int comptype = MapTypeToComponentIndex(setting.Type);
                if (propMaker == null)
                {
                    Debug.LogWarning("PropertyMaker component missing on prefab.");
                    continue;
                }

                if (comptype == -1)
                {
                    propMaker.currentMode = PropertyMaker.Mode.Title;
                    propMaker.titleText = setting.Label;
                }
                else
                {
                    propMaker.currentMode = PropertyMaker.Mode.Settings;
                    propMaker.settingsLabelText = setting.Label;
                    propMaker.activeComponentIndex = comptype;
                    propMaker.settingID = setting.ID;
                    //propMaker.enableReload = setting.EnableReload;
                    if (setting.Disclaimer != null)
                    {
                        propMaker.enableDisclaimer = true;
                        propMaker.DisclaimerText = setting.Disclaimer;
                    }
                    if (setting.Options != null)
                    {
                        propMaker.selectableComponents[2].GetComponent<TMP_Dropdown>().AddOptions(setting.Options.Select(option => new TMP_Dropdown.OptionData(option)).ToList());
                    }
                    if (string.IsNullOrEmpty(SaveSettingPath))
                    {
                        propMaker.selectableComponents[0].GetComponent<Toggle>().interactable = false;
                        propMaker.selectableComponents[2].GetComponent<TMP_Dropdown>().interactable = false;
                        propMaker.selectableComponents[3].GetComponent<TMP_InputField>().interactable = false;
                    }
                }

                propMaker.ApplyMode();
                if (comptype == 0)
                {
                    Toggle toggle = propMaker.selectableComponents[0].GetComponent<Toggle>();
                    bool userval = GetSettingValue<bool>(setting.ID);
                    toggle.isOn = userval;
                    toggle.onValueChanged.AddListener(delegate
                    {
                        SetSavedSetting(setting.ID, toggle.isOn);
                    });

                }
                if (comptype == 1)
                {
                    Button button = propMaker.selectableComponents[1].GetComponent<Button>();
                    TextMeshProUGUI btntext = button.GetComponentInChildren<TextMeshProUGUI>();
                    switch (setting.ID)
                    {
                        case "BTN_GlobalSkinFolder":
                            button.onClick.AddListener(delegate { SBS.OpenUltraskinsGlobalFolder(); });
                            btntext.text = "Launch";
                            break;
                        case "BTN_AboutMenu":
                            GotoSettingsPage settingspage = button.gameObject.AddComponent<GotoSettingsPage>();
                            settingspage.targetPageIndex = 2;
                            btntext.text = "View";
                            break;
                    }
                }
                if (comptype == 2)
                {
                    TMP_Dropdown dropdown = propMaker.selectableComponents[2].GetComponent<TMP_Dropdown>();
                    int index = dropdown.options.FindIndex(option => option.text == GetSettingValue<string>(setting.ID));
                    if (index >= 0)
                    {
                        dropdown.value = index;
                        dropdown.RefreshShownValue();
                    }
                    dropdown.onValueChanged.AddListener(delegate
                    {
                        SetSavedSetting(setting.ID, dropdown.options[dropdown.value].text);
                    });
                }

                if (comptype == 3)
                {
                    TMP_InputField inputfield = propMaker.selectableComponents[3].GetComponent<TMP_InputField>();
                    int userval = GetSettingValue<int>(setting.ID);
                    inputfield.text = userval.ToString();
                    inputfield.onEndEdit.AddListener(delegate
                    {
                        SetSavedSetting(setting.ID, int.Parse(inputfield.text));
                    });
                }
                // TODO: Initialize the actual UI control value here if needed
            }
        }

        /// <summary>
        /// Remove all instantiated setting UI blocks.
        /// </summary>
        private void ClearSettingsUI()
        {
            if (settingsParent == null) return;

            for (int i = settingsParent.childCount - 1; i >= 0; i--)
                Destroy(settingsParent.GetChild(i).gameObject);
        }

        /// <summary>
        /// Maps your JSON "type" string to PropertyMaker selectableComponents index.
        /// </summary>
        private int MapTypeToComponentIndex(string type)
        {
            return type?.ToLower() switch
            {
                "category" => -1,
                "toggle" => 0,
                "button" => 1,
                "dropdown" => 2,
                "int" => 3,
                _ => 0,  // Default fallback
            };
        }
        public T GetSettingValue<T>(string id)
        {
            // Check saved settings
            var saved = SavedSettings.FirstOrDefault(s => s.ID == id);
            if (saved != null)
            {
                if (saved.SavedValue != null)
                {
                    try
                    {
                        if (typeof(T) == typeof(int))
                            return (T)(object)Convert.ToInt32(saved.SavedValue);

                        if (typeof(T) == typeof(long))
                            return (T)(object)Convert.ToInt64(saved.SavedValue);
                        return (T)saved.SavedValue;
                    }
                    catch (InvalidCastException ice)
                    {
                        Debug.LogError($"Type mismatch for '{id}'. Stored: {saved.SavedValue.GetType().Name}, Requested: {typeof(T).Name} ");
                        Debug.LogError(ice.ToString());
                    }
                }

            }

            // Fallback to default settings
            var defaultSetting = settingsData.FirstOrDefault(s => s.ID == id);
            if (defaultSetting != null)
            {
                try
                {
                    if (typeof(T) == typeof(int))
                        return (T)(object)Convert.ToInt32(defaultSetting.Default);

                    if (typeof(T) == typeof(long))
                        return (T)(object)Convert.ToInt64(defaultSetting.Default);
                    return (T)defaultSetting.Default;
                }
                catch (InvalidCastException ice)
                {
                    Debug.LogError($"Type mismatch for '{id}'. Stored: {defaultSetting.Default.GetType().Name}, Requested: {typeof(T).Name} ");
                    Debug.LogError(ice.ToString());
                }
            }

            // Not found
            return default;
        }


        public void SetSavedSetting(string id, object value)
        {
            var existing = SavedSettings.FirstOrDefault(s => s.ID == id);
            if (existing == null)
            {
                existing = new SavedSetting { ID = id };
                SavedSettings.Add(existing);
            }

            existing.SavedValue = value;

            SaveSettings(SaveSettingPath);
        }


        private int currentPageIndex = -1;
        public void ShowSettingsAssets(bool toggle)
        {
            if (toggle)
            {
                Canvas.SetActive(true);
            }
            else
            {
                Canvas.SetActive(false);
            }
        }

        [Obsolete]
        public void ShowPreviewWireFrame(bool toggle)
        {
            if (toggle)
            {
                PreviewCam.SetActive(true);
                wireframe.SetActive(true);
                CameraModels.SetActive(true);
            }
            else
            {
                PreviewCam.SetActive(false);
                wireframe.SetActive(false);
                CameraModels.SetActive(false);
            }
        }
        public void ShowPage(int index)
        {
            if (index < 0 || index >= pages.Count)
            {
                Debug.LogWarning("Page index out of range");
                return;
            }

            if (pagesContainer != null && !pagesContainer.activeSelf)
                pagesContainer.SetActive(true);

            foreach (var page in pages)
                page.SetActive(false);

            pages[index].SetActive(true);
            pages[index].GetComponent<DynStatMatrix>().ShowDynamicAssets();

            currentPageIndex = index;
        }
        public void ShowPagePause(int index)
        {
            if (index < 0 || index >= pages.Count)
            {
                Debug.LogWarning("Page index out of range");
                return;
            }

            if (pagesContainer != null && !pagesContainer.activeSelf)
                pagesContainer.SetActive(true);

            foreach (var page in pages)
                page.SetActive(false);

            pages[index].SetActive(true);
            pages[index].GetComponent<DynStatMatrix>().ShowStaticAssets();

            currentPageIndex = index;
        }
        public void HidePage(int index)
        {
            if (index < 0 || index >= pages.Count) return;

            pages[index].SetActive(false);

            if (pagesContainer != null && !AnyPageActive())
                pagesContainer.SetActive(false);

            if (currentPageIndex == index)
                currentPageIndex = -1;
        }

        public Button GetBackButton(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex >= backButtons.Count)
                return null;
            return backButtons[pageIndex];
        }

        public bool AnyPageActive()
        {
            foreach (var page in pages)
                if (page.activeSelf)
                    return true;
            return false;
        }


        public void ShowPagesContainer()
        {
            if (pagesContainer != null)
                pagesContainer.SetActive(true);
        }

        public void HidePagesContainer()
        {
            if (pagesContainer != null)
                pagesContainer.SetActive(false);
        }

        public int GetCurrentPageIndex() => currentPageIndex;





        public void SaveSettings(string filePath)
        {
            try
            {
                string json = JsonConvert.SerializeObject(SavedSettings, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving settings: {ex.Message}");
            }
        }


        /// <summary>
        /// Model for deserializing JSON settings entries.
        /// </summary>

        private class SettingEntry
        {
            [JsonProperty("ID")]
            public string ID { get; set; }

            [JsonProperty("Label")]
            public string Label { get; set; }

            [JsonProperty("Type")]
            public string Type { get; set; }

            [JsonProperty("Disclaimer")]
            public string Disclaimer { get; set; }

            [JsonProperty("Default", Required = Required.Default)]
            public object Default { get; set; }

            [JsonProperty("Options", Required = Required.Default)]
            public List<string> Options { get; set; }





        }
        public class SavedSetting
        {
            [JsonProperty("ID")]
            public string ID { get; set; }

            // One of these two will be populated depending on the setting type
            [JsonProperty("SavedValue", Required = Required.Default)]
            public object SavedValue { get; set; }



        }
        public class UserData
        {
            [JsonProperty("Version")]
            public int Version;
            [JsonProperty("Settings")]
            public List<SavedSetting> savedSettings;
        }





        public class v0SS
        {
            [JsonProperty("id")]
            public string ID { get; set; }

            // One of these two will be populated depending on the setting type
            [JsonProperty("savedBool", Required = Required.Default)]
            public bool? SavedBool { get; set; }
            [JsonProperty("savedInt", Required = Required.Default)]
            public int? SavedInt { get; set; }

            [JsonProperty("savedOption", Required = Required.Default)]
            public string SavedOption { get; set; }
        }
    }
}
