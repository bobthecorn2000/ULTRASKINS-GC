using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.IO;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UltraSkins;
using BatonPassLogger;
using UltraSkins.Utils;
using Newtonsoft.Json;

using static UltraSkins.ULTRASKINHand;
using System.Text;



namespace UltraSkins.UI
{
    internal class MenuManager : MonoBehaviour
    {
        
        ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
        internal static MenuManager  MMinstance { get; private set; }
        List<GameObject> loadedButtons = new List<GameObject>();
        Dictionary<string, Button> AvailbleSkins = new Dictionary<string, Button>();
        public SkinDetails SD = null;

        void Awake()
        {
            MMinstance = this;

        }



        public void Closeskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject fallnoiseoff, Animator animator,ObjectActivateInSequence oais,GameObject content)
        {
            MMinstance.orderifier(oais, content);
            fallnoiseoff.SetActive(true);
            handInstance.settingsmanager.ShowPreviewWireFrame(false);
            handInstance.settingsmanager.ShowSettingsAssets(false);
            animator.Play("menuclose");

            // Start the coroutine
            MMinstance.StartCoroutine(MMinstance.DisableAfterCloseAnimation(mainmenucanvas, Configmenu, animator));

        }
        
        private IEnumerator DisableAfterCloseAnimation(GameObject mainmenucanvas, GameObject Configmenu, Animator animator)
        {
            // Wait until the animation finishes
            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animationLength);

