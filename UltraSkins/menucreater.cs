using System;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UltraSkins;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BatonPassLogger;
using Unity.Audio;
using System.Collections.Generic;
using UltraSkins.Utils;
using BatonPassLogger.GUI;
using System.Threading.Tasks;



//using UnityEngine.UIElements;


namespace UltraSkins.UI
{
    internal class MenuCreator : MonoBehaviour
    {


        static ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
        static SettingsManager sMan
        {
            get => ULTRASKINHand.HandInstance.settingsmanager;
            set => ULTRASKINHand.HandInstance.settingsmanager = value;
        }
        public static string folderupdater;
        static GameObject batonpassGUIInst = null;
        
        public static void makethemenu()
        {
            BatonPass.Debug("BATON PASS: WE ARE IN MAKETHEMENU()");

            Scene scene = SceneManager.GetActiveScene();
            BatonPass.Info("The Scene is: " + scene.name);
            
            GameObject canvas = null;
            GameObject mainmenu;
            GameObject fallNoiseOn = null;
            GameObject fallNoiseOff = null;
            
            try
            {
                foreach (var rootsound in scene.GetRootGameObjects().Where(obj => obj.name == "FallSound"))

                {
                    fallNoiseOn = rootsound.transform.Find("ToOne").gameObject;
                    fallNoiseOff = rootsound.transform.Find("ToZero").gameObject;
                }

                foreach (var rootCanvas in scene.GetRootGameObjects().Where(obj => obj.name == "Canvas"))

                {



                    BatonPass.Info("entered a foreach");
                    mainmenu = rootCanvas.transform.Find("Main Menu (1)").gameObject;

                    BatonPass.Info("finished search");
                    BatonPass.Info("HEARYEE HEAR YEE TRANSFORM IS " + mainmenu.ToString());
                    GameObject V1;
                    bool usev1color = sMan.GetSettingValue<bool>("V1Color");

                    if (usev1color)
                    {
                        BatonPass.Info(ColorBlindSettings.Instance.variationColors[2].r + " " + ColorBlindSettings.Instance.variationColors[2].g + " " + ColorBlindSettings.Instance.variationColors[2].b);




                        GameObject Knuckleblaster;
                        GameObject Whiplash;
                        GameObject Wings;

                        //mainmenu.AddComponent<TextureOverWatch>();
                        V1 = mainmenu.transform.Find("V1").gameObject;
                        ColorBlindGet v1cb = V1.AddComponent<ColorBlindGet>();
                        v1cb.variationColor = true;
                        v1cb.variationNumber = 0;
                        v1cb.UpdateColor();

                        Wings = V1.transform.Find("Wings").gameObject;
                        ColorBlindGet WingCB = Wings.AddComponent<ColorBlindGet>();
                        WingCB.variationColor = true;
                        WingCB.variationNumber = 0;
                        WingCB.UpdateColor();
                        //Color VariantColorB = new Color(ColorBlindSettings.Instance.variationColors[0].r, ColorBlindSettings.Instance.variationColors[0].g, ColorBlindSettings.Instance.variationColors[0].b, 1f);
                        //CanvasRenderer V1Renderer = V1.GetComponentInChildren<CanvasRenderer>();
                        //V1Renderer.SetColor(VariantColorB);


                        Knuckleblaster = V1.transform.Find("Knuckleblaster").gameObject;
                        ColorBlindGet KBcb = Knuckleblaster.AddComponent<ColorBlindGet>();
                        KBcb.variationColor = true;
                        KBcb.variationNumber = 2;
                        KBcb.UpdateColor();

                        //Color VariantColorR = new Color(ColorBlindSettings.Instance.variationColors[2].r, ColorBlindSettings.Instance.variationColors[2].g, ColorBlindSettings.Instance.variationColors[2].b, 1f);
                        //CanvasRenderer KBRenderer = Knuckleblaster.GetComponentInChildren<CanvasRenderer>();

                        //KBRenderer.SetColor(VariantColorR);


                        Whiplash = V1.transform.Find("Whiplash").gameObject;
                        ColorBlindGet WLcb = Whiplash.AddComponent<ColorBlindGet>();
                        WLcb.variationColor = true;
                        WLcb.variationNumber = 1;
                        WLcb.UpdateColor();

                        //Color VariantColorG = new Color(ColorBlindSettings.Instance.variationColors[1].r, ColorBlindSettings.Instance.variationColors[1].g, ColorBlindSettings.Instance.variationColors[1].b, 1f);
                        //CanvasRenderer WLRenderer = Whiplash.GetComponentInChildren<CanvasRenderer>();
                        Button oldstart = mainmenu.GetComponentInChildren<Button>();

                        //GameObject UltraskinsMenubutton = new GameObject();
                        //Button newstart = UltraskinsMenubutton.AddComponent<Button>();
                        //Image newImage = UltraskinsMenubutton.AddComponent<Image>();
                        //UltraskinsMenubutton.transform.parent = mainmenu.transform;

                        //WLRenderer.SetColor(VariantColorG);
                    }
                    canvas = rootCanvas;
                    GameObject prefabmenu;
                    GameObject UltraskinsConfigmenu;
                    GameObject Editorpanel;

                    //Addressables.LoadAssetAsync<GameObject>("ultraskinsButton").WaitForCompletion();
                    GameObject leftside = mainmenu.transform.Find("LeftSide").gameObject;

                    Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/UltraskinsEditmenu.prefab").Completed += handle =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            prefabmenu = handle.Result;
                            UltraskinsConfigmenu = Instantiate(prefabmenu);


                            Transform[] listobjects = UltraskinsConfigmenu.GetComponentsInChildren<Transform>();
                            foreach (Transform objects in listobjects)
                            {
                                //BatonPass(objects.name);
                            }
                            Editorpanel = UltraskinsConfigmenu.transform.Find("Canvas/editor").gameObject;

                            Animator menuanimator = Editorpanel.gameObject.GetComponent<Animator>();
                            MenuManager Mman = UltraskinsConfigmenu.AddComponent<MenuManager>();
                            Editorpanel.SetActive(false);
                            BatonPass.Info("looking for content");
                            /*                            foreach (OGTexturePair objects in UltraskinsConfigmenu.GetComponent<OGSkinsManager>().RawSkins)
                                                        {
                                                            BatonPass.Debug("test: " + objects.key);
                                                        }
                                                        Dictionary<string,Texture> ogman = UltraskinsConfigmenu.GetComponent<OGSkinsManager>().OGSKINS;
                                                        foreach (KeyValuePair<string, Texture> pair in ogman)
                                                        {
                                                           BatonPass.Debug("testicle" + pair.Key.ToString());
                                                        }*/
                            EditMenuManager EMM = UltraskinsConfigmenu.GetComponent<EditMenuManager>();

                            Mman.SD = EMM.skindetails;
                            EMM.campCreator.CreateButton.onClick.AddListener(() => Mman.CreateSkinFromEditor(EMM));
                            GameObject ReturnButton = Editorpanel.transform.Find("PanelLeft/ReturnButton").gameObject;
                            GameObject ApplyButton = Editorpanel.transform.Find("PanelLeft/Apply").gameObject;
                            GameObject contentfolder = UltraskinsConfigmenu.transform.Find("Canvas/editor/PanelLeft/Scroll View/Viewport/Content/UltraButton").gameObject;
                            ObjectActivateInSequence activateanimator = contentfolder.AddComponent<ObjectActivateInSequence>();
                            Mman.GenerateButtons(contentfolder, activateanimator);

                            // Now load the ultraskins button
                            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/ultraskinsmenubutton.prefab").Completed += buttonHandle =>
                            {
                                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                                {
                                    GameObject prefab = buttonHandle.Result;
                                    GameObject instance = Instantiate(prefab, leftside.transform);
                                    instance.SetActive(true);
                                    instance.transform.localPosition = new Vector3(450, -330, 0);
                                    instance.transform.localScale = new Vector3(1.5f, 1.5f, 1);
                                    Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                                    //ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = "ULTRASKINS";
                                    // Pass UltraskinsConfigmenu to the listener
                                    ultraskinsbutton.onClick.AddListener(() => Openskineditor(mainmenu, Editorpanel, fallNoiseOn));
                                    ReturnButton.GetComponent<Button>().onClick.AddListener(() => Mman.Closeskineditor(mainmenu, Editorpanel, fallNoiseOff, menuanimator,activateanimator,contentfolder));
                                    ApplyButton.GetComponent<Button>().onClick.AddListener(() => Mman.applyskins(contentfolder));
                                    BatonPass.Debug("Successfully loaded and instantiated ultraskinsMenuButton.");
                                }
                                else
                                {
                                    BatonPass.Error("Failed to load ultraskinsMenuButton: " + buttonHandle.OperationException.Message);
                                    BatonPass.Error("Ultraskins will still work, but the button to access the config menu was disabled. CODE -\"MCREATE-MAKETHEMENU-USBUTTON-ASSET_BUNDLE_FAILED\"");
                                }
                            };
                        }
                        else
                        {
                            BatonPass.Error("Failed to load UltraskinsConfigmenu: " + handle.OperationException.Message);
                            BatonPass.Error("Ultraskins will still work, but the menu will be disabled. CODE -\"MCREATE-MAKETHEMENU-CONFIGMENU-ASSET_BUNDLE_FAILED\"");
                        }
                    };



                }
            }
            catch (Exception e)
            {
                BatonPass.Error("HEAR YEE HEAR YEE MainConfigMenu not loaded " + e.ToString());
                BatonPass.Error("Ultraskins may still work, but the MainConfigMenu will not be accessible. CODE -\"MCREATE-MAKETHEMENU-EX\"");
                
            }
            BatonPass.Debug("INIT BATON PASS: We are returning");
            return;
        }


        public static void CreateSMan()
        {
            List<TextureOverWatch> PTOW = new List<TextureOverWatch>();
            var Settingshandle = Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/Settings.prefab");
            GameObject prefab = Settingshandle.WaitForCompletion();

                if (Settingshandle.Status == AsyncOperationStatus.Succeeded)
                {
                    BatonPass.Debug("Creating the Settings Menu");
                    
                    GameObject instance = Instantiate(prefab);
                    instance.SetActive(true);
                    sMan = instance.GetComponent<SettingsManager>();
                    handInstance.ogSkinsManager = instance.GetComponent<OGSkinsManager>();
                    string usersettings = SkinEventHandler.getUserSettingsFile();
                    BatonPass.Debug("loading user settings");
                    try
                    {
                        sMan.loadUserSettings(usersettings);
                    }
                    catch (Exception ex)
                    {
                        BatonPass.Warn("Users custom settings dont exist using defaults CODE -\"MCREATE-MAKETHEMENU-USERSETTINGSFILE-CORRUPTED_OR_MISSING\"");
                    }
                    BatonPass.Debug("loading settings options");
                    try
                    {
                        sMan.LoadSettingsFromJson(USC.MODPATH + Path.DirectorySeparatorChar + USC.DEFAULTSETTINGS);
                    }
                    catch (Exception ex)
                    {
                        BatonPass.Fatal("HEAR YE, HEAR YE! Critical failure: Default Settings and their constructor are missing, UltraSkins will be unstable at best. CODE -\"MCREATE-MAKETHEMENU-SETTINGSFILE-CORRUPTED_OR_MISSING\"");
                        handInstance.BPGUI.ShowGUI("Error");
                        handInstance.BPGUI.BatonPassAnnoucement(Color.red, "Fatal Error, Default Settings and their constructor are missing, UltraSkins will be unstable at best. CODE -\"MCREATE-MAKETHEMENU-SETTINGSFILE-CORRUPTED_OR_MISSING\"");
                    }
                    BatonPass.Debug("loading settings UI");
                    sMan.BuildSettingsUI();
                    try
                    {
                        BatonPass.Debug("finding previewer");
                        GameObject PreviewFather = instance.transform.Find("previewfather").gameObject;
                        BatonPass.Debug("clearing PTOW");
                        PTOW.Clear();
                        BatonPass.Debug("adding new tows");
                        PTOW = ULTRASKINHand.HarmonyGunPatcher.AddPTOWs(PreviewFather, true);
                        BatonPass.Success("Added TOW");
                        handInstance.PtowStorage = PreviewFather.gameObject.AddComponent<TowStorage>();
                        handInstance.PtowStorage.TOWS = PTOW;
                    }
                    catch (Exception ex)
                    {
                        BatonPass.Error("Cannot make PTOW. CODE -\"MCREATE-MAKETHEMENU-SKIN_PREVIEWER-UNABLE_TO_LOAD\"");
                        BatonPass.Error(ex.Message);
                    }

                }
                else
                {
                    BatonPass.Error("Failed to load UltraskinsSettingsmenu: " + Settingshandle.OperationException.Message);
                    BatonPass.Error("Ultraskins will still work, but the menu will be disabled. CODE -\"MCREATE-MAKETHEMENU-SETTINGMENU-ASSET_BUNDLE_FAILED\"");
                }

           
        }







        public static void Openskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject fallnoiseon)
        {
            BatonPass.Debug("opened the editor");
            mainmenucanvas.SetActive(false);
            fallnoiseon.SetActive(true);
            Configmenu.SetActive(true);
            
            handInstance.settingsmanager.ShowSettingsAssets(true);
            handInstance.settingsmanager.ShowPreviewWireFrame(true);

        }
        public static void Openpausedskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject backdrop, OptionsManager controller)
        {
            GameStateManager.Instance.RegisterState(new GameState("configpause", backdrop)
            {
                cursorLock = LockMode.Unlock,
                cameraInputLock = LockMode.Lock,
                playerInputLock = LockMode.Lock
            });
            mainmenucanvas.SetActive(false);
            Cursor.visible = true;
            backdrop.SetActive(true);
            Configmenu.SetActive(true);
            handInstance.settingsmanager.ShowSettingsAssets(true);
            handInstance.settingsmanager.ShowPreviewWireFrame(true);

            //GameStateManager.Instance.RegisterState(configState);
        }


        public static void Closepauseskineditor(GameObject mainmenucanvas, GameObject Configmenu, OptionsManager controller)
        {
            controller.UnPause();
            Configmenu.SetActive(false);
            //mainmenucanvas.SetActive(true);
            //GameStateManager.Instance.PopState("pause");
        }




        public static void makethePausemenu()
        {
            BatonPass.Debug("BATON PASS: WE ARE IN MAKETHEPAUSEMENU()");

            Scene scene = SceneManager.GetActiveScene();
            BatonPass.Info("The Scene is: " + scene.name);

            GameObject canvas = null;

            try
            {

                foreach (var rootCanvas in scene.GetRootGameObjects().Where(obj => obj.name == "Canvas"))

                {

                    //GameObject UltraskinsMenubutton = new GameObject();
                    //Button newstart = UltraskinsMenubutton.AddComponent<Button>();
                    //Image newImage = UltraskinsMenubutton.AddComponent<Image>();
                    //UltraskinsMenubutton.transform.parent = mainmenu.transform;

                    //WLRenderer.SetColor(VariantColorG);
                    canvas = rootCanvas;
                    GameObject prefabmenu;
                    GameObject UltraskinsConfigmenu;
                    GameObject Pausemenu = rootCanvas.transform.Find("PauseMenu").gameObject;
                    //Addressables.LoadAssetAsync<GameObject>("ultraskinsButton").WaitForCompletion();


                    Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/UltraskinsPausedEditmenu.prefab").Completed += handle =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            prefabmenu = handle.Result;
                            UltraskinsConfigmenu = Instantiate(prefabmenu);
                            UltraskinsConfigmenu.SetActive(false); // Keep it inactive initially
                            
                            Transform[] listobjects = UltraskinsConfigmenu.GetComponentsInChildren<Transform>();
                            foreach (Transform objects in listobjects)
                            {
                                BatonPass.Debug(objects.name);
                            }
                            BatonPass.Debug("looking for content");
                            
                            GameObject backdrop = UltraskinsConfigmenu.transform.Find("Canvas/Backdrop").gameObject;
                            MenuManager Mman = backdrop.AddComponent<MenuManager>();
                            GameObject contentfolder = backdrop.transform.Find("Scroll View/Viewport/Content/UltraButton").gameObject;
                            ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
                            Mman.GenerateButtons(contentfolder);




                            BatonPass.Debug("contentfolder found");
                            HudOpenEffect activateanimator = backdrop.AddComponent<HudOpenEffect>();
                            MenuEsc men = backdrop.AddComponent<MenuEsc>();

                            BatonPass.Debug("set animator to buttons");
                            activateanimator.speed = 30;

                            BatonPass.Debug("Successfully loaded and instantiated UltraskinsConfigmenu.");
                            //GameState configState = new GameState("pause", backdrop);
                            GameObject gamecontroller = null;
                            foreach (var rootcontroller in scene.GetRootGameObjects().Where(obj => obj.name == "GameController"))
                            {
                                gamecontroller = rootcontroller;
                            }
                            ;

                            BatonPass.Debug("Successfully loaded the gamecontroller");
                            OptionsManager controller = gamecontroller.GetComponentInChildren<OptionsManager>();
                            // Now load the ultraskins button
                            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/ultraskinsmenubutton.prefab").Completed += buttonHandle =>
                            {
                                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                                {
                                    GameObject prefab = buttonHandle.Result;
                                    GameObject instance = Instantiate(prefab, Pausemenu.transform);
                                    instance.SetActive(true);
                                    instance.transform.localPosition = new Vector3(-100, -90, 0);
                                    Button ultraskinsbutton = instance.GetComponentInChildren<Button>();

                                    // Pass UltraskinsConfigmenu to the listener
                                    ultraskinsbutton.onClick.AddListener(() => Openpausedskineditor(Pausemenu, UltraskinsConfigmenu, backdrop, controller));
                                    UltraskinsConfigmenu.GetComponentInChildren<Button>().onClick.AddListener(() => Closepauseskineditor(Pausemenu, backdrop, controller));
                                    BatonPass.Debug("Successfully loaded and instantiated ultraskinsButton.");
                                }
                                else
                                {
                                    BatonPass.Error("HEAR YEE HEAR YEE UltraskinsPausedConfigmenu not loaded " + buttonHandle.OperationException.Message);
                                    BatonPass.Error("Ultraskins will still work, but the button to access the config menu was disabled. CODE -\"MCREATE-MAKETHEPAUSEMENU-USBUTTON-ASSET_BUNDLE_FAILED\"");
                                }
                            };
                        }
                        else
                        {
                            BatonPass.Error("Failed to load UltraskinsPausedConfigmenu: " + handle.OperationException.Message);
                            BatonPass.Error("Ultraskins will still work, but the menu will be disabled. CODE -\"MCREATE-MAKETHEPAUSEMENU-CONFIGMENU-ASSET_BUNDLE_FAILED\"");
                        }
                    };




                }
            }
            catch (Exception e)
            {
                BatonPass.Error("HEAR YEE HEAR YEE PausedConfigMenu not loaded " + e.ToString());
                BatonPass.Error("Ultraskins may still work, but the PausedConfigMenu will not be accessible. CODE -\"MCREATE-MAKETHEPAUSEMENU-EX\"");
            }
            BatonPass.Debug("INIT BATON PASS: We are returning");
            return;
        }
       
    }
}
