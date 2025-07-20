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
using System.Threading.Tasks;
using static UltraSkins.ULTRASKINHand;
using System.Text;
using BatonPassLogger.EX;
using static UltraSkins.UI.MenuManager;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.ComponentModel.Design;
using System.Net.NetworkInformation;



namespace UltraSkins.UI
{
    internal class MenuManager : MonoBehaviour
    {

        ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
        internal static MenuManager MMinstance { get; private set; }
        List<GameObject> loadedButtons = new List<GameObject>();
        Dictionary<string, Button> AvailbleSkins = new Dictionary<string, Button>();
        public SkinDetails SD = null;

        void Awake()
        {
            MMinstance = this;

        }



        public void Closeskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject fallnoiseoff, Animator animator, ObjectActivateInSequence oais, GameObject content)
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
            BatonPass.Debug("Building Button for " + subfolder);
            string folder = Path.GetFileName(subfolder);
            string metadataPath = Path.Combine(subfolder, "metadata.GCMD");
            string PackPath = Path.Combine(subfolder, "pack.GCMD");
            GameObject instance = Instantiate(prefab, contentfolder.transform);
            instance.SetActive(true);

            Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
            BatonPass.Debug("Looking for BEM");
            ButtonEnableManager BEM = instance.GetComponent<ButtonEnableManager>();

            BEM.skinDetails = SD;

            if (Location == "r2modman" || Location == "Thunderstore")
            {
                BEM.isThunderstore = true;
            }

