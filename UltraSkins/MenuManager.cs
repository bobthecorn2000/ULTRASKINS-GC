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
using Mono.Cecil.Cil;
using System.ComponentModel;
using BepInEx;
using System.Diagnostics;
using BatonPassLogger.GUI;
using Unity.Audio;




namespace UltraSkins.UI
{
    internal class MenuManager : MonoBehaviour
    {

        ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
        internal static MenuManager MMinstance { get; private set; }
        List<GameObject> loadedButtons = new List<GameObject>();
        Dictionary<string, Button> AvailbleSkins = new Dictionary<string, Button>();
        public SkinDetails SD = null;
        public GameObject RefreshableContentFolder;
        public ObjectActivateInSequence RefreshableActivateAnimator;
        public BPGUIManager BPGUI = BPGUIManager.Instance;

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
            //float AnimStartTime = Time.realtimeSinceStartup;
            animator.Play("menuclose");
            //BatonPass.Debug("starting Anim, the time is " + AnimStartTime);
            // Start the coroutine
            MMinstance.StartCoroutine(MMinstance.DisableAfterCloseAnimation(mainmenucanvas, Configmenu, animator));

        }

        private IEnumerator DisableAfterCloseAnimation(GameObject mainmenucanvas, GameObject Configmenu, Animator animator)
        {
            // Wait until the animation finishes
            // float coroutineStartTime = Time.realtimeSinceStartup;
            //BatonPass.Debug("starting coroutine, the time is " +  coroutineStartTime);
            //float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(0.4167f);

            //BatonPass.Debug("coroutineDone, We waited " + animationLength);
            // Disable the GameObject (for example, the main menu canvas)
            Configmenu.SetActive(false);
            mainmenucanvas.SetActive(true);
            //float coroutineEndTime = Time.realtimeSinceStartup;
            //BatonPass.Debug("exitTime " + coroutineEndTime);
        }


        /// <summary>
        /// A generator for skin buttons, mostly used in main menus
        /// </summary>
        /// <param name="contentfolder">The Folder to act as the parent</param>
        /// <param name="activateanimator">An instance of ObjectActivateInSequence, should be attached to the contentfolder</param>
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
                                        
                                        UnsafeNotice safe = CheckIfUnsafe(path);
                                        if (safe.IsSafe)
                                        {
                                            string subsubfolder = Path.Combine(subfolder, path);
                                            if (Directory.Exists(subsubfolder) && safe.IsSafe)
                                            {
                                                BuildButton(Location, contentfolder, MDR, prefab, subsubfolder);
                                            }
                                            else
                                            {
                                                BatonPass.Warn("\"" + subsubfolder + "\" does not exist in the Pack. This is most likely an incorrect folder name in the Pack.GCMD file located in \"" + PackPath + "\". Code -\"MMAN-GENERATEBUTTONS-MAINMENU-PACKPATH_MISSING_LOCATION\"");
                                            }
                                        }
                                        else
                                        {
                                            BatonPass.Warn("Refusing to load Subdirectory at " + path);
                                            BatonPass.Warn(safe.Reason1);
                                            BatonPass.Warn(safe.Reason2);
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



        /// <summary>
        /// Builds a single prefab button based on a file path and places it as a child of a gameobject
        /// </summary>
        /// <param name="Location">determining where this button came from</param>
        /// <param name="contentfolder">the game object to act as the parent</param>
        /// <param name="MDR"> a metadata reader</param>
        /// <param name="prefab">the gameobject prefab, will throw if the prefab doesnt have a BEM in the root</param>
        /// <param name="subfolder"> the folder this button is based on</param>
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
                BEM.thunderstore = true;
            }

