﻿using BepInEx;
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
using static UltraSkins.BPGUIManager;
using static UltraSkins.SkinEventHandler;
using static UnityEngine.UIElements.UIRAtlasAllocator;



//using UnityEngine.UIElements;






namespace UltraSkins
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]


    public class ULTRASKINHand : BaseUnityPlugin
    {


#if DEBUG
        public const bool ShouldDoBatonPass = true;
#else
    public const bool ShouldDoBatonPass = false;
#endif
        public const bool ShouldDoBatonPassUnity = false;

        public const string PLUGIN_NAME = "UltraSkins";
        public const string PLUGIN_GUID = "ultrakill.UltraSkins.bobthecorn";
        public const string PLUGIN_VERSION = "6.0.2";
        private string modFolderPath;
        public bool loadTextureLock = false;
        public static ManualLogSource BatonPassLogger = new ManualLogSource("BatonPass");
        private static string _modPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private static string _skinPath = Path.Combine(_modPath, "Custom");

        public List<string> Paths
        {
            get
            {
                string[] files = Directory.GetFiles(_skinPath);

                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = files[i].Split(Path.DirectorySeparatorChar).Last();
                }

                return files.ToList();
            }
        }

        private List<Sprite> _default;
        private List<Sprite> _edited;
        private bool firstmenu = true;

        public static string pluginPath;
        public string folderupdater;
        public static Dictionary<string, Texture> autoSwapCache = new Dictionary<string, Texture>();
        public Dictionary<string,string> MaterialNames = new Dictionary<string,string>();
        public string[] directories;
        public string serializedSet = "";
        public bool swapped = false;
        Harmony UKSHarmony;
        public static ULTRASKINHand HandInstance { get; private set; }

        //public void Start(SkinEventHandler skinEventHandler)
        //{
        //    string modFolderPath = skinEventHandler.GetModFolderPath();

        //    LoadTextures(modFolderPath);

        //}


        private void Awake()
        {
            BepInEx.Logging.Logger.Sources.Add(BatonPassLogger);
            if (HandInstance == null)
            {
                HandInstance = this;
            }
            else
            {
                Destroy(this);
            }
            BatonPass("Attempting to start");



            pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            BatonPass("Plugin path set to " + pluginPath);
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string skinfolderdir = Path.Combine(appDataPath, AppDataLoc);
                                                             
            BatonPass("Setting appdatapath, appdataloc and SkinFolderDir to " + "\nAPPDATAPATH:" + appDataPath + "\nAPPDATALOC:" + AppDataLoc + "\nSKINFOLDERDIR:" + skinfolderdir);
            BatonPass("Starting addressables");


            BatonPass("Looking for the Config");
            var catalog = Addressables.LoadContentCatalogAsync(Path.Combine(pluginPath, "catalog.json"), true).WaitForCompletion();


            //BatonPass($"Registered Addressable Keys:\n{string.Join("\n", keys)}");

            try
            {
                BatonPass("Looking for Subfolders");
                string[] subfolders = Directory.GetDirectories(skinfolderdir);
                BatonPass("Done");



            }
            catch
            {
                Logger.LogError("if your seeing this restart the game things need to finish setting up, if you keep seeing this message something is wrong with your folder setup, Error -\"USHAND-AWAKE\"");
                //new ButtonField(config.rootPanel, "if your seeing this restart the game things need to finish setting up, if you keep seeing this message something is wrong with your folder setup, Error-\"USHAND-AWAKE\"","USHAND-AWAKE" );
            }
            BatonPass("Finishing setup");

            SceneManager.activeSceneChanged += SceneManagerOnsceneLoaded;
            BatonPass("Scenemanager Created");

            // The GUID of the configurator must not be changed as it is used to locate the config file path
            BatonPass("INIT BATON PASS: ONMODLOADED()");
            OnModLoaded(skinfolderdir);

        }
        void Start()
        {
            if (BPGUIManager.BPGUIinstance == null)
            {
                GameObject managerObject = new GameObject("BPGUIManager");
                managerObject.AddComponent<BPGUIManager>();
            }
        }
        public void OnModLoaded(string fpath)
        {
            //Harmony.DEBUG = true;
            // HarmonyFileLog.Enabled = true;
            UKSHarmony = new Harmony("Gcorn.UltraSkins");
            UKSHarmony.PatchAll(typeof(HarmonyGunPatcher));
            UKSHarmony.PatchAll(typeof(HarmonyProjectilePatcher));
            UKSHarmony.PatchAll();
            BatonPass("BATON PASS: Welcome To Ultraskins, We are on the ONMODLOADED() STEP");


            try
            {
                BatonPass("Creating the SkinEvent Handler");
                SkinEventHandler skinEventHandler = new SkinEventHandler();
                BatonPass("INIT BATON PASS: GETMODFOLDERPATH()");
                string[] modFolderPath = skinEventHandler.GetModFolderPath();
                BatonPass("BATON PASS: WELCOME BACK TO ONMODLOADED() WE RECIEVED " + modFolderPath);
                //LoadTextures("C:\\Users\\andrew fox\\AppData\\Roaming\\bobthecorn2000\\ULTRAKILL\\ultraskinsGC\\BrennanSet");
                HandInstance.MaterialNames.Add("Swapped_weapon_GreenArm (Instance)(Clone) (Instance)", "T_GreenArm");
                HandInstance.MaterialNames.Add("Swapped_weapon_FeedbackerLit (Instance)(Clone) (Instance)", "T_Feedbacker");
                HandInstance.MaterialNames.Add("Swapped_weapon_RedArmLit (Instance)(Clone) (Instance)", "v2_armtex");
                HandInstance.MaterialNames.Add("Swapped_weapon_MainArmLit (Instance)(Clone) (Instance)", "T_MainArm");

            }
            catch (Exception ex)
            {
                FileLog.Log("Hear Ye Hear Ye, ULTRASKINS HAS FAILED  Error -\"USHAND-ONMODLOADED\"" + ex.Message);
                FileLog.Log("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.ToString());
                Logger.LogError("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.ToString());
                Logger.LogError("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.Message);


            }
            //refreshskins();


        }

        public void refreshskins(string[] clickedButtons)
        {
            BatonPass("BATON PASS: WE ARE IN REFRESHSKINS()");

            StringSerializer serializer = new StringSerializer();
            BatonPass("Created The Serializer");
            BatonPass("looking for the dll, appdata paths and merging the directory strings");
            string dlllocation = Assembly.GetExecutingAssembly().Location.ToString();

            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);

            List<string> filepath = new List<string>();
            foreach (string clickedButton in clickedButtons)
            {
                filepath.Add(Path.Combine(dir + "\\" + clickedButton));
                BatonPass("added " + dir + "\\" + clickedButton);
            }
            string[] filepathArray = filepath.ToArray();
            foreach (string thing in filepathArray)
            {
                BatonPass(thing);
            }

            Debug.Log("folderis: " + filepath);
            serializer.SerializeStringToFile(filepathArray, Path.Combine(dir + "\\data.USGC"));
            BatonPass("Saved to data.USGC");
            BatonPass("INIT BATON PASS: RELOADTEXTURES(TRUE," + filepath + ")");
            
            ReloadTextures(filepathArray,false);
            
            BatonPass("BATON PASS: WELCOME BACK TO REFRESHSKINS()");
            BatonPass("Closing panel");

            //LoadTextures(filepath);

        }
        void refreshskins()
        {

            BatonPass("BATON PASS: WE ARE IN REFRESHSKINS(), THERE ARE NO OTHER ARGUMENTS");
            StringSerializer serializer = new StringSerializer();
            BatonPass("Created The Serializer");
            BatonPass("looking for the dll, appdata paths and merging the directory strings");
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);



            string[] filepath = serializer.DeserializeStringFromFile(Path.Combine(dir + "\\data.USGC"));
            BatonPass("Read data.USGC from " + filepath);
            BatonPass("INIT BATON PASS: RELOADTEXTURES(TRUE," + filepath + ")");
            ReloadTextures(filepath,true);

            //LoadTextures(filepath);

        }

        [HarmonyPatch]
        public class HarmonyGunPatcher
        {
            [HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] { typeof(int), typeof(int?), typeof(bool), typeof(bool), typeof(bool) })]
            [HarmonyPostfix]
            public static void SwitchWeaponPost(GunControl __instance, int targetSlotIndex, int? targetVariationIndex = null, bool useRetainedVariation = false, bool cycleSlot = false, bool cycleVariation = false)
            {

                TextureOverWatch[] TOWS = __instance.currentWeapon.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
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

                TextureOverWatch[] TOWS = __instance.currentArmObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
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
                AddTOWs(__instance.gameObject, true);
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
                BatonPass("added " + gameobject.name + "to textureoverwatch");
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




        private void SceneManagerOnsceneLoaded(Scene scene, Scene mode)
        {
            BatonPass("BATON PASS: WE ARE IN SceneManagerOnsceneLoaded()");



            swapped = false;



            BatonPass("Checking for Null CCE");

            //CreateSkinGUI();
            BatonPass("The Scene is: " + mode.name);
            BatonPass("INIT BATON PASS: REFRESHSKINS()");
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
                BatonPass("Cant make menu, currently straping my boots");
            }
            else { MenuCreator.makethePausemenu(); }

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
                                if (autoSwapCache.ContainsKey(textureToResolve))
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
                                if (autoSwapCache.ContainsKey(textureToResolve))
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
                if (textureToResolve != "" && autoSwapCache.ContainsKey(textureToResolve))
                    return autoSwapCache[textureToResolve];
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
                BatonPass("I should change" + TOW.iChange);
                BatonPass("requested " + mat.name + " got " + texturename);

                if (swapType == "weapon")
                {

                    string[] textureProperties = mat.GetTexturePropertyNames();

                    foreach (string property in textureProperties)
                    {




                        resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, property, texturename, property);
                        //BatonPass("Attempting to swap " + property + " of " + mat.name.ToString() + " with " + resolvedTexture.name.ToString());
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


        public static void SwapTheDial(TextureOverWatch TOW)
        {
            FieldInfo meterEmissivesField = typeof(ShotgunHammer).GetField("meterEmissives", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo meterEmissivesMaskField = typeof(ShotgunHammer).GetField("secondaryMeter", BindingFlags.NonPublic | BindingFlags.Instance);

            if (meterEmissivesField == null)
            {
                Debug.LogError("Failed to find 'meterEmissives' field.");
                return;
            }
            ShotgunHammer shotgunHammer = TOW.GetComponentInParent<ShotgunHammer>();
            var meterEmissives = (Texture[])meterEmissivesField.GetValue(shotgunHammer);
            var meterMask = (Image)meterEmissivesMaskField.GetValue(shotgunHammer);
            Texture glow1;
            Texture glow2;
            Texture glow3;
            if (autoSwapCache.ContainsKey("T_DialGlow1"))
            {
                glow1 = autoSwapCache["T_DialGlow1"];
            }
            else
            {
                glow1 = meterEmissives[0];
            }
            if (autoSwapCache.ContainsKey("T_DialGlow2"))
            {
                glow2 = autoSwapCache["T_DialGlow2"];
            }
            else
            {
                glow2 = meterEmissives[1];
            }
            if (autoSwapCache.ContainsKey("T_DialGlow3"))
            {
                glow3 = autoSwapCache["T_DialGlow3"];
            }
            else
            {
                glow3 = meterEmissives[2];
            }
            meterEmissives = new Texture[3]
                {
                        glow1,
                        glow2,
                        glow3,
                };
            meterEmissivesField.SetValue(shotgunHammer, meterEmissives);
            if (autoSwapCache.ContainsKey("T_DialMask"))
            {
                Texture dialmask = autoSwapCache["T_DialMask"];

                Sprite masksprite = Sprite.Create((Texture2D)dialmask, new Rect(0, 0, dialmask.width, dialmask.height), new Vector2(0.5f, 0.5f));
                meterMask.sprite = masksprite;
            }



            Debug.Log($"[UpdateMeter] Current Tier: {shotgunHammer}");
            for (int i = 0; i < meterEmissives.Length; i++)
            {
                Debug.Log($"[UpdateMeter] Texture at index {i}: {meterEmissives[i]?.name}");
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
            BatonPass("BATON PASS: WE ARE IN ReloadTextures() We have variables \n FIRSTTIME:" + firsttime + "\n PATH:" + path);
            BatonPass("Start Comparing");

#pragma warning restore CS1717 // Assignment made to same variable
            BatonPass("INIT BATON PASS: INITOWGAMEOBJECTS(" + firsttime + ")");

            BatonPass("BATON PASS: WELCOME BACK TO RELOADTEXTURES()");
            BatonPass("INIT BATON PASS: LOADTEXTURES(" + path + ")");
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
                BatonPass($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
/*            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
                        foreach (var tex in textures)
                        {
                            if (!AssetInUse(tex))
                            {
                                BatonPass($"Unused Texture: {tex.name} - Potential leak");
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

        }
        
        public async Task LoadTextures(string[] fpaths, bool firsttime = false)
        {
            BatonPass("BATON PASS: WE ARE IN LOADTEXTURES() we have the variable FPATH");
            if (firsttime == false)
            {
                BPGUI.ShowGUI("Loading");

            }

            foreach (string fpath in fpaths)
            {

                try
                {
                    FileLog.Log("ULTRASKINS IS SEARCHING " + fpath.ToString());
                    Debug.Log("ULTRASKINS IS SEARCHING " + fpath.ToString());
                    BatonPass("ULTRASKINS IS SEARCHING " + fpath.ToString());


                    //autoSwapCache.Clear();
                    bool failed = false;

                    DirectoryInfo dir = new DirectoryInfo(fpath);

                    if (!dir.Exists)
                    {
                        BPGUI.BatonPassAnnoucement(Color.red, "failed");
                        return; // Exit early if the directory doesn't exist
                    }

                    FileInfo[] Files = dir.GetFiles("*.png");
                    BatonPass("Beginning loop");

                    if (Files.Length > 0)
                    {
                        int totalFiles = Files.Length;
                        int filesLoaded = 0;
                        BPGUI.EnableTerminal(10);
                        BPGUI.ShowProgressBar();

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

                                    BatonPass("Reading " + file.FullName.ToString());

                                    Texture2D texture2D = new Texture2D(2, 2);
                                    texture2D.name = name;

                                    BatonPass("Creating " + texture2D.name);
                                    BPGUI.AddTermLine("Creating " + texture2D.name);
                                    texture2D.filterMode = FilterMode.Point;
                                    texture2D.LoadImage(data);
                                    BatonPass("Loading Image Data");
                                    texture2D.Apply();




                                    // Cache the texture
                                    Texture texture = texture2D;
                                    BatonPass("Setting texture");
                                    autoSwapCache.Add(name, texture);
                                    BatonPass("Adding to Cache " + texture.name + " " + name);

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
                                Logger.LogError("HEAR YE HEAR YE, They got away, Error -\"USHAND-LOADTEXTURES-CACHEFAIL\"");
                                failed = true;
                                BPGUI.DisableTerminal();
                                BPGUI.BatonPassAnnoucement(Color.red, "failed");
                            }

                            // Update progress (assuming a method for progress update exists, like updating a progress bar in UI)
                            filesLoaded++;
                            float progress = (float)filesLoaded / totalFiles;

                            progress = progress * 100;

                            BatonPass($"Progress: {progress:F2}%");
                            BPGUI.updatebar(progress);


                        }

                        if (!failed)
                        {
                            BatonPass("We Got the Textures ");
                            BPGUI.DisableTerminal();
                            BPGUI.HideProgressBar();
                            BPGUI.BatonPassAnnoucement(Color.green, "success");
                        }
                    }
                    else
                    {
                        BatonPass("HEAR YE HEAR YE The length of the files is zero, Error-\"USHAND-LOADTEXTURES-0INDEX\"");
                        BPGUI.DisableTerminal();
                        BPGUI.HideProgressBar();
                        BPGUI.BatonPassAnnoucement(Color.red, "No files found in " + fpath);
                    }


                }
                catch (Exception ex)
                {
                    Debug.Log("HEAR YE HEAR YE, The Search Was Fruitless, Error-\"USHAND-LOADTEXTURES-EX\" " + ex.Message);
                    Debug.Log("HEAR YE HEAR YE, The Search Was Fruitless, Error-\"USHAND-LOADTEXTURES-EX\" " + ex.ToString());
                    Logger.LogError("HEAR YE HEAR YE, The Search Was Fruitless, Error-\"USHAND-LOADTEXTURES-EX\" " + ex.Message);
                    Logger.LogError("HEAR YE HEAR YE, The Search Was Fruitless, Error-\"USHAND-LOADTEXTURES-EX\" " + ex.ToString());
                    BPGUI.BatonPassAnnoucement(Color.red, "Failed to load all textures from " + Path.GetFileName(fpath) + ".\nPlease ensure all of the Texture Files names are Correct, refer to the README file for the correct names and more info.");
                }
            }
            if (firsttime == false)
            {
                BPGUI.HideGUI(2);
            }
        }

        //Baton Pass handles debug logging for several different types of debuggers


        public static void BatonPass(string message)
        {
            if (ShouldDoBatonPass == true)
            {
                BatonPassLogger.LogInfo(message);
                if (ShouldDoBatonPassUnity == true)
                {
                    Debug.Log(message);
                }
            }
        }
    }
}