            if (File.Exists(metadataPath))
            {
                try
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
                        if (MD.Version != null || MD.Version != "")
                        {
                            BEM.VerNum = MD.Version;
                        }


                    }
                    else
                    {
                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                        BEM.SkinName = folder;
                        BEM.warning = true;
                    }
                }
                catch (Exception ex)
                {
                    BatonPass.Error("Something went wrong with the metadata formatting, falling back");
                    BEM.SkinName = folder;
                    BatonPass.Error(ex.ToString());
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


        void orderifier(ObjectActivateInSequence activateanimator, GameObject contentfolder)
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
            List<(Transform trans, int spot, ButtonEnableManager bem)> finalorder = new List<(Transform trans, int spot, ButtonEnableManager bem)>();
            foreach (Transform child in contentfolder.transform)
            {
                ButtonEnableManager BEM = child.GetComponent<ButtonEnableManager>();
                string specialpath = BEM.filePath;
                if (filepathrev.Contains(specialpath))
                {
                    int index = System.Array.IndexOf(filepathrev, specialpath);
                    BatonPass.Debug(child.name + " has path: " + specialpath + " and index of: " + index);
                    finalorder.Add((child, index, BEM));


                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(specialpath);
                    string hexString = BitConverter.ToString(bytes).Replace("-", " ");
                    BatonPass.Debug($"no match for {specialpath} \n Logging Bytes {hexString}");
                }
            }
            foreach ((Transform trans, int spot, ButtonEnableManager bem) in finalorder.OrderBy(x => x.spot))
            {
                trans.SetSiblingIndex(spot);
                bem.IsEnabled = true;

            }


        }


        // Generate Buttons for pause menu usage
        public void GenerateButtons(GameObject contentfolder)
        {
            Dictionary<string, string> Locations = SkinEventHandler.GetCurrentLocations();
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
                                }
                                catch (Exception ex)
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
            foreach (Transform childTransform in content.transform)
            {
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





        public async void CreateSkinFromEditor(EditMenuManager eMMAN)
        {
            CampCreator camp = eMMAN.campCreator;
            camp.gameObject.SetActive(false);
            EditMenuInfoBox infobox = eMMAN.editMenuInfoBox;
            infobox.TextSlot1.text = "Generating Skins";
            infobox.TextSlot2.text = "";
            infobox.gameObject.SetActive(true);
            infobox.GoBack.gameObject.SetActive(false);
            infobox.OpenFolder.gameObject.SetActive(false);
            infobox.OpenFolder.onClick.RemoveAllListeners();
            infobox.ReturnToMainMenu.gameObject.SetActive(false);
            infobox.ForceSetThrobber(0, Color.white);
            infobox.StartThrobber();
            CampCreator.TEMPGCMD tempSkinInfo = camp.GatherInfo();
            UnsafeNotice NameCheck = CheckIfUnsafe(tempSkinInfo.Name);
            if (!NameCheck.IsSafe)
            {
                // Stop the throbber at 45 degrees
                infobox.StopThrobber(45,Color.red);
                infobox.setText1(NameCheck.Reason1);
                infobox.setText2(NameCheck.Reason2);
                BatonPass.Warn($"{NameCheck.Reason1},{NameCheck.Reason2}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-UNSAFE_SKIN_NAME\"");
                infobox.GoBack.gameObject.SetActive(true);

                return;
            }
            GCMD gcmd = new GCMD { SkinName = tempSkinInfo.Name , Description = tempSkinInfo.Description};
            if (tempSkinInfo.Version != null && tempSkinInfo.Version == "")
            {
                gcmd.Version = tempSkinInfo.Version;
            }
            string ProjectPath = Path.Combine(USC.GCDIR, USC.UNISKIN, tempSkinInfo.Name);
            if (Directory.Exists(ProjectPath)) {
                infobox.StopThrobber(45, Color.red);
                infobox.setText1($"Sorry but the folder {tempSkinInfo.Name} already exists");
                infobox.setText2("Please try a different name");
                BatonPass.Warn($" {tempSkinInfo.Name}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-FOLDER_NAME_CONFILCT\"");
                infobox.GoBack.gameObject.SetActive(true);
                return;
            }
            try
            {
                Directory.CreateDirectory(ProjectPath);
            }
            catch (Exception e) {
                infobox.StopThrobber(45, Color.red);
                infobox.setText1($"an error occurred while creating {ProjectPath}");
                infobox.setText2(e.Message + " CODE -\"MMAN-SKIN_CREATOR_ENGINE-DIR_FAIL_AT_PROJECTPATH\"");
                infobox.GoBack.gameObject.SetActive(true);
               
                BatonPass.Error($"HEAR YE, HEAR YE. an error occurred while creating {ProjectPath}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-DIR_FAIL_AT_PROJECTPATH\"");
                BatonPass.Error(e.ToString());
                return;
            }

            metadataWriter metadataWriter = new metadataWriter();
            await metadataWriter.WriteMD(ProjectPath, gcmd);

            if (camp.TemplateMode == true)
            {
                try
                {
                    OGSkinsManager OGS = ULTRASKINHand.HandInstance.ogSkinsManager;
                    int batchmax = ULTRASKINHand.HandInstance.settingsmanager.GetSettingValue<int>("MaxTextureToPngPerFrame");
                    if (batchmax <= 0)
                    {
                        throw new BPBadSettingsValue("MaxTextureToPngPerFrame", batchmax.ToString());
                    }
                    StartCoroutine(ConvertToPNG(OGS.OGSKINS, batchmax, async pngs =>
                    {
                        if (pngs != null)
                        {
                            TaskStatus status = await PNGFileCreatorAsync(pngs, ProjectPath);
                            if (status.CurrentStatus == TaskStatus.status.WithErrors)
                            {
                                infobox.StopThrobber(0, Color.yellow);
                                infobox.setText1($"Success.... Mostly, The following failed to load");
                                infobox.setText2($"{string.Join(",", status.FailedValues)}");
                                infobox.GoBack.gameObject.SetActive(true);
                                infobox.OpenFolder.gameObject.SetActive(true);
                                infobox.ReturnToMainMenu.gameObject.SetActive(true);
                                infobox.OpenFolder.onClick.AddListener(() => OpenShellFileExplorer(ProjectPath));
                                BatonPass.Warn($"PNG converter finished with errors, the following failed to convert");
                                foreach (string value in status.FailedValues)
                                {
                                    BatonPass.Warn($"{value}");
                                }
                                return;
                            }
                            else if (status.CurrentStatus == TaskStatus.status.Success)
                            {
                                infobox.StopThrobber(0, Color.green);
                                infobox.setText1($"Success");
                                infobox.setText2($"");
                                infobox.OpenFolder.gameObject.SetActive(true);
                                infobox.ReturnToMainMenu.gameObject.SetActive(true);
                                infobox.OpenFolder.onClick.AddListener(() => OpenShellFileExplorer(ProjectPath));
                                BatonPass.Success($"The SkinSet was created Successfully");

                                return;
                            }
                            return;
                        }
                        else
                        {
                            infobox.StopThrobber(45, Color.red);
                            infobox.setText1($"PNG converter returned NULL");
                            infobox.setText2("CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-PNG_MAKER_NULL\"");
                            infobox.GoBack.gameObject.SetActive(true);
                            BatonPass.Error($"PNG converter returned NULL, CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-PNG_MAKER_NULL\"");
                            return;
                        }
                    }));
                }
                catch (BPBadSettingsValue e)
                {
                    infobox.StopThrobber(45, Color.red);
                    infobox.setText1("Whoa there, you almost trapped yourself");
                    infobox.setText2(e.Message + ", CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-BAD_SETTINGS_VALUE\"");
                    infobox.GoBack.gameObject.SetActive(true);
                    BatonPass.Error($"{e.Message}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-BAD_SETTINGS_VALUE\"");
                    BatonPass.Error(e.ToString());
                    return;
                }
                catch (Exception e) {

                    infobox.StopThrobber(45, Color.red);
                    infobox.setText1($"Don't Panic, but i have no idea why you are seeing this error");
                    infobox.setText2(e.Message + ", CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-EX\"");
                    infobox.GoBack.gameObject.SetActive(true);
                    BatonPass.Error($"{e.Message}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-EX\"");
                    BatonPass.Error(e.ToString());
                    return;
                }

            }
            else
            {
                infobox.setText1("EpicPoggiesworkies");
                infobox.GoBack.gameObject.SetActive(true);
            }






        }
        internal void OpenShellFileExplorer(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = folderPath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            else
            {
                BatonPass.Warn($"Folder not found: {folderPath}");
            }
        }

        private IEnumerator ConvertToPNG(Dictionary<string, Texture> SKINTEXs,int MAXBATCH, Action<Dictionary<string, byte[]>> complete)
        {
            Dictionary <string, byte[]> SKINPNGs = new Dictionary <string, byte[]>();
            int batch = 0;
            
            foreach (var KVP in SKINTEXs) {

                Texture2D tex2D = ConvertToReadable(KVP.Value);
                if (tex2D != null)
                {
                    
                    byte[] png = tex2D.EncodeToPNG();
                    SKINPNGs.Add(KVP.Key, png);

                    Destroy(tex2D);
                    // Use tex2D here
                }
                else
                {
                    BatonPass.Warn($"{KVP.Key}: Not a Texture2D");
                    SKINPNGs.Add(KVP.Key, null);
                }
                batch++;
                if (batch >= MAXBATCH)
                {
                    batch = 0;
                    yield return null;
                }
            }
            complete?.Invoke(SKINPNGs);


        }
        Texture2D ConvertToReadable(Texture source)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                source.width, source.height, 0,
                RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            Graphics.Blit(source, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;

            Texture2D readableTex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            readableTex.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            readableTex.Apply();

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);

            return readableTex;
        }


        internal async Task<TaskStatus> PNGFileCreatorAsync(Dictionary<string, byte[]> IMAGES, string folderPath)
        {
            var failedItems = new List<string>();
            foreach (var KVP in IMAGES) 
            { 
                if (KVP.Value != null)
                {
                    try
                    {
                        string filename = Path.Combine(folderPath, KVP.Key + ".png");
                        if (!File.Exists(filename))
                        {
                            await File.WriteAllBytesAsync(filename, KVP.Value);
                        }
                        else { BatonPass.Warn($"A file called {KVP.Value}.png already exists in {folderPath}, Skipping"); }
                    }
                    catch (Exception ex) 
                    {
                        BatonPass.Error($"Something went wrong writing {KVP.Key}, CODE - \"MMAN-PNG_CREATE_ASYNC-WRITE_ISSUE");
                        BatonPass.Error(ex.ToString());
                    }


                }
                else
                {
                    failedItems.Add(KVP.Key);
                }
            
            
            }
            
             if (failedItems.Count == 0)
                return new TaskStatus(TaskStatus.status.Success);
            else
                return new TaskStatus(TaskStatus.status.WithErrors, failedItems, "Some PNGs failed to save");
        }

        internal struct TaskStatus
        {
            public enum status {
                Success,
                WithErrors,
                Failed,
            }
            public status CurrentStatus { get; set; }
            public List<string>? FailedValues { get; set; }
            public string ExtraNote { get; set; }

            public TaskStatus(status status, List<string>? failedValues = null, string extraNote = "")
            {
                CurrentStatus = status;
                FailedValues = failedValues;
                ExtraNote = extraNote;
            }

        }


        public UnsafeNotice CheckIfUnsafe(string input)
        {
            string normalized = input.Replace('\\', '/');


            if (normalized.Contains("../"))
            {
                return UnsafeNotice.Unsafe("I know what your trying to do (._.)","Please do not try and escape the confines of the Ultraskins folder (../).");
            }
            if (normalized.StartsWith("/"))
            {
                return UnsafeNotice.Unsafe("Save root folder access for another day", "Slashes are not permitted");

            }
            try
            {
                if (Path.IsPathRooted(input))
                {
                    return UnsafeNotice.Unsafe("Unfortunately we dont support your... \"unique\" name", "Skins may only be created in the Ultraskins folder");
                }
            }
            catch (ArgumentException) { }  // invalid chars caught below



            char[] invalidChars = Path.GetInvalidFileNameChars();
            char[] foundInvalid = input.Where(c => invalidChars.Contains(c)).Distinct().ToArray();
            if (foundInvalid.Length > 0)
            {
                if (foundInvalid.Length == 1) 
                { 
                    return UnsafeNotice.Unsafe("Hmm I dont recognize that symbol", $"Invalid character: {string.Join(" ", foundInvalid)}");
                }
                return UnsafeNotice.Unsafe("Sorry Ultraskins names may not contain ancient runes", $"Invalid characters: {string.Join(" ", foundInvalid)}");
            }
            string cleaned = new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
            if (USC.ReservedNameJokes.TryGetValue(cleaned, out string value)) 
            {
                if (cleaned == "OG-SKINS")
                {
                    return UnsafeNotice.Unsafe(value, $"The name \"{cleaned}\" may not be used because I said so");
                }
                return UnsafeNotice.Unsafe(value, $"The name \"{cleaned}\" may not be used because microsoft said so");
            }




            return UnsafeNotice.Safe();
        }

        public struct UnsafeNotice
        {
            public bool IsSafe;
            public string Reason1;
            public string Reason2;
            public UnsafeNotice(bool isSafe, string reason1, string reason2 = "")
            {
                IsSafe = isSafe;
                Reason1 = reason1;
                Reason2 = reason2;

            }

            public static UnsafeNotice Safe() => new UnsafeNotice(true, "","");
            public static UnsafeNotice Unsafe(string reason1,string reason2 = "") => new UnsafeNotice(false, reason1,reason2);
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
                BatonPass.Warn($"The metadata.GCMD File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} . Code -\"MMAN-MDR-READMD-METADATA_READ_WARNING\"");
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
                BatonPass.Warn($"The Pack.GCMD File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} . Code -\"MMAN-MDR-READPACK-PACK_READ_WARNING\"");
                return null;


            }