            // Disable the GameObject (for example, the main menu canvas)
            Configmenu.SetActive(false);
            mainmenucanvas.SetActive(true);
        }

        public void GenerateButtons(GameObject contentfolder, ObjectActivateInSequence activateanimator)
        {

            Dictionary<string, string> Locations = SkinEventHandler.GetCurrentLocations();
            metadataReader MDR = new metadataReader();
            AvailbleSkins.Clear();
            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/ultraskinsButton.prefab").Completed += buttonHandle =>
            {
                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    foreach (string Location in Locations.Keys)
                    {

                        string skinfolderdir = Locations[Location];
                        string[] subfolders = Directory.GetDirectories(skinfolderdir);
                        int buttonsToLoad = subfolders.Length;
                        int buttonsLoaded = 0;

                        GameObject prefab = buttonHandle.Result;

                        foreach (string subfolder in subfolders)
                        {

                            string folder = Path.GetFileName(subfolder);
                            string metadataPath = Path.Combine(subfolder, "metadata.GCMD");
                            string PackPath = Path.Combine(subfolder, "pack.GCMD");

                            if (!(File.Exists(metadataPath) || File.Exists(PackPath)) && Location != "Global" && Location != "Version")
                            {
                                // Skip folders that aren't Ultraskins-compatible
                                BatonPass.Debug("Skipping: " + folder + " at " + metadataPath);
                                continue;
                            }

                            if (File.Exists(PackPath))
                            {
                                GCPACK PD = MDR.ReadPack(PackPath);
                                try
                                {
                                    foreach (string path in PD.SubDirectories)
                                    {
                                        string subsubfolder = Path.Combine(subfolder, path);
                                    
                                            if (File.Exists(subsubfolder))
                                            {
                                                BuildButton(Location, contentfolder, MDR, prefab, subsubfolder);
                                            }
                                            else
                                            {
                                                BatonPass.Warn("\"" + subsubfolder + "\" does not exist in the Pack. This is most likely an incorrect folder name in the Pack.GCMD file located in \"" + PackPath + "\". Code -\"MMAN-GENERATEBUTTONS-MAINMENU-PACKPATH_MISSING_LOCATION\"");
                                            }
                                    }



                                }
                                catch (Exception ex)
                                {

                                    BatonPass.Error($"Could not reach subdirectories due to a possible failed deserialization,this error should be accompanied by \"MMAN-MDR-READPACK-PACK_READ_WARNING\". Pack {folder} will be skipped. CODE -\"MMAN-MAINMENU-PACK_READ_FAILURE\"");
                                    BatonPass.Error(ex.Message);
                                }
                            }
                            else
                            {
                               BuildButton(Location, contentfolder, MDR, prefab, subfolder);
                            }

                                buttonsLoaded++;
                            }
                    }
                    orderfixer(contentfolder);
                    orderifier(activateanimator, contentfolder);


                }
                else
                {
                    BatonPass.Error("Failed to load ultraskinsButton: " + buttonHandle.OperationException.Message);
                    BatonPass.Error("Ultraskins will still work, but skin changes via the menu will be disabled. CODE -\"MMAN-GENERATEBUTTONS-MAINMENU-ASSET_BUNDLE_FAILED\"");
                }

                // Check if all buttons are loaded

            };
        }

        public void BuildButton(string Location, GameObject contentfolder, metadataReader MDR, GameObject prefab, string subfolder)
        {

            string folder = Path.GetFileName(subfolder);
            string metadataPath = Path.Combine(subfolder, "metadata.GCMD");
            string PackPath = Path.Combine(subfolder, "pack.GCMD");
            GameObject instance = Instantiate(prefab, contentfolder.transform);
            instance.SetActive(true);

            Button ultraskinsbutton = instance.GetComponentInChildren<Button>();

            ButtonEnableManager BEM = instance.GetComponent<ButtonEnableManager>();

            BEM.skinDetails = SD;

            if (Location == "r2modman" || Location == "Thunderstore")
            {
                BEM.isThunderstore = true;
            }

            if (File.Exists(metadataPath))
            {
                GCMD MD = MDR.ReadMD(metadataPath);
                if (MD != null)
                {
                    BEM.SkinDescription = MD.Description;
                    BEM.SkinName = MD.SkinName;
                    ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = MD.SkinName;
                    if (MD.SupportedPlugins.Count >= 1)
                    {
                        BEM.isplugin = true;
                    }
                    BEM.VerNum = MD.Version;
                    
                }
                else
                {
                    ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                    BEM.SkinName = folder;
                    BEM.warning = true;
                }
            }
            else
            {
                ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                BEM.SkinName = folder;
                BEM.warning = true;
            }
            instance.name = folder;
            BEM.filePath = subfolder;
            AvailbleSkins.Add(folder, ultraskinsbutton);
            // Add button to list

            BatonPass.Debug("Successfully loaded and instantiated ultraskinsButton.");

            
        }


        void orderifier(ObjectActivateInSequence activateanimator,GameObject contentfolder)
        {
            loadedButtons.Clear();
            foreach (Transform child in contentfolder.transform)
            {
                loadedButtons.Add(child.gameObject);
            }
            GameObject parentgo = contentfolder.transform.parent.gameObject;
            GameObject specials = parentgo.transform.Find("Special").gameObject;
            loadedButtons.Add(specials);
            foreach (Transform child in specials.transform)
            {
                loadedButtons.Add(child.gameObject);
            }
            BatonPass.Debug("All buttons loaded, setting up activation sequence.");

            activateanimator.objectsToActivate = loadedButtons.ToArray();
            activateanimator.delay = 0.05f;
            BatonPass.Debug("Successfully set up ObjectActivateInSequence.");
        }

        void orderfixer(GameObject contentfolder)
        {
            BatonPass.Debug("attempting to order files");
            string[] filepathrev = handInstance.filepathArray.Reverse().ToArray();
            BatonPass.Debug("We know the following files");
            int debugtracker = 0;
            foreach (string item in filepathrev)
            {
                
                byte[] bytes = Encoding.UTF8.GetBytes(item);
                string hexString = BitConverter.ToString(bytes).Replace("-", " ");
                BatonPass.Debug($"pos:{debugtracker} path:{item} \n Logging Bytes: {hexString}");
                debugtracker++;
            }
            List<(Transform trans, int spot,ButtonEnableManager bem)> finalorder = new List<(Transform trans, int spot, ButtonEnableManager bem)>();
            foreach (Transform child in contentfolder.transform)
            {
                ButtonEnableManager BEM = child.GetComponent<ButtonEnableManager>();
                string specialpath = BEM.filePath;
                if (filepathrev.Contains(specialpath))
                {
                    int index = System.Array.IndexOf(filepathrev, specialpath);
                    BatonPass.Debug(child.name + " has path: " + specialpath + " and index of: " + index);
                    finalorder.Add((child, index,BEM));
                    
                   
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(specialpath);
                    string hexString = BitConverter.ToString(bytes).Replace("-", " ");
                    BatonPass.Debug($"no match for {specialpath} \n Logging Bytes {hexString}");
                }
            }
            foreach ((Transform trans, int spot,ButtonEnableManager bem) in finalorder.OrderBy(x => x.spot))
            {
                trans.SetSiblingIndex(spot);
                bem.IsEnabled = true;
                
            }
            

        }


        // Generate Buttons for pause menu usage
        public void GenerateButtons(GameObject contentfolder)
        {
            Dictionary<string,string> Locations = SkinEventHandler.GetCurrentLocations();
            metadataReader MDR = new metadataReader();

            AvailbleSkins.Clear();

            
            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/ultraskinsButton.prefab").Completed += buttonHandle =>
            {
                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                {

                    
                    foreach (var button in loadedButtons)
                    {
                        Destroy(button); // Destroy old button GameObjects
                    }

                    loadedButtons.Clear();

                    foreach (string Location in Locations.Keys)
                    {
                        string skinfolderdir = Locations[Location];
                        string[] subfolders = Directory.GetDirectories(skinfolderdir);
                        int buttonsToLoad = subfolders.Length;
                        int buttonsLoaded = 0;
                        GameObject prefab = buttonHandle.Result;
                        foreach (string subfolder in subfolders)
                        {

                            string folder = Path.GetFileName(subfolder);
                            string metadataPath = Path.Combine(subfolder, "metadata.GCMD");
                            string PackPath = Path.Combine(subfolder, "pack.GCMD");

                            if (!(File.Exists(metadataPath) || File.Exists(PackPath)) && Location != "Global" && Location != "Version")
                            {
                                // Skip folders that aren't Ultraskins-compatible
                                BatonPass.Debug("Skipping: " + folder + " at " + metadataPath);
                                continue;
                            }

                            if (File.Exists(PackPath))
                            {
                                GCPACK PD = MDR.ReadPack(PackPath);
                                try
                                {
                                    foreach (string path in PD.SubDirectories)
                                    {
                                        string subsubfolder = Path.Combine(subfolder, path);
                                        if (File.Exists(subsubfolder))
                                        {
                                            BuildButton(Location, contentfolder, MDR, prefab, subsubfolder);
                                        }
                                        else
                                        {
                                            BatonPass.Warn("\"" + subsubfolder + "\" does not exist in the Pack. This is most likely an incorrect folder name in the .GCPACK file located in \"" + PackPath + "\". Code -\"MMAN-GENERATEBUTTONS-PAUSEMENU-PACKPATH_MISSING_LOCATION\"");
                                        }

                                    }
                                } catch (Exception ex)
                                {

                                    BatonPass.Error($"Could not reach subdirectories due to a possible failed deserialization,this error should be accompanied by \"MMAN-MDR-READPACK-PACK_READ_WARNING\". Pack {folder} will be skipped. CODE -\"MMAN-PAUSEMENU-PACK_READ_FAILURE\"");
                                    BatonPass.Error(ex.Message);
                                }

                            }
                            else
                            {
                                BuildButton(Location, contentfolder, MDR, prefab, subfolder);
                            }

                            buttonsLoaded++;
                        }
                    }

                }
                else
                {
                    BatonPass.Error("Failed to load ultraskinsButton: " + buttonHandle.OperationException.Message);
                    BatonPass.Error("Ultraskins will still work, but skin changes via the menu will be disabled. CODE -\"MMAN-GENERATEBUTTONS-PAUSEMENU-ASSET_BUNDLE_FAILED\"");
                }

                // Check if all buttons are loaded

            };
        }

        public void applyskins(GameObject content)
        {
            
            List<string> validButtons = new List<string>();
            foreach (Transform childTransform in content.transform) { 
            BatonPass.Debug(childTransform.name);
            }
            BatonPass.Debug("starting apply process");
            foreach (Transform child in content.transform)
            {
                GameObject USbutton = child.gameObject;
                BatonPass.Debug("working on " + USbutton.name);
                ButtonEnableManager bem = USbutton.GetComponent<ButtonEnableManager>();
                if (bem.IsEnabled)
                {
                    validButtons.Add(bem.filePath);
                }
            }
            string[] paths = validButtons.ToArray();
            handInstance.refreshskins(paths);


        }


        public void DisableButtons()
        {
            foreach (var button in AvailbleSkins.Values)
            {
              button.interactable = false;
            }
        }
        public void EnableButtons()
        {
            foreach (var button in AvailbleSkins.Values)
            {
                button.interactable = true;
            }
        }
    }

    class GCMD
    {
        public string SkinName { get; set; }
        public string Description { get; set; }
        public string? IconOveride { get; set; }
        public string? Version { get; set; }
        public Dictionary<string, string>? SupportedPlugins { get; set; }
    }
    class GCPACK
    {
        public string PackName { get; set; }
        public string[] SubDirectories { get; set; }
    }
    class metadataReader
    {
        public GCMD ReadMD(string file)
        {
          
          string GCMDreader = File.ReadAllText(file);
            try
            {
                GCMD gcmd = JsonConvert.DeserializeObject<GCMD>(GCMDreader);
                return gcmd;
            }
            catch (JsonReaderException ex) {
                BatonPass.Warn($"The metadata.GCMD File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} \". Code -\"MMAN-MDR-READMD-METADATA_READ_WARNING\"");
                return null;
            }

        }
        public GCPACK ReadPack(string file)
        {

            string GCMDreader = File.ReadAllText(file);
            try
            {
                GCPACK gcPack = JsonConvert.DeserializeObject<GCPACK>(GCMDreader);
                return gcPack;
            }
            catch (JsonReaderException ex) {
                BatonPass.Warn($"The Pack.GCMD File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} \". Code -\"MMAN-MDR-READPACK-PACK_READ_WARNING\"");
                return null;


            }
;
        }
    }
}

