using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UltraSkins.ULTRASKINHand;





//using UnityEngine.UIElements;


namespace UltraSkins
{
    internal class MenuCreator : MonoBehaviour
    {

        static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        static string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
        static string skinfolderdir = Path.Combine(appDataPath, AppDataLoc);
        public static string folderupdater;
        static GameObject batonpassGUIInst = null;

        public static void makethemenu()
        {
            BatonPass("BATON PASS: WE ARE IN MAKETHEMENU()");

            Scene scene = SceneManager.GetActiveScene();
            BatonPass("The Scene is: " + scene.name);

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
                    BatonPass("entered a foreach");
                    mainmenu = rootCanvas.transform.Find("Main Menu (1)").gameObject;

                    BatonPass("finished search");
                    BatonPass("HEARYEE HEAR YEE TRANSFORM IS " + mainmenu.ToString());
                    GameObject V1;

                    BatonPass(ColorBlindSettings.Instance.variationColors[2].r + " " + ColorBlindSettings.Instance.variationColors[2].g + " " + ColorBlindSettings.Instance.variationColors[2].b);




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
                    canvas = rootCanvas;
                    GameObject prefabmenu;
                    GameObject UltraskinsConfigmenu;
                    GameObject Editorpanel;

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
                                //BatonPass(objects.name);
                            }
                            Editorpanel = UltraskinsConfigmenu.transform.Find("Canvas/editor").gameObject;

                            Animator menuanimator = Editorpanel.gameObject.GetComponent<Animator>();
                            MenuManager Mman = Editorpanel.AddComponent<MenuManager>();
                            Editorpanel.SetActive(false);
                            BatonPass("looking for content");
                            GameObject contentfolder = UltraskinsConfigmenu.transform.Find("Canvas/editor/Scroll View/Viewport/Content").gameObject;
                            ObjectActivateInSequence activateanimator = contentfolder.AddComponent<ObjectActivateInSequence>();
                            Mman.GenerateButtons(skinfolderdir, contentfolder, activateanimator);

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
                                    ultraskinsbutton.onClick.AddListener(() => Openskineditor(mainmenu, Editorpanel, fallNoiseOn));
                                    Editorpanel.GetComponentInChildren<Button>().onClick.AddListener(() => Mman.Closeskineditor(mainmenu, Editorpanel, fallNoiseOff, menuanimator));
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

        public static void Openskineditor(GameObject mainmenucanvas, GameObject Configmenu, GameObject fallnoiseon)
        {
            mainmenucanvas.SetActive(false);
            fallnoiseon.SetActive(true);
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


        public static void Closepauseskineditor(GameObject mainmenucanvas, GameObject Configmenu, OptionsManager controller)
        {
            controller.UnPause();
            Configmenu.SetActive(false);
            //mainmenucanvas.SetActive(true);
            //GameStateManager.Instance.PopState("pause");
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
                            MenuManager Mman = backdrop.AddComponent<MenuManager>();
                            GameObject contentfolder = backdrop.transform.Find("Scroll View/Viewport/Content").gameObject;
                            ULTRASKINHand handInstance = ULTRASKINHand.HandInstance;
                            Mman.GenerateButtons(skinfolderdir, contentfolder);




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
                                    ultraskinsbutton.onClick.AddListener(() => Openpausedskineditor(Pausemenu, UltraskinsConfigmenu, backdrop, controller));
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
