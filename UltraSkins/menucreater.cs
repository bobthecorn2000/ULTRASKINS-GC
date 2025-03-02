using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

using HarmonyLib;
using NewBlood;
using Unity.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Reflection;
using static UltraSkins.ULTRASKINHand;

using BepInEx.Logging;
using static UltraSkins.SkinEventHandler;

using System.Net.NetworkInformation;
using HarmonyLib.Tools;
using static MonoMod.RuntimeDetour.Platforms.DetourNativeMonoPosixPlatform;
using static UnityEngine.ParticleSystem.PlaybackState;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;




//using UnityEngine.UIElements;


namespace UltraSkins
{
    internal class menucreater : MonoBehaviour
    {
        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
        static string skinfolderdir = Path.Combine(appDataPath, AppDataLoc);
        public static string folderupdater;
        static GameObject batonpassGUIInst = null;
        public static void makethemenu()
        {
            Debug.Log("BATON PASS: WE ARE IN MAKETHEMENU()");

            Scene scene = SceneManager.GetActiveScene();
            Debug.Log("The Scene is: " + scene.name);

            GameObject canvas = null;
            GameObject mainmenu;
            try
            {

                foreach (var rootCanvas in scene.GetRootGameObjects().Where(obj => obj.name == "Canvas"))

                {
                    Debug.Log("entered a foreach");
                    mainmenu = rootCanvas.transform.Find("Main Menu (1)").gameObject;
                    Debug.Log("finished search");
                    Debug.Log("HEARYEE HEAR YEE TRANSFORM IS " + mainmenu.ToString());
                    GameObject V1;

                    Debug.Log(ColorBlindSettings.Instance.variationColors[2].r + " " + ColorBlindSettings.Instance.variationColors[2].g + " " + ColorBlindSettings.Instance.variationColors[2].b);




                    GameObject Knuckleblaster;
                    GameObject Whiplash;
                    GameObject Wings;

                    //mainmenu.AddComponent<TextureOverWatch>();
                    V1 = mainmenu.transform.Find("V1").gameObject;
                    ColorBlindGet v1cb = V1.AddComponent<ColorBlindGet>();
                    v1cb.variationColor = true;
                    v1cb.variationNumber = 0;
                    v1cb.UpdateColor();

                    Color VariantColorB = new Color(ColorBlindSettings.Instance.variationColors[0].r, ColorBlindSettings.Instance.variationColors[0].g, ColorBlindSettings.Instance.variationColors[0].b, 1f);
                    CanvasRenderer V1Renderer = V1.GetComponentInChildren<CanvasRenderer>();
                    //V1Renderer.SetColor(VariantColorB);


                    Knuckleblaster = V1.transform.Find("Knuckleblaster").gameObject;
                    ColorBlindGet KBcb = Knuckleblaster.AddComponent<ColorBlindGet>();
                    KBcb.variationColor = true;
                    KBcb.variationNumber = 2;
                    KBcb.UpdateColor();

                    Color VariantColorR = new Color(ColorBlindSettings.Instance.variationColors[2].r, ColorBlindSettings.Instance.variationColors[2].g, ColorBlindSettings.Instance.variationColors[2].b, 1f);
                    CanvasRenderer KBRenderer = Knuckleblaster.GetComponentInChildren<CanvasRenderer>();

                    //KBRenderer.SetColor(VariantColorR);


                    Whiplash = V1.transform.Find("Whiplash").gameObject;
                    ColorBlindGet WLcb = Whiplash.AddComponent<ColorBlindGet>();
                    WLcb.variationColor = true;
                    WLcb.variationNumber = 1;
                    WLcb.UpdateColor();

                    Color VariantColorG = new Color(ColorBlindSettings.Instance.variationColors[1].r, ColorBlindSettings.Instance.variationColors[1].g, ColorBlindSettings.Instance.variationColors[1].b, 1f);
                    CanvasRenderer WLRenderer = Whiplash.GetComponentInChildren<CanvasRenderer>();
                    Button oldstart = mainmenu.GetComponentInChildren<Button>();

                    //GameObject UltraskinsMenubutton = new GameObject();
                    //Button newstart = UltraskinsMenubutton.AddComponent<Button>();
                    //Image newImage = UltraskinsMenubutton.AddComponent<Image>();
                    //UltraskinsMenubutton.transform.parent = mainmenu.transform;

                    //WLRenderer.SetColor(VariantColorG);
                    canvas = rootCanvas;
                    GameObject prefabmenu;
                    GameObject UltraskinsConfigmenu;
                    GameObject Editorpanel;
                    Addressables.LoadAssetAsync<GameObject>("Assets/BatonpassGUI.prefab").Completed += buttonHandle =>
                    {
                        if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            GameObject prefab = buttonHandle.Result;
                            batonpassGUIInst = Instantiate(prefab, canvas.transform);
                            batonpassGUIInst.SetActive(false);
                            batonpassGUIInst.transform.localPosition = new Vector3(0, 0, 0);


                            // Pass UltraskinsConfigmenu to the listener

                            BatonPass("Successfully loaded and instantiated Baton Pass GUI.");
                        }
                        else
                        {
                            BatonPass("Failed to load Baton Pass GUI " + buttonHandle.OperationException);
                        }
                    };
                    //Addressables.LoadAssetAsync<GameObject>("ultraskinsButton").WaitForCompletion();
                    GameObject leftside = mainmenu.transform.Find("LeftSide").gameObject;

                    Addressables.LoadAssetAsync<GameObject>("Assets/UltraskinsEditmenu.prefab").Completed += handle =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            prefabmenu = handle.Result;
                            UltraskinsConfigmenu = Instantiate(prefabmenu);
                            

                            Transform[] listobjects = UltraskinsConfigmenu.GetComponentsInChildren<Transform>();
                            foreach (Transform objects in listobjects)
                            {
                                BatonPass(objects.name);
                            }
                            Editorpanel = UltraskinsConfigmenu.transform.Find("Canvas/editor").gameObject;
                            Editorpanel.SetActive(false);
                            BatonPass("looking for content");
                            string[] subfolders = Directory.GetDirectories(skinfolderdir);
                            GameObject contentfolder = UltraskinsConfigmenu.transform.Find("Canvas/editor/Scroll View/Viewport/Content").gameObject;
                            List<GameObject> loadedButtons = new List<GameObject>();
                            int buttonsToLoad = subfolders.Length;
                            int buttonsLoaded = 0;

                            foreach (string subfolder in subfolders)
                            {
                                string folder = Path.GetFileName(subfolder);
                                BatonPass("SubFolder: " + folder);
                                folderupdater = folder;
                                ULTRASKINHand handInstance = new ULTRASKINHand();

                                Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsButton.prefab").Completed += buttonHandle =>
                                {
                                    if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                                    {
                                        GameObject prefab = buttonHandle.Result;
                                        GameObject instance = Instantiate(prefab, contentfolder.transform);
                                        instance.SetActive(true);

                                        Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                                        ultraskinsbutton.onClick.AddListener(() => handInstance.refreshskins(folder));

                                        loadedButtons.Add(instance); // Add button to list

                                        BatonPass("Successfully loaded and instantiated ultraskinsButton.");
                                    }
                                    else
                                    {
                                        BatonPass("Failed to load ultraskinsButton: " + buttonHandle.OperationException);
                                    }

                                    // Check if all buttons are loaded
                                    buttonsLoaded++;
                                    if (buttonsLoaded == buttonsToLoad)
                                    {
                                        BatonPass("All buttons loaded, setting up activation sequence.");
                                        ObjectActivateInSequence activateanimator = contentfolder.AddComponent<ObjectActivateInSequence>();
                                        activateanimator.objectsToActivate = loadedButtons.ToArray();
                                        activateanimator.delay = 0.05f;
                                        BatonPass("Successfully set up ObjectActivateInSequence.");
                                    }
                                };
                            }



                            /*                            BatonPass("searching for Viewport");
                                                        contentfolder = UltraskinsConfigmenu.transform.Find("Viewport").gameObject;
                                                        BatonPass("searching for content");
                                                        contentfolder = contentfolder.transform.Find("Content").gameObject;*/


                            // Now load the ultraskins button
                            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsmenubutton.prefab").Completed += buttonHandle =>
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
                                    ultraskinsbutton.onClick.AddListener(() => Openskineditor(mainmenu, Editorpanel));
                                    Editorpanel.GetComponentInChildren<Button>().onClick.AddListener(() => Closeskineditor(mainmenu, Editorpanel));
                                    BatonPass("Successfully loaded and instantiated ultraskinsButton.");
                                }
                                else
                                {
                                    BatonPass("Failed to load ultraskinsButton: " + buttonHandle.OperationException);
                                }
                            };
                        }
                        else
                        {
                            BatonPass("Failed to load UltraskinsConfigmenu: " + handle.OperationException);
                        }
                    };




                }
            }
            catch (Exception e)
            {
                Debug.Log("HEAR YEE HEAR YEE mainmenu not loaded " + e.ToString());
            }
            Debug.Log("INIT BATON PASS: We are returning");
            return;
        }

        public static void Openskineditor(GameObject mainmenucanvas, GameObject Configmenu)
        {
            mainmenucanvas.SetActive(false);
            
            Configmenu.SetActive(true);

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

            //GameStateManager.Instance.RegisterState(configState);
        }
        public static void Closeskineditor(GameObject mainmenucanvas, GameObject Configmenu)
        {
            mainmenucanvas.SetActive(true);
            Configmenu.SetActive(false);
            
        }
        public static void Closepauseskineditor(GameObject mainmenucanvas, GameObject Configmenu, OptionsManager controller)
        {
            controller.UnPause();
            Configmenu.SetActive(false);
            //mainmenucanvas.SetActive(true);
            //GameStateManager.Instance.PopState("pause");
        }
        
        public static void BatonPassAnnoucement(Color bordercolor, string message)
        {
            GameObject messagebox = batonpassGUIInst.transform.Find("MessageBox").gameObject;
            GameObject messageboxOutline = batonpassGUIInst.transform.Find("MessageBox/MessageOutline").gameObject;
            batonpassGUIInst.SetActive(false);
            messagebox.SetActive(true);
            batonpassGUIInst.GetComponentInChildren<TextMeshProUGUI>().text = message;
            messageboxOutline.GetComponentInChildren<Image>().color = bordercolor;


            batonpassGUIInst.SetActive(true);
        }

        
            public static void makethePausemenu()
        {
            Debug.Log("BATON PASS: WE ARE IN MAKETHEPAUSEMENU()");

            Scene scene = SceneManager.GetActiveScene();
            Debug.Log("The Scene is: " + scene.name);

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
                    Addressables.LoadAssetAsync<GameObject>("Assets/BatonpassGUI.prefab").Completed += buttonHandle =>
                    {
                        if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            GameObject prefab = buttonHandle.Result;
                            batonpassGUIInst = Instantiate(prefab, canvas.transform);
                            batonpassGUIInst.SetActive(false);
                            batonpassGUIInst.transform.localPosition = new Vector3(0, 0, 0);
                            
                           
                            // Pass UltraskinsConfigmenu to the listener
                            
                            BatonPass("Successfully loaded and instantiated Baton Pass GUI.");
                        }
                        else
                        {
                            BatonPass("Failed to load Baton Pass GUI " + buttonHandle.OperationException);
                        }
                    };

                    Addressables.LoadAssetAsync<GameObject>("Assets/UltraskinsPausedEditmenu.prefab").Completed += handle =>
                    {
                        if (handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            prefabmenu = handle.Result;
                            UltraskinsConfigmenu = Instantiate(prefabmenu);
                            UltraskinsConfigmenu.SetActive(false); // Keep it inactive initially

                            Transform[] listobjects = UltraskinsConfigmenu.GetComponentsInChildren<Transform>();
                            foreach (Transform objects in listobjects)
                            {
                                BatonPass(objects.name);
                            }
                            BatonPass("looking for content");
                            string[] subfolders = Directory.GetDirectories(skinfolderdir);
                            GameObject backdrop = UltraskinsConfigmenu.transform.Find("Canvas/Backdrop").gameObject;
                            GameObject contentfolder = backdrop.transform.Find("Scroll View/Viewport/Content").gameObject;
                            foreach (string subfolder in subfolders)
                            {

                                string folder = Path.GetFileName(subfolder);
                                BatonPass("SubFolder: " + folder);
                                folderupdater = folder;
                                ULTRASKINHand handInstance = new ULTRASKINHand();
                                Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsButton.prefab").Completed += buttonHandle =>
                                {
                                    if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                                    {
                                        GameObject prefab = buttonHandle.Result;
                                        GameObject instance = Instantiate(prefab, contentfolder.transform);
                                        instance.SetActive(true);

                                        Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                                        ultraskinsbutton.GetComponentInChildren<TextMeshProUGUI>().text = folder;
                                        // Pass UltraskinsConfigmenu to the listener

                                        ultraskinsbutton.GetComponentInChildren<Button>().onClick.AddListener(() => handInstance.refreshskins(folder));
                                        BatonPass("Successfully loaded and instantiated ultraskinsButton.");
                                    }
                                    else
                                    {
                                        BatonPass("Failed to load ultraskinsButton: " + buttonHandle.OperationException);
                                    }
                                };
                            }



                            /*                            BatonPass("searching for Viewport");
                                                        contentfolder = UltraskinsConfigmenu.transform.Find("Viewport").gameObject;
                                                        BatonPass("searching for content");
                                                        contentfolder = contentfolder.transform.Find("Content").gameObject;*/
                            BatonPass("contentfolder found");
                            HudOpenEffect activateanimator = backdrop.AddComponent<HudOpenEffect>();
                            MenuEsc men = backdrop.AddComponent<MenuEsc>();
                           
                            BatonPass("set animator to buttons");
                            activateanimator.speed = 30;

                            BatonPass("Successfully loaded and instantiated UltraskinsConfigmenu.");
                            //GameState configState = new GameState("pause", backdrop);
                            GameObject gamecontroller = null;
                            foreach (var rootcontroller in scene.GetRootGameObjects().Where(obj => obj.name == "GameController"))
                            {
                                gamecontroller = rootcontroller;
                            }
                            ;

                            BatonPass("Successfully loaded the gamecontroller");
                            OptionsManager controller = gamecontroller.GetComponentInChildren<OptionsManager>();
                            // Now load the ultraskins button
                            Addressables.LoadAssetAsync<GameObject>("Assets/ultraskinsmenubutton.prefab").Completed += buttonHandle =>
                            {
                                if (buttonHandle.Status == AsyncOperationStatus.Succeeded)
                                {
                                    GameObject prefab = buttonHandle.Result;
                                    GameObject instance = Instantiate(prefab, Pausemenu.transform);
                                    instance.SetActive(true);
                                    instance.transform.localPosition = new Vector3(-100, -90, 0);
                                    Button ultraskinsbutton = instance.GetComponentInChildren<Button>();
                                    
                                    // Pass UltraskinsConfigmenu to the listener
                                    ultraskinsbutton.onClick.AddListener(() => Openpausedskineditor(Pausemenu, UltraskinsConfigmenu,backdrop,controller));
                                    UltraskinsConfigmenu.GetComponentInChildren<Button>().onClick.AddListener(() => Closepauseskineditor(Pausemenu, backdrop, controller));
                                    BatonPass("Successfully loaded and instantiated ultraskinsButton.");
                                }
                                else
                                {
                                    BatonPass("Failed to load ultraskinsButton: " + buttonHandle.OperationException);
                                }
                            };
                        }
                        else
                        {
                            BatonPass("Failed to load UltraskinsConfigmenu: " + handle.OperationException);
                        }
                    };




                }
            }
            catch (Exception e)
            {
                Debug.Log("HEAR YEE HEAR YEE mainmenu not loaded " + e.ToString());
            }
            Debug.Log("INIT BATON PASS: We are returning");
            return;
        }
    }
}
