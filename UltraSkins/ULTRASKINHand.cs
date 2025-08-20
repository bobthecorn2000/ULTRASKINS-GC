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

using StringSerializer = UltraSkins.Utils.SkinEventHandler.StringSerializer;
using BatonPassLogger;
using System.Xml.Serialization;
using static UltraSkins.ULTRASKINHand.HoldEm;





//using UnityEngine.UIElements;






namespace UltraSkins
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]


    public class ULTRASKINHand : BaseUnityPlugin
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
        public TowStorage PtowStorage;
        public Dictionary<string, Texture2D> IconCache = new Dictionary<string, Texture2D>();
        public Dictionary<string, Sprite> SpriteCache = new Dictionary<string, Sprite>();
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
            MenuCreator.CreateSMan();
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
                
                HandInstance.MaterialNames.Add("Swapped_weapon_GreenArm (Instance)(Clone) (Instance)", "T_GreenArm");
                HandInstance.MaterialNames.Add("Swapped_weapon_FeedbackerLit (Instance)(Clone) (Instance)", "T_Feedbacker");
                HandInstance.MaterialNames.Add("Swapped_weapon_RedArmLit (Instance)(Clone) (Instance)", "v2_armtex");
                HandInstance.MaterialNames.Add("Swapped_weapon_MainArmLit (Instance)(Clone) (Instance)", "T_MainArm");

            }
            catch (Exception ex)
            {
                BatonPass.Error("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.Message);


            }
            //refreshskins();


        }

        public void refreshskins(string[] clickedButtons)
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
        void refreshskins()
        {

            BatonPass.Debug("BATON PASS: WE ARE IN REFRESHSKINS(), THERE ARE NO OTHER ARGUMENTS");
            StringSerializer serializer = new StringSerializer();
            BatonPass.Debug("Created The Serializer");
            BatonPass.Debug("looking for the dll, appdata paths and merging the directory strings");
            string save = SkinEventHandler.getDataFile();



            filepathArray = serializer.DeserializeStringFromFile(save);
            BatonPass.Info("Read data.USGC from " + filepathArray);
            BatonPass.Debug("INIT BATON PASS: RELOADTEXTURES(TRUE," + filepathArray + ")");
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
                RANKTITLESWAPPER.swapthestylerank(__instance);
            }
        }
        [HarmonyPatch]
        public class HarmonyGunPatcher
        {

            public static List<TextureOverWatch> AddPTOWs(GameObject gameobject,bool refresh)
            {
                List<TextureOverWatch> ptows = new List<TextureOverWatch>();
                Renderer[] childRenderers = gameobject.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in childRenderers)
                {
                    if (renderer != null && renderer.GetType() != typeof(ParticleSystemRenderer) && renderer.GetType() != typeof(CanvasRenderer) && renderer.GetType() != typeof(LineRenderer))
                    {
                        if (!renderer.GetComponent<TextureOverWatch>())
                        {
                            TextureOverWatch tow = renderer.gameObject.AddComponent<TextureOverWatch>();
                            ptows.Add(tow);
                        }
                        else
                        {
                            renderer.GetComponent<TextureOverWatch>().enabled = refresh;

                        }
                    }
                }
                return ptows;
            }
            public static List<TextureOverWatch> AddTOWs(GameObject gameobject, bool toself = true, bool tochildren = false, bool toparent = false, bool refresh = false)
            {
                BatonPass.Debug("added " + gameobject.name + "to textureoverwatch");
                List<TextureOverWatch> tows = new List<TextureOverWatch>();
                if (toself)
                {
                    if (!gameobject.GetComponent<TextureOverWatch>())
                    {
                        TextureOverWatch tow = gameobject.AddComponent<TextureOverWatch>();
                        tows.Add(tow);
                    }
                    else
                    {
                        gameobject.GetComponent<TextureOverWatch>().enabled = refresh;
                    }
                }
                if (toparent)
                {
                    Renderer[] parentRenderers = gameobject.GetComponentsInParent<Renderer>();
                    foreach (Renderer renderer in parentRenderers)
                    {
                        if (renderer != null && renderer.GetType() != typeof(ParticleSystemRenderer) && renderer.GetType() != typeof(CanvasRenderer) && renderer.GetType() != typeof(LineRenderer))
                        {
                            if (!renderer.GetComponent<TextureOverWatch>())
                            {
                                TextureOverWatch tow = renderer.gameObject.AddComponent<TextureOverWatch>();
                                tows.Add(tow);
                            }
                            else
                            {
                                renderer.GetComponent<TextureOverWatch>().enabled = refresh;
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
                            if (!renderer.GetComponent<TextureOverWatch>())
                            {
                                TextureOverWatch tow = renderer.gameObject.AddComponent<TextureOverWatch>();
                                tows.Add(tow);
                            }
                            else
                            {
                                renderer.GetComponent<TextureOverWatch>().enabled = refresh;
                                
                            }
                        }
                    }
                }
                return tows;
            }

            [HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] { typeof(int), typeof(int?), typeof(bool), typeof(bool), typeof(bool) })]
            [HarmonyPostfix]
            public static void SwitchWeaponPost(GunControl __instance, int targetSlotIndex, int? targetVariationIndex = null, bool useRetainedVariation = false, bool cycleSlot = false, bool cycleVariation = false)
            {

                //TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                //ReloadTextureOverWatch(TOWS);
            }

            [HarmonyPatch(typeof(ShopZone), "Start")]
            [HarmonyPostfix]
            public static void Start(ShopZone __instance)
            {
                //Transform trans = FindDeepChildByPathIncludeInactive(__instance.transform, "Canvas/Background/Main Panel/Weapons/Arm Window");
                //if (trans != null)
                //{
                //    //List<TextureOverWatch> SHOPTOWS = AddTOWs(__instance.gameObject, true, true, false, true);
                //    Addressables.LoadAssetAsync<GameObject>("Assets/ultraskins/HandPaintPlusColors.prefab").Completed += handle =>
                //    {
                //        GameObject prefabmenu = handle.Result;


                    
                //        GameObject ColorMenu = Instantiate(prefabmenu, trans);
                    
                //    };
                //}

                //ReloadTextureOverWatch(TOWS);
            }

            [HarmonyPatch(typeof(GunControl), "YesWeapon")]
            [HarmonyPostfix]
            public static void WeaponYesPost(GunControl __instance)
            {
                if (!__instance.noWeapons)
                {
                    TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                    ReloadTextureOverWatch(TOWS);
                }
            }




            [HarmonyPatch(typeof(GunControl), "UpdateWeaponList", new Type[] { typeof(bool) })]
            [HarmonyPostfix]
            public static void UpdateWeaponListPost(GunControl __instance, bool firstTime = false)
            {
                InitOWGameObjects(true);
                TextureOverWatch[] TOWS = CameraController.Instance.gameObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }





            [HarmonyPatch(typeof(FistControl), "YesFist")]
            [HarmonyPostfix]
            public static void YesFistPost(FistControl __instance)
            {
                if (__instance.currentArmObject)
                {
                    TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                    ReloadTextureOverWatch(TOWS);
                }
            }

            [HarmonyPatch(typeof(FistControl), "ArmChange", new Type[] { typeof(int) })]
            [HarmonyPostfix]
            public static void SwitchFistPost(FistControl __instance, int orderNum)
            {
                try
                {
                    TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                    ReloadTextureOverWatch(TOWS);
                }
                catch (ArgumentOutOfRangeException AOOR) {
                    BatonPass.Warn("HEAR YEE HEAR YEE CurrentArmObject Argument Out of Range " + AOOR.ToString());
                    BatonPass.Warn("currentArmObject is empty, this is normal if you are in 5-S, P-1 or P-2  CODE -\"USHAND-GUNPATCHER-FC_ARMCHANGE_SFP-ARG_OUT_OF_RANGE\"");
                }

            }

            [HarmonyPatch(typeof(FistControl), "ResetFists")]
            [HarmonyPostfix]
            public static void ResetFistsPost(FistControl __instance)
            {
                InitOWGameObjects(false);
                TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }


            [HarmonyPatch(typeof(DualWieldPickup), "PickedUp")]
            [HarmonyPostfix]
            public static void DPickedupPost(DualWieldPickup __instance)
            {
                if (GunControl.Instance)
                {
                    if (PlayerTracker.Instance.playerType != PlayerType.Platformer)
                    {
                        DualWield[] DWs = GunControl.Instance.GetComponentsInChildren<DualWield>(true);
                        foreach (DualWield DW in DWs)
                        {
                            if (DW)
                            {
                                Renderer[] renderers = DW.GetComponentsInChildren<Renderer>(true);
                                foreach (Renderer renderer in renderers)
                                {
                                    if (renderer && renderer.gameObject.layer == 13 && !renderer.gameObject.GetComponent<ParticleSystemRenderer>() && !renderer.gameObject.GetComponent<CanvasRenderer>())
                                    {
                                        if (!renderer.gameObject.GetComponent<TextureOverWatch>())
                                        {
                                            
                                            TextureOverWatch TOW = renderer.gameObject.AddComponent<TextureOverWatch>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            [HarmonyPatch(typeof(DualWield), "UpdateWeapon")]
            [HarmonyPostfix]
            public static void DUpdateWPost(DualWield __instance)
            {
                TextureOverWatch[] TOWS = __instance.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }


            [HarmonyPatch(typeof(PlayerTracker), "ChangeToFPS")]
            [HarmonyPostfix]
            public static void ChangeToFPSPost(PlayerTracker __instance)
            {
                TextureOverWatch[] WTOWS = GameObject.FindGameObjectWithTag("GunControl").GetComponent<GunControl>().currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(WTOWS);
                TextureOverWatch[] FTOWS = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<FistControl>().currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(FTOWS);
            }


            public static void ReloadTextureOverWatch(TextureOverWatch[] TOWS)
            {
                foreach (TextureOverWatch TOW in TOWS)
                {
                    TOW.enabled = true;
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
                    AddTOWs(__instance.gameObject, false, true);
                }

            }

            [HarmonyPatch(typeof(Magnet), "Start")]
            [HarmonyPostfix]
            public static void MagnetPost(Magnet __instance)
            {
                AddTOWs(__instance.transform.parent.gameObject, false, true);
            }

            [HarmonyPatch(typeof(Grenade), "Start")]
            [HarmonyPostfix]
            public static void GrenadePost(Grenade __instance)
            {
                AddTOWs(__instance.gameObject, false, true);
            }

            [HarmonyPatch(typeof(Coin), "Start")]
            [HarmonyPostfix]
            public static void coinPost(Coin __instance)
            {
                AddTOWs(__instance.gameObject, false,true);
            }

            public static void ReloadTextureOverWatch(TextureOverWatch[] TOWS)
            {

                foreach (TextureOverWatch TOW in TOWS)
                {
                    TOW.enabled = true;
                }
            }

            public static void AddTOWs(GameObject gameobject, bool toself = true, bool tochildren = false, bool toparent = false, bool refresh = false)
            {
                BatonPass.Debug("added " + gameobject.name + "to textureoverwatch");
                if (toself)
                {
                    if (!gameobject.GetComponent<TextureOverWatch>())
                    {
                        gameobject.AddComponent<TextureOverWatch>();
                    }
                    else
                    {
                        gameobject.GetComponent<TextureOverWatch>().enabled = refresh;
                    }
                }
                if (toparent)
                {
                    Renderer[] parentRenderers = gameobject.GetComponentsInParent<Renderer>();
                    foreach (Renderer renderer in parentRenderers)
                    {
                        if (renderer != null && renderer.GetType() != typeof(ParticleSystemRenderer) && renderer.GetType() != typeof(CanvasRenderer) && renderer.GetType() != typeof(LineRenderer))
                        {
                            if (!renderer.GetComponent<TextureOverWatch>())
                            {
                                renderer.gameObject.AddComponent<TextureOverWatch>();
                            }
                            else
                            {
                                renderer.GetComponent<TextureOverWatch>().enabled = refresh;
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
                            if (!renderer.GetComponent<TextureOverWatch>())
                            {
                                renderer.gameObject.AddComponent<TextureOverWatch>();
                            }
                            else
                            {
                                renderer.GetComponent<TextureOverWatch>().enabled = refresh;
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

        private void SceneManagerOnsceneLoaded(Scene scene, Scene mode)
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
                    refreshskins();
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

        }




        public static Texture ResolveTheTextureProperty(Material mat, string property, string texturename, string propertyfallback = "_MainTex")
        {

            if (mat != null && texturename == null)
                return null;

            string textureToResolve = "";
            if (mat && !texturename.StartsWith("TNR_") && property != "_Cube")
            {
                switch (property)
                {
                    case "_MainTex":
                        textureToResolve = texturename;
                        break;
                    case "_EmissiveTex":
                        switch (texturename)
                        {
                            case "T_NailgunNew_NoGlow":
                                textureToResolve = "T_Nailgun_New_Glow";
                                break;
                            case "T_RocketLauncher_Desaturated":
                                textureToResolve = "T_RocketLauncher_Emissive";
                                if (HoldEm.Check(textureToResolve))
                                {
                                    mat.EnableKeyword("EMISSIVE");
                                    mat.SetInt("_UseAlbedoAsEmissive", 0);
                                }
                                break;
                            case "T_ImpactHammer":
                                textureToResolve = "T_ImpactHammer_Glow";
                                break;
                            default:
                                textureToResolve = texturename + "_Emissive";
                                if (HoldEm.Check(textureToResolve))
                                {
                                    mat.EnableKeyword("EMISSIVE");
                                    mat.SetInt("_UseAlbedoAsEmissive", 0);
                                }

                                break;
                        }
                        break;
                    case "_IDTex":
                        switch (mat.mainTexture.name)
                        {
                            case "T_RocketLauncher_Desaturated":
                                textureToResolve = "T_RocketLauncher_ID";
                                break;
                            case "T_NailgunNew_NoGlow":
                                textureToResolve = "T_NailgunNew_ID";
                                break;
                            case "Railgun_Main_AlphaGlow":
                                textureToResolve = "T_Railgun_ID";
                                break;
                            default:
                                textureToResolve = mat.mainTexture.name + "_ID";
                                break;
                        }
                        break;
                    case "ROCKIT":
                        textureToResolve = (mat.name.Contains("Swapped_rocket_AltarUnlitRed") && !texturename.StartsWith("T_")) ? "skull2rocketbonus" : texturename.Contains("T_Sakuya") ? "" : "skull2rocket";
                        break;
                    case "THROWITBACK":
                        textureToResolve = "skull2grenade";
                        break;
                    default:
                        textureToResolve = "";
                        break;
                }
                if (textureToResolve != "" && HoldEm.Check(textureToResolve))
                    return HoldEm.Call(textureToResolve);
            }
            return mat.GetTexture(propertyfallback);
        }
        string GetTextureName(string materialName)
        {
            if (ULTRASKINHand.HandInstance.MaterialNames.TryGetValue(materialName, out string textureName))
            {
                // If the material name exists, return the texture name
                return textureName;
            }
            else
            {
                // If the material name does not exist, return a default value (e.g., "Texture Not Found")
                return null;
            }
        }
        public static void PerformTheSwap(Material mat, bool forceswap = false, TextureOverWatch TOW = null, string swapType = "weapon")
        {
            if (mat && (!mat.name.StartsWith("Swapped_") || forceswap))
            {

                if (!mat.name.StartsWith("Swapped_"))
                {
                    mat.name = "Swapped_" + swapType + "_" + mat.name;
                }

                forceswap = false;
                Texture resolvedTexture = new Texture();
                string texturename = HandInstance.GetTextureName(mat.name);
                BatonPass.Info("I should change" + TOW.iChange);
                BatonPass.Debug("requested " + mat.name + " got " + texturename);

                if (swapType == "weapon")
                {

                    string[] textureProperties = mat.GetTexturePropertyNames();

                    foreach (string property in textureProperties)
                    {




                        resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, property, texturename, property);
                        //BatonPass.Info("Attempting to swap " + property + " of " + mat.name.ToString() + " with " + resolvedTexture.name.ToString());
                        if (resolvedTexture && resolvedTexture != null && mat.HasProperty(property) && mat.GetTexture(property) != resolvedTexture)
                        {
                            //Debug.Log("swapping " + property + " of " + mat.name.ToString());

                            mat.SetTexture(property, resolvedTexture);

                        }

                        if (TOW != null && mat.HasProperty("_EmissiveColor"))
                        {

                            if (mat.name.ToString() == "Swapped_weapon_ImpactHammerDial (Instance)")
                            {
                                break;
                            }
                            else
                            {

                                //Debug.Log("swapping " + property + " of " + mat.name.ToString());
                                Color VariantColor = GetVarationColor(TOW);
                                Color VariantColor2 = new Color(255, 255, 255, 255);
                                mat.SetColor("_EmissiveColor", VariantColor);
                            }
                        }

                    }


                }
                else if (swapType == "projectile")
                {
                    resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, "_MainTex", texturename);
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {

                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }
                else if (swapType == "grenade")
                {
                    resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, "THROWITBACK", texturename);
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {

                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }
                else if (swapType == "rocket")
                {
                    resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, "ROCKIT", texturename);
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {

                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }

            }
        }


        internal class RANKTITLESWAPPER
        {

            internal static void swapthestylerank(StyleHUD stylehudINST)
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
            internal static void makethestylerank()
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

        internal class ReadOut
        {

            private static readonly AccessTools.FieldRef<ShotgunHammer, Texture[]> meterEmissivesRef = AccessTools.FieldRefAccess<ShotgunHammer, Texture[]>("meterEmissives");

            private static readonly AccessTools.FieldRef<ShotgunHammer, Image> secondaryMeterRef = AccessTools.FieldRefAccess<ShotgunHammer, Image>("secondaryMeter");

            internal delegate void UpdateMeterDelegate(ShotgunHammer instance, bool forceUpdateTexture = false);

            static MethodInfo updateMeterMethod = AccessTools.Method(typeof(ShotgunHammer), "UpdateMeter");

            internal static UpdateMeterDelegate updateMeter = AccessTools.MethodDelegate<UpdateMeterDelegate>(updateMeterMethod);


            internal static void SwapTheDial(TextureOverWatch TOW)
            {


                if (meterEmissivesRef == null)
                {
                    BatonPass.Error("Failed to find 'meterEmissives' field.");
                    return;
                }
                ShotgunHammer shotgunHammer = TOW.GetComponentInParent<ShotgunHammer>();
                var meterEmissives = meterEmissivesRef(shotgunHammer);
                var meterMask = secondaryMeterRef(shotgunHammer);
                Texture glow1;
                Texture glow2;
                Texture glow3;
                if (HoldEm.Check("T_DialGlow1"))
                {
                    glow1 = HoldEm.Call("T_DialGlow1");
                }
                else
                {
                    glow1 = meterEmissives[0];
                }
                if (HoldEm.Check("T_DialGlow2"))
                {
                    glow2 = HoldEm.Call("T_DialGlow2");
                }
                else
                {
                    glow2 = meterEmissives[1];
                }
                if (HoldEm.Check("T_DialGlow3"))
                {
                    glow3 = HoldEm.Call("T_DialGlow3");
                }
                else
                {
                    glow3 = meterEmissives[2];
                }
                meterEmissivesRef(shotgunHammer) = new Texture[3]
                    {
                        glow1,
                        glow2,
                        glow3,
                    };

                if (HoldEm.Check("T_DialMask"))
                {
                    Texture dialmask = HoldEm.Call("T_DialMask");

                    Sprite masksprite = Sprite.Create((Texture2D)dialmask, new Rect(0, 0, dialmask.width, dialmask.height), new Vector2(0.5f, 0.5f));
                    meterMask.sprite = masksprite;
                }
            }
        }

       
        





        public static Color GetVarationColor(TextureOverWatch TOW)
        {
            Color VariantColor = new Color(0, 0, 0, 0);
            if (TOW.GetComponentInParent<WeaponIcon>())
            {

                WeaponIcon WPI = TOW.GetComponentInParent<WeaponIcon>();
                Type type = WPI.GetType();
                PropertyInfo propertyInfo = type.GetProperty("variationColor", BindingFlags.NonPublic | BindingFlags.Instance);
                int value = (int)propertyInfo.GetValue(WPI);
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[value].r,
                    ColorBlindSettings.Instance.variationColors[value].g,
                    ColorBlindSettings.Instance.variationColors[value].b, 1f);
            }
            else if (TOW.GetComponentInParent<Punch>())
            {
                Punch P = TOW.GetComponentInParent<Punch>();
                switch (P.type)
                {
                    case FistType.Heavy:
                        VariantColor = new Color(ColorBlindSettings.Instance.variationColors[2].r,
                    ColorBlindSettings.Instance.variationColors[2].g,
                    ColorBlindSettings.Instance.variationColors[2].b, 1f);

                        break;
                    case FistType.Standard:
                        VariantColor = new Color(ColorBlindSettings.Instance.variationColors[0].r,
                   ColorBlindSettings.Instance.variationColors[0].g,
                   ColorBlindSettings.Instance.variationColors[0].b, 1f);
                        break;
                }

            }
            else if (TOW.GetComponentInParent<HookArm>())
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[1].r,
                    ColorBlindSettings.Instance.variationColors[1].g,
                    ColorBlindSettings.Instance.variationColors[1].b, 1f);
            }
            else if (TOW.GetComponentInParent<BandScroller>())
            {

            }
                return VariantColor;
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
        public static bool CheckTextureInCache(string name)
        {
            if (autoSwapCache.ContainsKey(name))
                return true;
            return false;
        }

        public async void ReloadTextures(string[] path, bool firsttime = false)
        {
            BatonPass.Debug("BATON PASS: WE ARE IN ReloadTextures() We have variables \n FIRSTTIME:" + firsttime + "\n PATH:" + path);
            BatonPass.Debug("Start Comparing");

#pragma warning restore CS1717 // Assignment made to same variable
            BatonPass.Debug("INIT BATON PASS: INITOWGAMEOBJECTS(" + firsttime + ")");

            BatonPass.Debug("BATON PASS: WELCOME BACK TO RELOADTEXTURES()");
            BatonPass.Debug("INIT BATON PASS: LOADTEXTURES(" + path + ")");
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
            InitOWGameObjects(firsttime);
            foreach (var kvp in ULTRASKINHand.HandInstance.MaterialNames)
            {
                BatonPass.Debug($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
/*            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
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
        public static void InitOWGameObjects(bool firsttime = false)
        {
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");

            foreach (Renderer renderer in cam.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.gameObject.layer == 13 && !renderer.gameObject.GetComponent<ParticleSystemRenderer>() && !renderer.gameObject.GetComponent<TrailRenderer>() && !renderer.gameObject.GetComponent<LineRenderer>())
                {
                    if (renderer.GetComponent<TextureOverWatch>() && !firsttime)
                    {
                        TextureOverWatch TOW = renderer.GetComponent<TextureOverWatch>();
                        TOW.enabled = true;
                        TOW.forceswap = true;
                    }
                    else if (!renderer.GetComponent<TextureOverWatch>())
                    {
                        renderer.gameObject.AddComponent<TextureOverWatch>().enabled = true;
                    }
                }
            }
            foreach(TextureOverWatch tow in HandInstance.PtowStorage.TOWS)
            {
                tow.enabled = true;
                tow.forceswap = true;
            }

        }

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
                BPGUI.AddTermLine("Creating " + name + " from Holdem ASC");
                Texture workingfile = autoSwapCache[name];
                UnityEngine.Object.Destroy(workingfile);

            }
            autoSwapCache.Clear();
            HoldEm.Fold(HoldemType.SC);


            System.Array.Reverse(fpaths);
            BatonPass.Debug("starting ForEach");
            bool failed = false;
            foreach (string fpath in fpaths)
            {
                TexOpData texOpData = new TexOpData();
                switch (TypeDetection(fpath))
                {
                    case archivetype.folder:
                        texOpData = await LoadTexturesFromFolder(fpath);
                        break;
                }
                await ConvertToTextures(texOpData);

                
                
            }
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
            RANKTITLESWAPPER.makethestylerank();
        }

        public archivetype TypeDetection(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext.Equals(".zip", StringComparison.OrdinalIgnoreCase)){
                return archivetype.zip;
            }
            else {
                return archivetype.folder;
            }
        }




        public enum archivetype
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








        /// <summary>
        /// The HoldEm system allows you to request and modify the textures Ultraskins currently keeps track of
        /// </summary>
        public class HoldEm
        {

            /// <summary>
            /// one of either 
            /// ASC - AUTOSWAPCACHE (the users loaded skins)
            /// OGS - OGSKINS (the master list of all supported skins, the secondary cache)
            /// IC - ICONCACHE (all skin icons for this session) 
            /// SC - SPRITECACHE (all currently known sprites)
            /// </summary>
            public enum HoldemType
            {
                ASC = 0, OGS = 1, IC = 2, SC = 3
            }
            /// <summary>
            /// Returns the Texture If it exists in either Primary or Secondary Cache
            /// </summary>
            /// <param name="key">The KEY we are looking for</param>
            /// <returns>A Texture</returns>
            public static Texture Call(string key)
            {
                if (autoSwapCache.TryGetValue(key, out Texture texture))
                {
                    BatonPass.Debug("ASC Call:" + key);
                    return texture;
                }
                else if (HandInstance.ogSkinsManager.OGSKINS.TryGetValue(key, out Texture originalTexture))
                {
                    BatonPass.Debug("OGS Call:" + key);
                    return originalTexture;
                }
                else
                {
                    BatonPass.Debug("Call NoKeyFound" + key);
                    // Not found anywhere
                    return null;
                }
            }
            /// <summary>
            /// Check if a KEY exists in either Primary Cache or Secondary Cache
            /// </summary>
            /// <param name="key">the KEY we are looking for</param>
            /// <returns>Bool "True" if we have it "False" if we don't</returns>
            public static bool Check(string key)
            {
                if (autoSwapCache.ContainsKey(key))
                {
                    BatonPass.Debug("ASC Check:" + key);
                    return true;
                }
                else if (HandInstance.ogSkinsManager.OGSKINS.ContainsKey(key))
                {
                    BatonPass.Debug("OGS Check:" + key);
                    return true;
                }
                else
                {
                    // Not found anywhere
                    BatonPass.Debug("Check NoKeyFound" + key);
                    return false;
                }
            }
            /// <summary>
            /// Removes a specific item from the Dictionary
            /// </summary>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="name">The KEY we are removing</param>
            public static void Discard(HoldemType holdemType, string name)
            {


                switch (holdemType)
                {
                    case HoldemType.ASC:
                        if (autoSwapCache.ContainsKey(name))
                        {
                            Texture workingfile = autoSwapCache[name];
                            UnityEngine.Object.Destroy(workingfile);
                            autoSwapCache.Remove(name);
                        }
                        break;
                    case HoldemType.OGS:
                        if (HandInstance.ogSkinsManager.OGSKINS.ContainsKey(name))
                        {
                            Texture workingfile = HandInstance.ogSkinsManager.OGSKINS[name];
                            UnityEngine.Object.Destroy(workingfile);
                            HandInstance.ogSkinsManager.OGSKINS.Remove(name);
                        }
                        break;
                    case HoldemType.IC:
                        if (HandInstance.IconCache.ContainsKey(name))
                        {
                         
                            HandInstance.IconCache.Remove(name);
                        }
                        break;
                    case HoldemType.SC:
                        if (HandInstance.SpriteCache.ContainsKey(name))
                        {

                            HandInstance.SpriteCache.Remove(name);
                        }
                        break;
                }
            }
            /// <summary>
            /// Adds a Key Value pair into the Holdem System, Placing a Bet on the table
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="TextureName">The KEY for the dict</param>
            /// <param name="texture2D">the VALUE for the dict, In the case of IC this is a byte[]</param>
            public static void Bet(HoldemType holdemType,string TextureName,Texture texture2D)
            {
                switch (holdemType)
                {
                    case HoldemType.ASC:
                        
                        autoSwapCache.Add(TextureName, texture2D);
                        break;
                    case HoldemType.OGS:
                        HandInstance.ogSkinsManager.OGSKINS.Add(TextureName, texture2D);
                        break;
                    case HoldemType.IC:
                        HandInstance.IconCache.Add(TextureName, texture2D as Texture2D);
                        break;
                    case HoldemType.SC:
                        texture2D.filterMode = FilterMode.Bilinear;
                        Sprite newsprite = Sprite.Create(texture2D as Texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        HandInstance.SpriteCache.Add(TextureName, newsprite);
                        break;

                }
            }
            /// <summary>
            /// Pulls a Value from a Key in the Holdem System, ,
            /// </summary>
            /// <typeparam name="T">Either a Texture or a Sprite</typeparam>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="name">The KEY we are asking for</param>
            /// <returns>Draw a Texture or byte[] into your hand</returns>
            public static T Draw<T>(HoldemType holdemType, string name) where T : class
            {

                switch (holdemType)
                {
                    case HoldemType.ASC:
                        autoSwapCache.TryGetValue(name, out Texture rawASCtexture);
                        Texture2D ASCtexture = rawASCtexture as Texture2D;
                        return ASCtexture as T;

                    case HoldemType.OGS:
                        HandInstance.ogSkinsManager.OGSKINS.TryGetValue(name, out Texture rawOGtexture);
                        Texture2D OGtexture = rawOGtexture as Texture2D;
                        return OGtexture as T;

                    case HoldemType.IC:
                        HandInstance.IconCache.TryGetValue(name, out Texture2D ICtexture);
                        return ICtexture as T;
                    case HoldemType.SC:
                        HandInstance.SpriteCache.TryGetValue(name, out Sprite SCtexture);
                        return SCtexture as T;
                    default:
                        return null;

                }

            }
            /// <summary>
            /// Check a specific holdem stash for a texture
            /// </summary>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            /// <param name="name">The KEY we are asking for</param>
            /// <returns></returns>
            public static bool Bluff(HoldemType holdemType, string name)
            {
                switch (holdemType)
                {
                    case HoldemType.ASC:
                        return (autoSwapCache.ContainsKey(name)) ? true : false; 


                    case HoldemType.OGS:
                        return (HandInstance.ogSkinsManager.OGSKINS.ContainsKey(name)) ? true : false;

                    case HoldemType.IC:
                        return (HandInstance.IconCache.ContainsKey(name)) ? true : false;
                    case HoldemType.SC:
                        return (HandInstance.SpriteCache.ContainsKey(name)) ? true : false;

                }
                return false;
            }
            /// <summary>
            /// Danger: Purges an entire Dictionary and resets it,
            /// </summary>
            /// <param name="holdemType">The Type of Dictionary we are Changing</param>
            public static void Fold(HoldemType holdemType)
            {


                switch (holdemType)
                {
                    case HoldemType.ASC:
                        foreach (KeyValuePair<string, Texture> kvp in autoSwapCache)
                        {

                            string name = kvp.Key;
                            BatonPass.Debug("Deleting " + name + " from Holdem ASC");
                            Texture workingfile = autoSwapCache[name];
                            UnityEngine.Object.Destroy(workingfile);

                        }
                        autoSwapCache.Clear();
                        break;
                    case HoldemType.OGS:
                        foreach (KeyValuePair<string, Texture> kvp in HandInstance.ogSkinsManager.OGSKINS)
                        {

                            string name = kvp.Key;
                            BatonPass.Debug("Deleting " + name + " from Holdem ASC");
                            Texture workingfile = HandInstance.ogSkinsManager.OGSKINS[name];
                            UnityEngine.Object.Destroy(workingfile);

                        }
                        HandInstance.ogSkinsManager.OGSKINS.Clear();
                        break;
                    case HoldemType.IC:

                        HandInstance.IconCache.Clear();
                        break;
                    case HoldemType.SC:
                        foreach (KeyValuePair<string, Sprite> kvp in HandInstance.SpriteCache)
                        {

                            string name = kvp.Key;
                            BatonPass.Debug("Deleting " + name + " from Holdem SC");
                            Sprite workingfile = HandInstance.SpriteCache[name];
                            UnityEngine.Object.Destroy(workingfile);

                        }
                        HandInstance.SpriteCache.Clear();
                        break;


                }
            }

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