            if (File.Exists(metadataPath))
            {
                try
                {
                    GCMD MD = MDR.ReadMD(metadataPath);
                    string warningMessage = null;
                    if (MD != null)
                    {
                        if (MD.PackFormat.IsNullOrWhiteSpace())
                        {
                            BEM.warning = true;
                            warningMessage = "Pack Format is missing, Skin may have issues";
                        }
                        else
                        {
                            if (!USC.SupportedPackFormats.Contains(MD.PackFormat))
                            {
                                warningMessage = "Made for a different version of ultraskins";
                                BEM.warning = true;
                            }

                        }

                        if (!MD.Description.IsNullOrWhiteSpace())
                        {
                            BEM.SkinDescription = MD.Description;
                        }
                        else
                        {
                            BEM.SkinDescription = "No Info";
                        }
                        if (!MD.SkinName.IsNullOrWhiteSpace())
                        {
                            BEM.SkinName = MD.SkinName;
                            ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = MD.SkinName;
                        }
                        else
                        {
                            BEM.SkinName = folder;
                        }
                        if (!MD.Author.IsNullOrWhiteSpace())
                        {
                            BEM.Author = MD.Author;
                        }
                        else
                        {
                            BEM.Author = "Unknown";
                        }


                        if (MD.SupportedPlugins != null && MD.SupportedPlugins.Count >= 1)
                        {
                            BEM.isplugin = true;
                        }
                        if (!MD.Version.IsNullOrWhiteSpace())
                        {
                            BEM.VerNum = MD.Version;
                        }
                        if (!string.IsNullOrWhiteSpace(MD.IconOveride))
                        {
                            UnsafeNotice unsafeNotice = CheckIfUnsafe(MD.IconOveride);
                            if (unsafeNotice.IsSafe)
                            {
                                ICFinder(subfolder, BEM, MD.IconOveride);
                            }
                            else
                            {
                                BatonPass.Warn("Skipping Unsafe Icon for " + MD.IconOveride);
                            }
                        }
                        else
                        {
                            ICFinder(subfolder, BEM);
                        }


                    }
                    else
                    {
                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                        BEM.SkinName = folder;
                        BEM.warning = true;
                        warningMessage = "The metadata file is null";
                        ICFinder(subfolder, BEM);
                    }
                    if (warningMessage != null)
                    {
                        BEM.warningmessage = warningMessage;
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

            }
            instance.name = folder;
            BEM.filePath = subfolder;
            AvailbleSkins.Add(folder + subfolder.GetHashCode(), ultraskinsbutton);
            // Add button to list
            BEM.DecideColor();
            BEM.makeObject();
            BatonPass.Debug("Successfully loaded and instantiated ultraskinsButton.");


        }

        public async void ICFinder(string subfolder, ButtonEnableManager BEM, string IconName = "icon.png")
        {
            try
            {
                string subhash = subfolder.GetHashCode().ToString();
                if (handInstance.IconCache.TryGetValue(subhash, out Texture2D icon))
                {
                    BEM.RawIcon = icon;
                    BEM.RawIcon.Apply();
                }
                else
                {
                    string path = Path.Combine(subfolder, IconName);
                    if (File.Exists(path))
                    {
                        BatonPass.Debug("Searching for icon " + path);
                        byte[] image = await LoadSingleIcon(path);
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.name = IconName;

                        BatonPass.Debug("Creating " + texture2D.name);
                        BEM.RawIcon = texture2D;
                        texture2D.filterMode = FilterMode.Point;

                        HoldEm.Bet(HoldEm.HoldemType.IC, subhash, texture2D);
                        BEM.RawIcon.LoadImage(image);
                        BEM.RawIcon.Apply();
                    }



                }
            }
            catch (Exception ex)
            {

                BatonPass.Error($"{ex.Message}, Code -\"MMAN-ICFINDER-EX\"");
                BatonPass.Error(ex.ToString());

            }
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
            /*  BatonPass.Debug("We know the following files");
            int debugtracker = 0;
            foreach (string item in filepathrev)
            {

                byte[] bytes = Encoding.UTF8.GetBytes(item);
                string hexString = BitConverter.ToString(bytes).Replace("-", " ");
                BatonPass.Debug($"pos:{debugtracker} path:{item} \n Logging Bytes: {hexString}");
                debugtracker++;
            }*/
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
                    /* byte[] bytes = Encoding.UTF8.GetBytes(specialpath);
                     string hexString = BitConverter.ToString(bytes).Replace("-", " ");*/
                    BatonPass.Debug($"no match for {specialpath}");
                }
            }
            foreach ((Transform trans, int spot, ButtonEnableManager bem) in finalorder.OrderBy(x => x.spot))
            {
                trans.SetSiblingIndex(spot);
                bem.IsEnabled = true;

            }


        }


        // Generate Buttons for pause menu usage
        /// <summary>
        /// an alternative generator for use in pause menus, May be removed soon
        /// </summary>
        /// <param name="contentfolder"></param>
        [Obsolete("To Be Removed")]
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


        public async Task<byte[]> LoadSingleIcon(string path)
        {
            if (File.Exists(path))
            {
                byte[] data = await File.ReadAllBytesAsync(path);
                return data;
            }
            return null;
        }


        // In Editor Skin Maker thing


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
                infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
                infobox.setText1(NameCheck.Reason1);
                infobox.setText2(NameCheck.Reason2);
                BatonPass.Warn($"{NameCheck.Reason1},{NameCheck.Reason2}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-UNSAFE_SKIN_NAME\"");
                infobox.GoBack.gameObject.SetActive(true);

                return;
            }
            GCMD gcmd = new GCMD { SkinName = tempSkinInfo.Name, Description = tempSkinInfo.Description, PackFormat = USC.GCSKINVERSION };
            if (tempSkinInfo.Version != null && tempSkinInfo.Version != "")
            {
                gcmd.Version = tempSkinInfo.Version;
            }
            if (!string.IsNullOrWhiteSpace(tempSkinInfo.Author))
            {
                gcmd.Author = tempSkinInfo.Author;
            }
            string ProjectPath = Path.Combine(USC.GCDIR, USC.UNISKIN, tempSkinInfo.Name);
            if (Directory.Exists(ProjectPath))
            {
                infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
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
            catch (Exception e)
            {
                infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
                infobox.setText1($"an error occurred while creating {ProjectPath}");
                infobox.setText2(e.Message + " CODE -\"MMAN-SKIN_CREATOR_ENGINE-DIR_FAIL_AT_PROJECTPATH\"");
                infobox.GoBack.gameObject.SetActive(true);

                BatonPass.Error($"HEAR YE, HEAR YE. an error occurred while creating {ProjectPath}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-DIR_FAIL_AT_PROJECTPATH\"");
                BatonPass.Error(e.ToString());
                return;
            }

            metadataWriter metadataWriter = new metadataWriter();
            await metadataWriter.WriteMD(ProjectPath, gcmd);
            if (!string.IsNullOrWhiteSpace(tempSkinInfo.iconpath))
            {
                if (File.Exists(tempSkinInfo.iconpath))
                {
                    try
                    {
                        File.Copy(tempSkinInfo.iconpath, Path.Combine(ProjectPath,"icon.png"));
                    }
                    catch (Exception ex)
                    {
                        BatonPass.Error($"Icon failed to be copied into the new location, CODE -\"MMAN-SKIN_CREATOR_ENGINE-ICON_COPY_FAIL\"");
                    }
                    
                }
            }
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
                                infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
                                infobox.setText1($"Success.... Mostly, The following failed to load");
                                infobox.setText2($"{string.Join(",", status.FailedValues)}");
                                //infobox.GoBack.gameObject.SetActive(true);
                                infobox.OpenFolder.gameObject.SetActive(true);
                                infobox.ReturnToMainMenu.gameObject.SetActive(true);
                                infobox.OpenFolder.onClick.AddListener(() => OpenShellFileExplorer(ProjectPath));
                                BatonPass.Warn($"PNG converter finished with errors, the following failed to convert");
                                foreach (string value in status.FailedValues)
                                {
                                    BatonPass.Warn($"{value}");
                                }
                                camp.ClearAllEntrys();
                                RefreshHandlerPage();
                                return;
                            }
                            else if (status.CurrentStatus == TaskStatus.status.Success)
                            {
                                infobox.StopThrobber(EditMenuInfoBox.SpinState.Good);
                                infobox.setText1($"Success");
                                infobox.setText2($"");
                                infobox.OpenFolder.gameObject.SetActive(true);
                                infobox.ReturnToMainMenu.gameObject.SetActive(true);
                                infobox.OpenFolder.onClick.AddListener(() => OpenShellFileExplorer(ProjectPath));
                                BatonPass.Success($"The SkinSet was created Successfully");
                                camp.ClearAllEntrys();
                                RefreshHandlerPage();
                                return;
                            }
                            return;
                        }
                        else
                        {
                            infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
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
                    infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
                    infobox.setText1("Whoa there, you almost trapped yourself");
                    infobox.setText2(e.Message + ", CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-BAD_SETTINGS_VALUE\"");
                    infobox.GoBack.gameObject.SetActive(true);
                    BatonPass.Error($"{e.Message}, CODE -\"MMAN-SKIN_CREATOR_ENGINE-TEMPLATE-BAD_SETTINGS_VALUE\"");
                    BatonPass.Error(e.ToString());
                    return;
                }
                catch (Exception e)
                {

                    infobox.StopThrobber(EditMenuInfoBox.SpinState.Bad);
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

                infobox.StopThrobber(EditMenuInfoBox.SpinState.Good);
                infobox.setText1($"Success");
                infobox.setText2($"");
                infobox.OpenFolder.gameObject.SetActive(true);
                infobox.ReturnToMainMenu.gameObject.SetActive(true);
                infobox.OpenFolder.onClick.AddListener(() => OpenShellFileExplorer(ProjectPath));

            }


            RefreshHandlerPage();



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

        private IEnumerator ConvertToPNG(Dictionary<string, Texture> SKINTEXs, int MAXBATCH, Action<Dictionary<string, byte[]>> complete)
        {
            Dictionary<string, byte[]> SKINPNGs = new Dictionary<string, byte[]>();
            int batch = 0;

            foreach (var KVP in SKINTEXs)
            {

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
            public enum status
            {
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
                return UnsafeNotice.Unsafe("I know what your trying to do (._.)", "Please do not try and escape the confines of the Ultraskins folder (../).");
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

            public static UnsafeNotice Safe() => new UnsafeNotice(true, "", "");
            public static UnsafeNotice Unsafe(string reason1, string reason2 = "") => new UnsafeNotice(false, reason1, reason2);
        }



        /// <summary>
        /// Refreshes the current MenuManager Page and regenerates all the buttons
        /// </summary>
        public void RefreshHandlerPage()
        {
            int childCount = RefreshableContentFolder.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                GameObject child = RefreshableContentFolder.transform.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
            RefreshableContentFolder.SetActive(false);
            //loadedButtons.Clear();
            // AvailbleSkins.Clear();
            GenerateButtons(RefreshableContentFolder, RefreshableActivateAnimator);
            RefreshableContentFolder.SetActive(true);

        }



        // Thunderstore Coverter Manager Stuff
        /// <summary>
        /// adds a button for each folder found in the thunderstore directory
        /// </summary>
        /// <param name="handle">the ThunderstoreHandle Component to populate</param>
        /// <param name="eMM">the EditMenuManager to use for filling out the folder</param>
        public void PopulateThunderstoreMenu(ThunderstoreHandle handle, EditMenuManager eMM)
        {
            if (handInstance.ThunderStoreMode)
            {
                string name = USC.MODPATH;
                string parentDir = Directory.GetParent(name).FullName;
                string[] subfolders = Directory.GetDirectories(parentDir);
                foreach (string folder in subfolders)
                {
                    if (!File.Exists(Path.Combine(folder, USC.MDFILE)) && !File.Exists(Path.Combine(folder, USC.MDFILE + ".old")) && !File.Exists(Path.Combine(folder, USC.PACKFILE)) && !File.Exists(Path.Combine(folder, USC.PACKFILE + ".old")))
                    {
                        if (folder != name)
                        {
                            handle.AddOptionToUI(folder);
                        }
                    }

                }
                handle.ContinueButton.onClick.RemoveAllListeners();
                handle.ContinueButton.onClick.AddListener(() => TStoList(handle, eMM));
            }
        }


        /// <summary>
        /// Takes the selected file in a particular thunderstorehandle and adds metadata to it 
        /// </summary>
        /// <param name="handle">the ThunderstoreHandle Component to grab from</param>
        /// <param name="eMM">the EditMenuManager to use for toggling pages after completion</param>
        private void TStoList(ThunderstoreHandle handle, EditMenuManager eMM)
        {
            string file = handle.GetSelectedFile();

            if (Directory.Exists(file))
            {
                string mdfilepath = Path.Combine(file, USC.MDFILE);
                string TSManiPath = Path.Combine(file, USC.TSMANIFEST);
                string filename = Path.GetFileName(file);
                GCMD gcmd = new GCMD();
                try
                {



                    string[] parts = filename.Split('-');
                    if (parts.Length > 1)
                    {
                        gcmd.Author = parts[0];
                    }
                    else
                    {
                        gcmd.Author = "Thunderstore Import Tool";
                    }

                    if (File.Exists(TSManiPath))
                    {
                        metadataReader MDR = new metadataReader();

                        TSjson tsmani = MDR.ReadTSmani(TSManiPath);
                        if (tsmani != null)
                        {

                            gcmd.Version = tsmani.version_number;
                            gcmd.SkinName = tsmani.name;
                            gcmd.Description = tsmani.description;
                            TSFinalizeData(file, gcmd);
                            //gcmd.Author = "Thunderstore Import Tool";

                        }
                        else
                        {
                            BatonPass.Warn("ew, i cant read that metadata, i might throw up");
                            throw new BPBadData(TSManiPath);
                        }
                    }
                    else
                    {
                        if (parts.Length > 1)
                        {
                            gcmd.SkinName = parts[1];
                        }
                        else
                        {
                            gcmd.SkinName = filename;
                        }
                        gcmd.Description = "Created with Thunderstore Import Tool";
                        TSFinalizeData(file, gcmd);

                    }
                }
                catch (BPBadData BPex)
                {
                    BatonPass.Error("it appears i threw an error because the thunderstore json file was not readable, Code -\"MMAN-TSTOLIST-UNREADABLE_THUNDERSTORE_JSON\"");
                    BatonPass.Error(BPex.ToString());
                    TSRepairOnFail(file, gcmd);
                }
                catch (Exception E)
                {
                    BatonPass.Error("HELP SOMETHING WENT WRONG AND I DONT KNOW WHAT, Code -\"MMAN-TSTOLIST-EX\"");
                    BatonPass.Error(E.ToString());
                    TSRepairOnFail(file, gcmd);
                }


            }
            else
            {
                BatonPass.Error("well there isnt a directory here, Code -\"MMAN-TSTOLIST-DIR_MISSING_MID_READ\"");
            }

            handle.gameObject.SetActive(false);

            RefreshHandlerPage();
            handle.ClearUI();
            PopulateThunderstoreMenu(handle, eMM);
            eMM.Editor.SetActive(true);
        }


        /// <summary>
        /// finishes the GCMD and writes it to the path given
        /// </summary>
        /// <param name="file">Path you want to write the metadata.gcmd to</param>
        /// <param name="gcmd">the GCMD object to write</param>
        private async void TSFinalizeData(string file, GCMD gcmd)
        {
            gcmd.PackFormat = USC.GCSKINVERSION;
            metadataWriter MDW = new metadataWriter();
            MDWriteReturn status = await MDW.WriteMD(file, gcmd);

        }


        /// <summary>
        /// Can be called to fix a crashed generation during GCMD creation
        /// </summary>
        /// <param name="file">Path you want to write the metadata.gcmd to</param>
        /// <param name="gcmd">the GCMD object to write</param>
        private void TSRepairOnFail(string file, GCMD gcmd)
        {
            gcmd.SkinName = Path.GetFileName(file);
            gcmd.Description = "Created with Thunderstore Import Tool";
            TSFinalizeData(file, gcmd);
        }




        // Migration Tool Stuff
        public void PopulateMigrateMenu(MigrationPage MP, EditMenuManager eMM)
        {
            string legdir = USC.LEGACYGCDIR;
            string newdir = Path.Combine(USC.GCDIR,USC.UNISKIN);
            string[] legacyfolders = Directory.GetDirectories(legdir);
            List<string> legfolname = legacyfolders.Select(path => Path.GetFileName(path).ToLowerInvariant()).ToList();
            string[] knownfolders = Directory.GetDirectories(newdir);
            
            foreach (string knownfolder in knownfolders)
            {
                string knownname = Path.GetFileName(knownfolder).ToLowerInvariant(); 
                if (legfolname.Contains(knownname))
                {
                    MP.AddOptionToUnsupported(Path.Combine(legdir, knownname));
                    legfolname.Remove(knownname);
                }
            }
            
            if (legfolname.Contains("og-skins"))
            {
                legfolname.Remove("og-skins");
                MP.AddOptionToUnsupported(Path.Combine(legdir, "og-skins"));
            }

            foreach (string legfol in legfolname)
            {
                MP.AddOptionToSupported(Path.Combine(legdir, legfol));
            }
            MP.continuebutton.onClick.RemoveAllListeners();
            MP.continuebutton.onClick.AddListener(async () => await startMigrationProcess(MP, eMM));
        }
        internal async Task startMigrationProcess(MigrationPage MP, EditMenuManager eMM)
        {
            if (MP.migratableFolders.Count() > 0)
            {
                BPGUI.ShowGUI("GatheringInfo");
                eMM.MigrationPage.gameObject.SetActive(false);
                BPGUI.EnableTerminal(10);
                foreach (string sourcedir in MP.migratableFolders)
                {
                    
                    string folderName = new DirectoryInfo(sourcedir).Name;
                    string newdir = Path.Combine(USC.GCDIR, USC.UNISKIN, folderName);
                    if (Directory.Exists(sourcedir) && !Directory.Exists(newdir))
                    {
                        BPGUI.AddTermLine("Migrating " + folderName);
                        Directory.CreateDirectory(newdir);
                        foreach (string filepath in Directory.GetFiles(sourcedir))
                        {
                            string fileName = Path.GetFileName(filepath);
                            BPGUI.AddTermLine("Moving " + fileName);
                            BatonPass.Debug("Moving" + fileName);
                            string destFile = Path.Combine(newdir, fileName);
                            try
                            {
                                await CopyFileAsync(filepath, destFile);
                                BPGUI.AddTermLine("done");
                            }
                            catch (Exception EX)
                            {
                                BatonPass.Error(EX.Message + ", Code - \"MMAN-MIGRATEPROCESS-COPY_FILE_FAIL\" ");
                                BatonPass.Error(EX.ToString());
                                BPGUI.AddTermLine("<color=\"red\"> FAILED:" + EX.Message + "</color>");
                            }

                        }
                    }
                    else
                    {
                        BPGUI.AddTermLine("Skipping " + sourcedir);
                        BatonPass.Warn("Skipping " + sourcedir);
                    }

                }
                BPGUI.DisableTerminal();
                BPGUI.BatonPassAnnoucement(Color.white,"done");
                BPGUI.HideGUI(3);
                MP.ClearMenu();
                this.PopulateMigrateMenu(MP,eMM);
                RefreshHandlerPage();
                eMM.Editor.gameObject.SetActive(true);
            }
        }
        private static async Task CopyFileAsync(string source, string destination)
        {
            using FileStream sourceStream = File.Open(source, FileMode.Open, FileAccess.Read);
            using FileStream destinationStream = File.Create(destination);
            await sourceStream.CopyToAsync(destinationStream);
        }

    }















    /// <summary>
    /// Metadata Object
    /// </summary>
    class GCMD
    {
        public string SkinName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? IconOveride { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Version { get; set; }
        public string PackFormat { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string>? SupportedPlugins { get; set; }
    }

    /// <summary>
    /// Pack Object
    /// </summary>
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
        public TSjson ReadTSmani(string file)
        {
            string Jsonreader = File.ReadAllText(file);
            try
            {
                TSjson tsjson = JsonConvert.DeserializeObject<TSjson>(Jsonreader);
                return tsjson;
            }
            catch(JsonReaderException ex)
            {
                BatonPass.Warn($"The manifest.json File located at \"{file}\" could not be read, We think the error happened around line: {ex.LineNumber} character: {ex.LinePosition} . Code -\"MMAN-MDR-TSMANI-THUNDERSTORE_MANIFEST_READ_WARNING\"");
                return null;
            }
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
            
            string filepath = Path.Combine(Folder, USC.MDFILE);
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
    public class TSjson
    {


        public string name { get; set; }
        public string description { get; set; }

        public string version_number { get; set; }
    }

}