;
        }
    }
    internal struct MDWriteReturn
    {
        bool worked;
        string message;
        internal MDWriteReturn(bool Worked, string Message) { 
            worked = Worked;
            message = Message;

        }
        public static MDWriteReturn good() => new MDWriteReturn(true, "");
        public static MDWriteReturn bad(string message) => new MDWriteReturn(false, message);
    }

    class metadataWriter
    {
        public async Task<MDWriteReturn> WriteMD(string Folder,GCMD gcmd)
        {
            
            string filepath = Path.Combine(Folder, "metadata.GCMD");
            try
            {

                

                    if (File.Exists(filepath))
                    {
                        throw new BPFileExistsExc(filepath);
                    }
                    var MD = JsonConvert.SerializeObject(gcmd);
                    await File.WriteAllTextAsync(filepath, MD);
                
                        return MDWriteReturn.good();
            }
            catch (JsonException ex)
            {
                BatonPass.Error($"\"{filepath}\" failed to serialize . Code -\"MMAN-MDW-WRITEMD-METADATA_WRITE_FAILURE\"");
                BatonPass.Error(ex.ToString());
                return MDWriteReturn.bad($"\"{filepath}\" failed to serialize .");
            }
            catch (BPFileExistsExc ex)
            {
                BatonPass.Error($"\"{filepath}\" already exists . Code -\"MMAN-MDW-WRITEMD-METADATA_ALREADY_EXISTS\"");
                BatonPass.Error(ex.ToString());
                return MDWriteReturn.bad($"\"{filepath}\" already exists ."); 
            }
            catch (Exception ex)
            {

                BatonPass.Error($"an error occurred while trying to create {filepath}. Code -\"MMAN-MDW-WRITEMD-EX\"");
                BatonPass.Error(ex.ToString());
                return MDWriteReturn.bad($"an error occurred while trying to create {filepath}"); 
            }



        }
    }

    
}

