using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BatonPassLogger.GUI;
using UltraSkins.Utils;
using UltraSkins.UI;
using UltraSkins.Fractal;
using StringSerializer = UltraSkins.Utils.SkinEventHandler.StringSerializer;
using BatonPassLogger;
using System.Xml.Serialization;
using static UltraSkins.ULTRASKINHand.HoldEm;
using UltraSkins.API;
using static UltraSkins.API.USAPI;
using static UltraSkins.ULTRASKINHand;
using UltraSkins.Prism;
using UnityEngine.ResourceManagement.AsyncOperations;


//using UltraSkins.Prism;







//using UnityEngine.UIElements;






namespace UltraSkins
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]


    public partial class ULTRASKINHand : BaseUnityPlugin
    {



        public const string PLUGIN_NAME = "UltraSkins";
        public const string PLUGIN_GUID = "ultrakill.UltraSkins.bobthecorn";
        public const string PLUGIN_VERSION = USC.VERSION;
        private string modFolderPath;
        public bool loadTextureLock = false;
        public BPGUIManager BPGUI;

        string skinfolderdir;

        public string[] filepathArray;


        private List<Sprite> _default;
        private List<Sprite> _edited;
        private bool firstmenu = true;

        
        public string folderupdater;
        public static Dictionary<string, Texture> autoSwapCache = new Dictionary<string, Texture>();
        public  OGSkinsManager ogSkinsManager;
        public Dictionary<string,string> MaterialNames = new Dictionary<string,string>();
        public string[] directories;
        public string serializedSet = "";
        public bool swapped = false;
        public UltraSkins.UI.SettingsManager settingsmanager;
        public Legacy.TowStorage PtowStorage;
        public Dictionary<string, Texture2D> IconCache = new Dictionary<string, Texture2D>();
        public Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
        public CompatLayer compatlayer = new CompatLayer();
        public bool UsingCompatLayer = false;
        Harmony UKSHarmony;
        
        public static ULTRASKINHand HandInstance { get; private set; }

        //public void Start(SkinEventHandler skinEventHandler)
        //{
        //    string modFolderPath = skinEventHandler.GetModFolderPath();

        //    LoadTextures(modFolderPath);

        //}
        public bool ThunderStoreMode = false;
        public string[] ThunderProfInfo = null;
        public bool OldSaveDataFound = false;
        public static ManualLogSource BatonPassLogger = new ManualLogSource("BatonPass");
        private void Awake()
        {
            
           
            BepInEx.Logging.Logger.Sources.Add(BatonPassLogger);
            BatonPass.RegisterTerminal(new BepInExTerminal(BatonPassLogger));
            BatonPass.RegisterTerminal(new UKPlog("UltraSkins"));
            if (HandInstance == null)
            {
                HandInstance = this;
            }
            else
            {
                Destroy(this);
            }
            BatonPass.Info("Attempting to start");


            

            BatonPass.Debug("Plugin path set to " + USC.MODPATH);
            
                                                             
            BatonPass.Debug("NAVI: APPDATAPATH AT: " + USC.GCDIR);
            BatonPass.Debug("Starting addressables");

            Assembly.Load(File.ReadAllBytes(Path.Combine(USC.MODPATH, "usUI.dll")));
            BatonPass.Debug("Looking for the Config");
            var catalog = Addressables.LoadContentCatalogAsync(Path.Combine(USC.MODPATH, "catalog.json"), true).WaitForCompletion();


            //BatonPass.Info($"Registered Addressable Keys:\n{string.Join("\n", keys)}");


            BatonPass.Info("Finishing setup");

            SceneManager.activeSceneChanged += SceneManagerOnsceneLoaded;
            BatonPass.Info("Scenemanager Created");
            USAPI.OnTexLoadFinished += RANKTITLESWAPPER.MakeTheStyleRank;
            BatonPass.Info("Subscribing to the API");
            // The GUID of the configurator must not be changed as it is used to locate the config file path
            BatonPass.Debug("INIT BATON PASS: ONMODLOADED()");
            OnModLoaded();

        }
        void Start()
        {
            BatonPass.Info("Starting BatonPassGUI");
            if (BPGUIManager.BPGUIinstance == null)
            {
                GameObject managerObject = new GameObject("BPGUIManager");
                managerObject.AddComponent<BPGUIManager>();
                BPGUI = BPGUIManager.BPGUIinstance;
            }
            UsingCompatLayer = compatlayer.PluginConfigCompatBoot();
            MenuCreator.CreateSMan();
            if (Prism.PrismManager.Instance == null)
            {
                GameObject PrismManagerObject = new GameObject("Prism");
                PrismManagerObject.AddComponent<Prism.PrismManager>();
            }
            
        }
        public void OnModLoaded()
        {
            //Harmony.DEBUG = true;
            // HarmonyFileLog.Enabled = true;
            
            UKSHarmony = new Harmony("Gcorn.UltraSkins");
            UKSHarmony.PatchAll(typeof(HarmonyGunPatcher));
            UKSHarmony.PatchAll(typeof(HarmonyProjectilePatcher));
            UKSHarmony.PatchAll();
            BatonPass.Success("BATON PASS: Welcome To Ultraskins, We are on the ONMODLOADED() STEP");


            try
            {
                BatonPass.Debug("Creating the SkinEvent Handler");
                ThunderProfInfo = SkinEventHandler.GetThunderstoreProfileName();
                if (ThunderProfInfo != null)
                {
                    ThunderStoreMode = true;
                }
                SkinEventHandler skinEventHandler = new SkinEventHandler();
                BatonPass.Debug("INIT BATON PASS: GETMODFOLDERPATH()");
                string[] modFolderPath = skinEventHandler.GetModFolderPath();
                BatonPass.Debug("BATON PASS: WELCOME BACK TO ONMODLOADED() WE RECIEVED " + modFolderPath);
                
                HandInstance.MaterialNames.Add("Swapped_WL_GreenArm (Instance)(Clone) (Instance)", "T_GreenArm");
                HandInstance.MaterialNames.Add("Swapped_FB_FeedbackerLit (Instance)(Clone) (Instance)", "T_Feedbacker");
                HandInstance.MaterialNames.Add("Swapped_KB_RedArmLit (Instance)(Clone) (Instance)", "v2_armtex");
                HandInstance.MaterialNames.Add("Swapped_arm_MainArmLit (Instance)(Clone) (Instance)", "T_MainArm");

            }
            catch (ArgumentOutOfRangeException ex)
            {
                BatonPass.Warn("Something didnt return correctly Error -\"USHAND-ONMODLOADED-AOOR\"" + ex.Message);
            }
            catch (Exception ex)
            {
                BatonPass.Error("ULTRASKINS HAS FAILED TO INIT Error -\"USHAND-ONMODLOADED-EX\"" + ex.Message);
                BatonPass.Error("If you don't see the button to open the menu. try setting HideManagerGameobject to true in your bepinex settings");


            }
            //refreshskins();


        }

        public void RefreshSkins(string[] clickedButtons)
        {
            BatonPass.Debug("BATON PASS: WE ARE IN REFRESHSKINS()");

            StringSerializer serializer = new StringSerializer();
            BatonPass.Debug("Created The Serializer");
            BatonPass.Debug("looking for the dll, appdata paths and merging the directory strings");

            string dir = SkinEventHandler.getDataFile();

            List<string> filepath = new List<string>();
            foreach (string clickedButton in clickedButtons)
            {
                filepath.Add(Path.Combine(clickedButton));
                BatonPass.Debug("added " + clickedButton);
            }
            filepathArray = filepath.ToArray();
            foreach (string thing in filepathArray)
            {
                BatonPass.Debug(thing);
            }

            BatonPass.Debug("folderis: " + filepath);
            serializer.SerializeStringToFile(filepathArray, Path.Combine(dir));
            BatonPass.Success("Saved to data.USGC");
            BatonPass.Debug("INIT BATON PASS: RELOADTEXTURES(TRUE," + filepath + ")");
            
            ReloadTextures(filepathArray,false);
            
            BatonPass.Debug("BATON PASS: WELCOME BACK TO REFRESHSKINS()");
            BatonPass.Info("Closing panel");

            //LoadTextures(filepath);

        }
        void RefreshSkins()
        {

            
            StringSerializer serializer = new StringSerializer();
            BatonPass.Debug("Created The Serializer");
            BatonPass.Debug("looking for the dll, appdata paths and merging the directory strings");
            string save = SkinEventHandler.getDataFile();



            filepathArray = serializer.DeserializeStringFromFile(save);
            BatonPass.Info("Read data.USGC from " + filepathArray);
            ReloadTextures(filepathArray,true);

            //LoadTextures(filepath);

        }
        [HarmonyPatch]
        public class HarmonyUIPatcher
        {
            [HarmonyPatch(typeof(StyleHUD), "Start")]
            [HarmonyPostfix]
            static void StyleHudStartPost(StyleHUD __instance)
            {
                BatonPass.Debug("hooking to the style meter");
                RANKTITLESWAPPER.SwapTheStyleRank(__instance);
            }



            [HarmonyPatch(typeof(ShopZone), "Start")]
            [HarmonyPostfix]
            public static void Start(ShopZone __instance)
            {
                //BatonPass.Debug("Started a ShopZone " + __instance.gameObject.name + " " + __instance.GetInstanceID());
                Transform trans = FindDeepChildByPathIncludeInactive(__instance.transform, "Canvas/Background/Main Panel/Weapons");

                Transform buttonloc = FindDeepChildByPathIncludeInactive(trans, "Arm Window/Variation Screen/Variations");
                BatonPass.Debug("found both transforms");
                if (trans != null)
                {
                    Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/SmileOSbutton.prefab").Completed += handle =>
                    {
                        BatonPass.Debug("button callback");
                        GameObject ColorButton = Instantiate(handle.Result, buttonloc);



                        //List<TextureOverWatch> SHOPTOWS = AddTOWs(__instance.gameObject, true, true, false, true);
                        Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/PrismUI.prefab").Completed += prismhandle =>
                        {
                            BatonPass.Debug("PrismCallBack");
                            GameObject prefabmenu = prismhandle.Result;



                            GameObject ColorMenu = Instantiate(prefabmenu, trans);
                            ColorButton.GetComponent<Button>().onClick.AddListener(() => ColorMenu.SetActive(true));
                            ColorMenu.SetActive(false);
                        };
                    };
                }

                //ReloadTextureOverWatch(TOWS);
            }
        }
        [HarmonyPatch]
        public class HarmonyGunPatcher
        {

            [HarmonyPatch(typeof(ColorBlindSettings), "UpdateWeaponColors")]
            [HarmonyPostfix]
            public static void UWCPost()
            {
                BroadcastDynEmSwap();
            }


            [HarmonyPatch(typeof(RocketLauncher), "Start")]
            [HarmonyPostfix]
            public static void RockSetup(RocketLauncher __instance)
            {
                if (__instance.rocket)
                {
                    GameObject rocket = __instance.rocket;
                    if (!rocket.GetComponent<GrenadeFractal>() && rocket.TryGetComponent<Grenade>(out Grenade FractGrenade))
                    {

                        GrenadeFractal fract = rocket.AddComponent<GrenadeFractal>();
                        fract.Init(FractGrenade);
                        fract.PrepareSwap();
                    }
                }

            }
            [HarmonyPatch(typeof(Shotgun), "Start")]
            [HarmonyPostfix]
            public static void GrenadePost(Shotgun __instance)
            {
                if (__instance.grenade)
                {
                    GameObject grenade = __instance.grenade;
                    if (!grenade.GetComponent<GrenadeFractal>() && grenade.TryGetComponent<Grenade>(out Grenade FractGrenade))
                    {

                        GrenadeFractal fract = grenade.AddComponent<GrenadeFractal>();
                        fract.Init(FractGrenade);
                        fract.PrepareSwap();
                    }
                }

            }

            [HarmonyPatch(typeof(Revolver), "Start")]
            [HarmonyPostfix]
            public static void RevSetup(Revolver __instance)
            {
                if (__instance.coin)
                {

                    if (!__instance.coin.GetComponent<Fractal.CoinFractal>() && __instance.coin.TryGetComponent<Coin>(out Coin FractCoin))
                    {

                        CoinFractal fract = __instance.coin.AddComponent<Fractal.CoinFractal>();
                        fract.Init(FractCoin);
                        fract.PrepareSwap();
                    }
                }

                ArmFractal igf = null;
                bool dontInit = false;
                if (__instance.altVersion)
                {
                    GameObject go = __instance.transform.Find("Revolver_Rerigged_Alternate/RightArm").gameObject;
                    if (go)
                    {

                    
                    if (!go.GetComponent<ArmFractal>())
                    {
                         
                        igf = go.AddComponent<ArmFractal>();
                    }
                        else
                        {
                            dontInit = true;
                        }
                    }
                }
                else
                {
                    GameObject go = __instance.transform.Find("Revolver_Rerigged_Standard/RightArm").gameObject;
                    if (go)
                    {
                        if (!go.GetComponent<ArmFractal>())
                        {
                            igf = go.AddComponent<ArmFractal>();
                        }
                        else
                        {
                            dontInit = true;
                        }

                    }
                }
                if (igf != null && !dontInit)
                {
                    
                    igf.Init(__instance);
                    igf.PrepareSwap();
                }
                

            }

            [HarmonyPatch(typeof(ShotgunHammer), "Awake")]
            [HarmonyPrefix]
            public static void EarlyHammerSetup(ShotgunHammer __instance)
            {
                
                if (!__instance.GetComponent<Fractal.ChainsawFractal>())
                {
                    if (__instance.variation == 2)
                    {
                        ChainsawFractal fract = __instance.gameObject.AddComponent<Fractal.ChainsawFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }

                }

            }
            [HarmonyPatch(typeof(Shotgun), "Awake")]
            [HarmonyPrefix]
            public static void EarlyShotgunSetup(Shotgun __instance)
            {

                if (!__instance.GetComponent<Fractal.ChainsawFractal>())
                {
                    if (__instance.variation == 2)
                    {
                        ChainsawFractal fract = __instance.gameObject.AddComponent<Fractal.ChainsawFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }

                }

            }



            [HarmonyPatch(typeof(ShotgunHammer), "Awake")]
            [HarmonyPostfix]
            public static void HammerSetup(ShotgunHammer __instance)
            {

                    if (!__instance.GetComponent<Fractal.DialFractal>())
                    {

                        DialFractal fract = __instance.gameObject.AddComponent<Fractal.DialFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }
                
            }


            private static readonly AccessTools.FieldRef<Punch, SkinnedMeshRenderer> smrRef = AccessTools.FieldRefAccess<Punch, SkinnedMeshRenderer>("smr");
            [HarmonyPatch(typeof(Punch), "Start")]
            [HarmonyPostfix]
            public static void PunchArm(Punch __instance)
            {
                try
                {
                    GameObject model = smrRef(__instance).gameObject;
                    if (!model.GetComponent<ArmFractal>())
                    {

                        ArmFractal fract = model.gameObject.AddComponent<ArmFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }
                    if (!model.GetComponent<PrismColorGetter>())
                    {
                        //todo add prismcolorgetter
                    }
                }
                catch (Exception Ex)
                {
                    BatonPass.Error("Punch Patch failed to load. CODE - \"USHAND-HGP-PUNCH-EX\"");
                    BatonPass.Error(Ex.ToString());
                }
            }

            private static readonly AccessTools.FieldRef<HookArm, LineRenderer> lrRef = AccessTools.FieldRefAccess<HookArm, LineRenderer>("lr");
            private static readonly AccessTools.FieldRef<HookArm, GameObject> hookArmModelRef = AccessTools.FieldRefAccess<HookArm, GameObject>("model");
            

            [HarmonyPatch(typeof(HookArm), "Start")]
            [HarmonyPostfix]
            public static void HookArm(HookArm __instance)
            {
                try
                {
                    GameObject hookmodel = __instance.hookModel;
                    if (!hookmodel.GetComponent<ArmFractal>())
                    {

                        ArmFractal fract = hookmodel.gameObject.AddComponent<ArmFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }
                    if (!hookmodel.GetComponent<PrismColorGetter>())
                    {
                        //todo add prismcolorgetter
                    }
                }
                catch(Exception Ex)
                {
                    BatonPass.Error("HookArm Patch HOOK failed to load. CODE - \"USHAND-HGP-HOOKARM-HOOK-EX\"");
                    BatonPass.Error(Ex.ToString());
                }

                try
                {
                    GameObject parent = hookArmModelRef(__instance);
                    GameObject model = parent.transform.Find("Arm").gameObject;
                    if (!model.GetComponent<ArmFractal>())
                    {

                        ArmFractal fract = model.AddComponent<ArmFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }
                    if (!model.GetComponent<PrismColorGetter>())
                    {
                        //todo add prismcolorgetter
                    }
                }
                catch (Exception Ex)
                {
                    BatonPass.Error("HookArm ARM Patch failed to load. CODE - \"USHAND-HGP-HOOKARM-ARM-EX\"");
                    BatonPass.Error(Ex.ToString());
                }

            }
            [HarmonyPatch(typeof(GunColorGetter), "Awake")]
            [HarmonyPrefix]
            public static void StartAwake(GunColorGetter __instance)
            {
                BatonPass.Debug(__instance.name + " I HAVE AWOKEN");
                if (!__instance.GetComponent<GCGFractal>())
                {
                    GCGFractal fract = __instance.gameObject.AddComponent<GCGFractal>();
                    fract.Init(__instance);
                    fract.PrepareSwap();
                }

            }

            
        }
        [HarmonyPatch]
        public class HarmonyProjectilePatcher
        {
            [HarmonyPatch(typeof(Nail), "Start")]
            [HarmonyPostfix]
            public static void NailPost(Nail __instance)
            {
                if (__instance.sawblade)
                {
                    if (!__instance.GetComponent<BaseFractal>())
                    {

                        BaseFractal fract = __instance.gameObject.AddComponent<BaseFractal>();
                        fract.Init(__instance);
                        fract.PrepareSwap();
                    }
                }


            }

            [HarmonyPatch(typeof(Magnet), "Start")]
            [HarmonyPostfix]
            public static void MagnetPost(Magnet __instance)
            {
                if (!__instance.GetComponent<BaseFractal>() )
                {

                    BaseFractal fract = __instance.gameObject.AddComponent<BaseFractal>();
                    fract.Init(__instance);
                    fract.PrepareSwap();
                }
            }


            [Obsolete]
            public static void ReloadTextureOverWatch(Legacy.TextureOverWatch[] TOWS)
            {

                foreach (Legacy.TextureOverWatch TOW in TOWS)
                {
                    TOW.enabled = true;
                }
            }
            [Obsolete]
            public static void AddTOWs(GameObject gameobject, bool toself = true, bool tochildren = false, bool toparent = false, bool refresh = false)
            {
                BatonPass.Debug("added " + gameobject.name + "to textureoverwatch");
                if (toself)
                {
                    if (!gameobject.GetComponent<Legacy.TextureOverWatch>())
                    {
                        gameobject.AddComponent<Legacy.TextureOverWatch>();
                    }
                    else
                    {
                        gameobject.GetComponent<Legacy.TextureOverWatch>().enabled = refresh;
                    }
                }
                if (toparent)
                {
                    Renderer[] parentRenderers = gameobject.GetComponentsInParent<Renderer>();
                    foreach (Renderer renderer in parentRenderers)
                    {
                        if (renderer != null && renderer.GetType() != typeof(ParticleSystemRenderer) && renderer.GetType() != typeof(CanvasRenderer) && renderer.GetType() != typeof(LineRenderer))
                        {
                            if (!renderer.GetComponent<Legacy.TextureOverWatch>())
                            {
                                renderer.gameObject.AddComponent<Legacy.TextureOverWatch>();
                            }
                            else
                            {
                                renderer.GetComponent<Legacy.TextureOverWatch>().enabled = refresh;
                            }
                        }
                    }
                }
                if (tochildren)
                {
                    Renderer[] childRenderers = gameobject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in childRenderers)
                    {
                        if (renderer != null && renderer.GetType() != typeof(ParticleSystemRenderer) && renderer.GetType() != typeof(CanvasRenderer) && renderer.GetType() != typeof(LineRenderer))
                        {
                            if (!renderer.GetComponent<Legacy.TextureOverWatch>())
                            {
                                renderer.gameObject.AddComponent<Legacy.TextureOverWatch>();
                            }
                            else
                            {
                                renderer.GetComponent<Legacy.TextureOverWatch>().enabled = refresh;
                            }
                        }
                    }
                }
            }

        }

        
        public static Transform FindDeepChildByPathIncludeInactive(Transform root, string path)
        {
            string[] parts = path.Split('/');
            Transform current = root;

            foreach (string part in parts)
            {
                bool found = false;
                foreach (Transform child in current)
                {
                    if (child.name == part)
                    {
                        current = child;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return null; // Path is invalid
            }

            return current;
        }

        private async void SceneManagerOnsceneLoaded(Scene scene, Scene mode)
        {
            BatonPass.Debug("BATON PASS: WE ARE IN SceneManagerOnsceneLoaded()");



            swapped = false;



            BatonPass.Debug("Checking for Null CCE");

            //CreateSkinGUI();
            BatonPass.Info("The Scene is: " + mode.name);
            BatonPass.Debug("INIT BATON PASS: REFRESHSKINS()");
            if (mode.name == "b3e7f2f8052488a45b35549efb98d902")
            {

                MenuCreator.makethemenu();
                if (firstmenu == true)
                {
                    firstmenu = false;
                    RefreshSkins();
                }

            }
            else if (mode.name == "Bootstrap")
            {
                BatonPass.Info("Cant make menu, currently straping my boots");
            }
            else if (mode.name == "241a6a8caec7a13438a5ee786040de32")
            {
                BatonPass.Info("Cant make menu, currently watching a movie");
            }
            else { 
                
                //Pause menu is disabled for now
                //MenuCreator.makethePausemenu();
            }

            if (mode.name == "7b3cb6a0a342eb54dafe5552d4820eeb")
            {
                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/BrennanTerminal.prefab");
                await handle.Task;
                
                Instantiate(handle.Result);
            }

        }









        internal class RANKTITLESWAPPER
        {

            internal static void SwapTheStyleRank(StyleHUD stylehudINST)
            {

                BatonPass.Debug("stylehud has started");
                foreach (StyleRank sr in stylehudINST.ranks)
                {
                    string spritelookup = sr.sprite.name;
                    BatonPass.Debug($"looking for {spritelookup}");
                    if (HoldEm.Bluff(HoldemType.SC, spritelookup))
                    {
                        sr.sprite = HoldEm.Draw<Sprite>(HoldemType.SC, spritelookup);
                        BatonPass.Debug($"found in bluff");
                    }


                }
            }
            internal static void MakeTheStyleRank(TextureLoadEventArgs e)


            {
                if (e.Failed != true)
                {
                    string[] styleranks = new string[8] { "RankSSS", "RankSS", "RankS", "RankA", "RankB", "RankC", "RankD", "RankU" };
                    BatonPass.Debug("stylehud has started");


                    foreach (string sr in styleranks)
                    {
                        string spritelookup = sr;

                        HoldEm.Bet(HoldemType.SC, spritelookup, HoldEm.Call(spritelookup));
                        BatonPass.Debug($"Betting {spritelookup}");



                    }
                }

            }
        }

       











        /*   private void Update(SkinEventHandler skinEventHandler)
           {
               modFolderPath = skinEventHandler.GetModFolderPath();
               if (!swapped)
               {
                   if (Directory.Exists(modFolderPath))
                   {
                       serializedSet = modFolderPath;

                   }
                   ReloadTextures(false, modFolderPath);
                   swapped = true;
               }
           }
   */

        [Obsolete]
        public static bool CheckTextureInCache(string name)
        {
            if (autoSwapCache.ContainsKey(name))
                return true;
            return false;
        }




        /// <summary>
        /// Disables Buttons in the menu and awaits texture loading then dispatches them to their respective fractals
        /// </summary>
        /// <param name="path">the array of paths to load</param>
        /// <param name="firsttime">whether or not the menu should disable buttons</param>
        public async void ReloadTextures(string[] path, bool firsttime = false)
        {
            BatonPass.Debug("BATON PASS: WE ARE IN ReloadTextures() We have variables \n FIRSTTIME:" + firsttime + "\n PATH:" + path);

            if (!firsttime)
            {
                MenuManager.MMinstance.DisableButtons();
                await LoadTextures(path, firsttime);
                MenuManager.MMinstance.EnableButtons();
            }
            else
            {
                await LoadTextures(path, firsttime);
            }
            USAPI.BroadcastTextureUpdate(this, new FractalTextureUpdateArgs(true));

            /*                      foreach (var kvp in ULTRASKINHand.HandInstance.MaterialNames)
                        {
                            BatonPass.Debug($"Key: {kvp.Key}, Value: {kvp.Value}");
                        }
            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
                                    foreach (var tex in textures)
                                    {
                                        if (!AssetInUse(tex))
                                        {
                                            BatonPass.Info($"Unused Texture: {tex.name} - Potential leak");
                                        }
                                    }*/

        }
        bool AssetInUse(UnityEngine.Object asset)
        { 
            return Resources.FindObjectsOfTypeAll<Component>().Any(c =>
                c is Renderer r && r.sharedMaterials.Any(m =>
                    m && m.HasProperty("_MainTex") && m.mainTexture == asset) ||
                c is Image img && img.sprite && img.sprite.texture == asset ||
                c is RawImage rawImg && rawImg.texture == asset);
        }

        [Obsolete]
        public static void InitOWGameObjects(bool firsttime = false)
        {
            /*
            
            //BatonPass.Debug("initializing Overwatch protocall");
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            //BatonPass.Debug("finding renderer");
            foreach (Renderer renderer in cam.GetComponentsInChildren<Renderer>(true))
            {
                //BatonPass.Debug("Renderer found: checking usabilty");
                if (renderer.gameObject.layer == 13 && !renderer.gameObject.GetComponent<ParticleSystemRenderer>() && !renderer.gameObject.GetComponent<TrailRenderer>() && !renderer.gameObject.GetComponent<LineRenderer>())
                {
                    //BatonPass.Debug("Its usable");
                    if (renderer.GetComponent<TextureOverWatch>() && !firsttime)
                    {
                        //BatonPass.Debug("Has Overwatchable Mat");
                        TextureOverWatch TOW = renderer.GetComponent<TextureOverWatch>();
                        TOW.enabled = true;
                        TOW.forceswap = true;
                    }
                    else if (!renderer.GetComponent<TextureOverWatch>())
                    {
                        //BatonPass.Debug("Creating Overwatch Component");
                        renderer.gameObject.AddComponent<TextureOverWatch>().enabled = true;
                    }
                }
            }
            try
            {
                foreach (TextureOverWatch tow in HandInstance.PtowStorage.TOWS)
                {
                    tow.enabled = true;
                    tow.forceswap = true;
                }
            } catch
            {
                BatonPass.Error("Previewer is not available");
            }
            */

        }




        /// <summary>
        /// Loads textures from disk async
        /// </summary>
        /// <param name="fpaths">paths to load</param>
        /// <param name="firsttime"></param>
        /// <returns></returns>
        public async Task LoadTextures(string[] fpaths, bool firsttime = false)
        {
            float ProgressTotal = 0;
            float ProgressDone = 0;

            if (firsttime == false)
            {
                BPGUI.ShowGUI("Loading:Gathering Info");
                BatonPass.Info("Loading: Gathering Info");


            }
            ProgressTotal = autoSwapCache.Count;
            foreach (string fpath in fpaths)
            {
                DirectoryInfo dir = new DirectoryInfo(fpath);
                FileInfo[] Files = dir.GetFiles("*.png");
                ProgressTotal += Files.Length;
            }
            BatonPass.Debug("BATON PASS: WE ARE IN LOADTEXTURES() we have the variable FPATH");



            BPGUI.EnableTerminal(10);
            BPGUI.ShowProgressBar(ProgressTotal);
            foreach (KeyValuePair<string, Texture> kvp in autoSwapCache)
            {
                ProgressDone++;
                BPGUI.updatebar(ProgressDone);

                string name = kvp.Key;
                BatonPass.Debug("Deleting " + name + " from Holdem ASC");
                BPGUI.AddTermLine("Deleting " + name + " from Holdem ASC");
                Texture workingfile = autoSwapCache[name];
                UnityEngine.Object.Destroy(workingfile);

            }
            autoSwapCache.Clear();
            HoldEm.Fold(HoldemType.SC);


            System.Array.Reverse(fpaths);
            BatonPass.Debug("starting ForEach");
            bool failed = false;
            Dictionary<string,string> pathbook = new Dictionary<string,string>();
            foreach (string fpath in fpaths)
            {
                
                switch (TypeDetection(fpath))
                {
                    case ArchiveType.folder:
                        pathbook = QueryTexturesInFolder(fpath,pathbook);
                        break;
                }

            }
            TexOpData texOpData = await LoadOpDataFromPathBook(pathbook);
            await ConvertToTextures(texOpData);
            if (!failed)
            {
                BPGUI.DisableTerminal();
                BPGUI.HideProgressBar();

                BPGUI.BatonPassAnnoucement(Color.green, "success");
            }
            if (firsttime == false)
            {
                BPGUI.HideGUI(2);
            }


            
            USAPI.BroadcastTextureFinished(new USAPI.TextureLoadEventArgs(failed));
           
        }

        public ArchiveType TypeDetection(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext.Equals(".zip", StringComparison.OrdinalIgnoreCase)){
                return ArchiveType.zip;
            }
            else {
                return ArchiveType.folder;
            }
        }




        public enum ArchiveType
        {
            folder,
            zip,
            gcskin,
        }

        public class TexOpData
        {
            public List<(string name, byte[] data)> RawData = new List<(string name, byte[] data)>();
            public float ProgressState { get; set; }
            public bool FailState { get; set; }
        }
        public class UniOpData
        {
            
            public float ProgressState { get; set; }
            public bool FailState { get; set; }
        }


        public Dictionary<string, string> QueryTexturesInFolder(string fpath, Dictionary<string,string> pathbook)
        {
            BatonPass.Info("ULTRASKINS IS SEARCHING " + fpath.ToString());

            DirectoryInfo dir = new DirectoryInfo(fpath);

            if (!dir.Exists)
            {
                BPGUI.DisableTerminal();
                BPGUI.BatonPassAnnoucement(Color.red, "failed, CODE - \"USHAND-LOADTEXTURES-DIR_NOT_FOUND\" \n FILEPATH:" + fpath);
                BatonPass.Error("Dir does not exist, CODE - \"USHAND-LOADTEXTURES-DIR_NOT_FOUND\" ");
                //failed = true;
                BPGUI.HideGUI(5);
                return pathbook; // Exit early if the directory doesn't exist
            }

            FileInfo[] Files = dir.GetFiles("*.png");
            BatonPass.Debug("Beginning file swap loop");

            if (Files.Length > 0)
            {
                int totalFiles = Files.Length;



                foreach (FileInfo file in Files)
                {
                    if (file.Exists)
                    {
                        pathbook[file.Name] = file.FullName;
                        BatonPass.Debug("found " + file.FullName);
                    }
                }
            }
            return pathbook;
        }

        public async Task<TexOpData> LoadOpDataFromPathBook(Dictionary<string,string> pathbook, float ProgressDone = 0, bool failed = false)
        {
            TexOpData texOpData = new TexOpData();
            foreach (KeyValuePair<string, string> kvp in pathbook)
            {
                try
                {

                    // Read file asynchronously
                    byte[] data = await File.ReadAllBytesAsync(kvp.Value);
                    string name = Path.GetFileNameWithoutExtension(kvp.Value);
                    if (autoSwapCache.ContainsKey(name))
                    {
                        Texture workingfile = autoSwapCache[name];
                        UnityEngine.Object.Destroy(workingfile);
                        autoSwapCache.Remove(name);
                    }


                    BatonPass.Debug("Reading " + kvp.Value);
                    texOpData.RawData.Add((name, data));
                    BPGUI.AddTermLine("Creating " + name);

                }
                catch (Exception ex)
                {
                    Logger.LogError("Error reading or processing texture file: " + kvp.Value + " Error: " + ex.Message);
                    failed = true;
                    BPGUI.DisableTerminal();
                    BPGUI.BatonPassAnnoucement(Color.red, "failed");
                }
            }
            return texOpData;
        }






        [Obsolete]
        public async Task<TexOpData> LoadTexturesFromFolder(string fpath, float ProgressDone = 0, bool failed = false)
        {
            TexOpData texOpData = new TexOpData();
            try
            {

                BatonPass.Info("ULTRASKINS IS SEARCHING " + fpath.ToString());


                //autoSwapCache.Clear();


                DirectoryInfo dir = new DirectoryInfo(fpath);

                if (!dir.Exists)
                {
                    BPGUI.DisableTerminal();
                    BPGUI.BatonPassAnnoucement(Color.red, "failed, CODE - \"USHAND-LOADTEXTURES-DIR_NOT_FOUND\" \n FILEPATH:" + fpath);
                    BatonPass.Error("Dir does not exist, CODE - \"USHAND-LOADTEXTURES-DIR_NOT_FOUND\" ");
                    failed = true;
                    BPGUI.HideGUI(5);
                    return null; // Exit early if the directory doesn't exist
                }
                
                FileInfo[] Files = dir.GetFiles("*.png");
                BatonPass.Debug("Beginning file swap loop");

                if (Files.Length > 0)
                {
                    int totalFiles = Files.Length;



                    foreach (FileInfo file in Files)
                    {
                        if (file.Exists)
                        {
                            try
                            {

                                // Read file asynchronously
                                byte[] data = await File.ReadAllBytesAsync(file.FullName);
                                string name = Path.GetFileNameWithoutExtension(file.FullName);
                                if (autoSwapCache.ContainsKey(name))
                                {
                                    Texture workingfile = autoSwapCache[name];
                                    UnityEngine.Object.Destroy(workingfile);
                                    autoSwapCache.Remove(name);
                                }
                                

                                BatonPass.Debug("Reading " + file.FullName.ToString());
                                texOpData.RawData.Add((name, data));
                                BPGUI.AddTermLine("Creating " + name);

                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("Error reading or processing texture file: " + file.Name + " Error: " + ex.Message);
                                failed = true;
                                BPGUI.DisableTerminal();
                                BPGUI.BatonPassAnnoucement(Color.red, "failed");
                            }

                        }
                        else
                        {
                            BatonPass.Error("HEAR YE HEAR YE, They got away, CODE -\"USHAND-LOADTEXTURES-CACHEFAIL\"");
                            failed = true;
                            BPGUI.DisableTerminal();
                            BPGUI.BatonPassAnnoucement(Color.red, "failed");
                        }


                        ProgressDone++;



                        BPGUI.updatebar(ProgressDone);


                    }

                    if (!failed)
                    {
                        BatonPass.Info("We Got the Textures from " + fpath);

                    }
                }
                else
                {
                    BatonPass.Error("HEAR YE HEAR YE The length of the files in " + fpath + " is zero, CODE -\"USHAND-LOADTEXTURES-0INDEX\"");
                    BPGUI.DisableTerminal();
                    BPGUI.HideProgressBar();
                    failed = true;
                    BPGUI.BatonPassAnnoucement(Color.yellow, "No files found in " + fpath + ", CODE -\\\"USHAND-LOADTEXTURES-0INDEX\\\"\"");
                }


            }
            catch (Exception ex)
            {
                BatonPass.Error("Failed to load all textures from \" + Path.GetFileName(fpath) + \".\\nPlease ensure all of the Texture Files names are Correct.");
                BatonPass.Error("HEAR YE HEAR YE, The Search Was Fruitless, CODE -\"USHAND-LOADTEXTURES-EX\" " + ex.Message);
                BatonPass.Error("HEAR YE HEAR YE, The Search Was Fruitless, CODE -\"USHAND-LOADTEXTURES-EX\" " + ex.ToString());

                BPGUI.BatonPassAnnoucement(Color.red, "Failed to load all textures from " + Path.GetFileName(fpath) + ".\nPlease ensure all of the Texture Files names are Correct.");
            }
            texOpData.FailState = failed;
            texOpData.ProgressState = ProgressDone;
            
            return texOpData;
        }

        public async Task<UniOpData> ConvertToTextures(TexOpData texOpData)
        {
            UniOpData uniOpData = new UniOpData();
            bool failed = texOpData.FailState;
            float progress = texOpData.ProgressState;
            foreach ((string name, byte[] data) in texOpData.RawData)
            {
                Texture2D texture2D = new Texture2D(2, 2);
                texture2D.name = name;

                BatonPass.Debug("Creating " + texture2D.name);
                
                texture2D.filterMode = FilterMode.Point;
                texture2D.LoadImage(data);
                BatonPass.Debug("Loading Image Data");
                texture2D.Apply();


                // Cache the texture
                Texture texture = texture2D;
                BatonPass.Debug("Setting texture");
                autoSwapCache.Add(name, texture);
                BatonPass.Debug("Adding to Cache " + texture.name + " " + name);
                progress++;
            }
            uniOpData.ProgressState = progress;
            uniOpData.FailState = failed;
            return uniOpData;
        }
        //Baton Pass handles debug logging for several different types of debuggers


/*        public static void BatonPass(string message)
        {
            if (ShouldDoBatonPass == true)
            {
                BatonPassLogger.LogInfo(message);
                if (ShouldDoBatonPassUnity == true)
                {
                    Debug.Log(message);
                }
            }
        }*/
    }
}
