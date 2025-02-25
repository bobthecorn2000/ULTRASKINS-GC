using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;

using HarmonyLib;

using Unity.Audio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 using PluginConfig.API;
using System.Reflection;

using PluginConfig.API.Fields;
using BepInEx.Logging;
using static UltraSkins.SkinEventHandler;
using PluginConfig.API.Functionals;
using System.Net.NetworkInformation;
using HarmonyLib.Tools;
using static MonoMod.RuntimeDetour.Platforms.DetourNativeMonoPosixPlatform;
using static UnityEngine.ParticleSystem.PlaybackState;






namespace UltraSkins
{
   [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
   [BepInDependency(PluginConfig.PluginConfiguratorController.PLUGIN_GUID, "1.7.0")]
    
    public class ULTRASKINHand : BaseUnityPlugin
    {
        public PluginConfigurator config;
        public const string PLUGIN_NAME = "UltraSkins";
        public const string PLUGIN_GUID = "ultrakill.UltraSkins.bobthecorn";
        public const string PLUGIN_VERSION = "6.0.0";
        private string modFolderPath;

        
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


        public string pluginPath;
        public string folderupdater;
        public static Dictionary<string, Texture> autoSwapCache = new Dictionary<string, Texture>();
		public string[] directories;
		public string serializedSet = "";
        public bool swapped = false;
        Harmony UKSHarmony;
        AssetBundle bundle0;
        static Shader CCE;
        static Shader DE;
        static Cubemap cubemap;
        //public void Start(SkinEventHandler skinEventHandler)
        //{
        //    string modFolderPath = skinEventHandler.GetModFolderPath();
            
        //    LoadTextures(modFolderPath);
            
        //}


            private void Awake()
        {
            //System.Diagnostics.Debugger.Break();
            Logger.LogInfo("Attempting to start");
            config = PluginConfigurator.Create("Ultraskins", "ultrakill.ultraskins.bobthecorn");
            Logger.LogInfo("Created PluginConfig");

            pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Logger.LogInfo("Plugin path set to " + pluginPath);
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string skinfolderdir = Path.Combine(appDataPath, AppDataLoc);

            Logger.LogInfo("Setting appdatapath, appdataloc and SkinFolderDir to " + "\nAPPDATAPATH:" + appDataPath + "\nAPPDATALOC:" + AppDataLoc + "\nSKINFOLDERDIR:"+ skinfolderdir);
            try {
                Logger.LogInfo("Looking for Subfolders");
                string[] subfolders = Directory.GetDirectories(skinfolderdir);
                Logger.LogInfo("Done");
                Dictionary<string, ButtonField> objects = new Dictionary<string, ButtonField>();
                ConfigPanel skinfolders = new ConfigPanel(config.rootPanel, "skinfolders", "skinfolders");
                foreach (string subfolder in subfolders)
                {
                    
                    string folder = Path.GetFileName(subfolder);
                    Logger.LogInfo("SubFolder: " + folder);
                    folderupdater = folder;
                    var button = new ButtonField(skinfolders, folder, folder);
                    Logger.LogInfo("Making the button");
                    objects[subfolder] = button;
                    
                    button.onClick += () => refreshskins(skinfolders, button.guid);
                    Logger.LogInfo("OnClick Ready");
                }
            }
            catch
            {
                Logger.LogError("if your seeing this restart the game things need to finish setting up, if you keep seeing this message something is wrong with your folder setup, Error -\"USHAND-AWAKE\"");
                new ButtonField(config.rootPanel, "if your seeing this restart the game things need to finish setting up, if you keep seeing this message something is wrong with your folder setup, Error-\"USHAND-AWAKE\"","USHAND-AWAKE" );
            }
            Logger.LogInfo("Finishing setup");

            SceneManager.activeSceneChanged += SceneManagerOnsceneLoaded;
            Logger.LogInfo("Scenemanager Created");

            config.SetIconWithURL("https://github.com/bobthecorn2000/ULTRASKINS-GC/blob/main/UltraSkins/icon.png?raw=true");
            Logger.LogInfo("Setting Config icon");
            BoolField debuggermode = new BoolField(config.rootPanel, "Debug Mode", "DebugMode", false);
            Logger.LogInfo("Created debug button");
            // The GUID of the configurator must not be changed as it is used to locate the config file path
            Logger.LogInfo("INIT BATON PASS: ONMODLOADED()");
            OnModLoaded(skinfolderdir);

        }
        public void OnModLoaded(string fpath){
            //Harmony.DEBUG = true;
           // HarmonyFileLog.Enabled = true;
            UKSHarmony = new Harmony("Gcorn.UltraSkins");
            UKSHarmony.PatchAll(typeof(HarmonyGunPatcher));
            UKSHarmony.PatchAll(typeof(HarmonyProjectilePatcher));
            UKSHarmony.PatchAll();
            Logger.LogInfo("BATON PASS: Welcome To Ultraskins, We are on the ONMODLOADED() STEP");
            refreshskins();

            try
            {
                Logger.LogInfo("Creating the SkinEvent Handler");
                SkinEventHandler skinEventHandler = new SkinEventHandler();
                Logger.LogInfo("INIT BATON PASS: GETMODFOLDERPATH()");
                string modFolderPath = skinEventHandler.GetModFolderPath();
                Logger.LogInfo("BATON PASS: WELCOME BACK TO ONMODLOADED() WE RECIEVED " + modFolderPath);
                //LoadTextures("C:\\Users\\andrew fox\\AppData\\Roaming\\bobthecorn2000\\ULTRAKILL\\ultraskinsGC\\BrennanSet");

            }
            catch (Exception ex)
            {
                FileLog.Log("Hear Ye Hear Ye, ULTRASKINS HAS FAILED  Error -\"USHAND-ONMODLOADED\"" + ex.Message);
                FileLog.Log("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.ToString());
                Logger.LogError("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.ToString());
                Logger.LogError("Hear Ye Hear Ye, ULTRASKINS HAS FAILED Error -\"USHAND-ONMODLOADED\"" + ex.Message);
                ButtonField exmessage = new ButtonField(config.rootPanel, ex.Message, ex.Message);
                ButtonField exerror = new ButtonField(config.rootPanel, ex.ToString(), ex.ToString());

            }



        }

         void refreshskins(ConfigPanel folderpage, string clickedButton)
        {
            Logger.LogInfo("BATON PASS: WE ARE IN REFRESHSKINS() WE RECIEVED AND HAVE THE CURRENT VARIABLES \n folderpage " + folderpage + "\n clickedButton " + clickedButton);
            Debug.Log("pannel close:" + clickedButton);
            Logger.LogInfo("Panal closed for " + clickedButton);
            StringSerializer serializer = new StringSerializer();
            Logger.LogInfo("Created The Serializer");
            Logger.LogInfo("looking for the dll, appdata paths and merging the directory strings");
            string dlllocation = Assembly.GetExecutingAssembly().Location.ToString();
            
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);

            string filepath = Path.Combine(dir + "\\" + clickedButton);
            Logger.LogInfo("Done, The folder is " + filepath);

            Debug.Log("folderis: " + filepath);
            serializer.SerializeStringToFile(filepath, Path.Combine(dir + "\\data.USGC"));
            Logger.LogInfo("Saved to data.USGC");
            Logger.LogInfo("INIT BATON PASS: RELOADTEXTURES(TRUE,"+ filepath +")");
            ReloadTextures(true, filepath);
            Logger.LogInfo("BATON PASS: WELCOME BACK TO REFRESHSKINS()");
            Logger.LogInfo("Closing panel");
            folderpage.ClosePanel();
            //LoadTextures(filepath);

        }
        void refreshskins()
        {

            Logger.LogInfo("BATON PASS: WE ARE IN REFRESHSKINS(), THERE ARE NO OTHER ARGUMENTS");
            StringSerializer serializer = new StringSerializer();
            Logger.LogInfo("Created The Serializer");
            Logger.LogInfo("looking for the dll, appdata paths and merging the directory strings");
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);



            string filepath = serializer.DeserializeStringFromFile(Path.Combine(dir + "\\data.USGC"));
            Logger.LogInfo("Read data.USGC from " + filepath);
            Logger.LogInfo("INIT BATON PASS: RELOADTEXTURES(TRUE," + filepath + ")");
            ReloadTextures(true, filepath);

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

   
            

            [HarmonyPatch(typeof(GunControl), "UpdateWeaponList", new Type[] {typeof(bool)})]
            [HarmonyPostfix]
            public static void UpdateWeaponListPost(GunControl __instance, bool firstTime = false)
            {
                InitOWGameObjects(true);
                TextureOverWatch[] TOWS = CameraController.Instance.gameObject.GetComponentsInChildren<TextureOverWatch>(true);
                ReloadTextureOverWatch(TOWS);
            }
            [HarmonyPatch(typeof(ShotgunHammer))]
            public static class UpdateMeterPatch
            {
                static FieldInfo meterEmissivesField;
                static FieldInfo meterEmissivesMaskField;

                static UpdateMeterPatch()
                {
                    meterEmissivesField = typeof(ShotgunHammer).GetField("meterEmissives", BindingFlags.NonPublic | BindingFlags.Instance);
                    meterEmissivesMaskField = typeof(ShotgunHammer).GetField("secondaryMeter", BindingFlags.NonPublic | BindingFlags.Instance);
                }

                [HarmonyPrefix]
                [HarmonyPatch("OnEnable")]
                public static void PrefixUpdateMeter(ShotgunHammer __instance)
                {
                    if (meterEmissivesField == null)
                    {
                        Debug.LogError("Failed to find 'meterEmissives' field.");
                        return;
                    }
                    
                    var meterEmissives = (Texture[])meterEmissivesField.GetValue(__instance);
                    var meterMask = (Image)meterEmissivesMaskField.GetValue(__instance);
                    Texture glow1;
                    Texture glow2;
                    Texture glow3;
                    if (autoSwapCache.ContainsKey("T_DialGlow1")) {
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
                    meterEmissivesField.SetValue(__instance, meterEmissives);
                    if (autoSwapCache.ContainsKey("T_DialMask"))
                    {
                        Texture dialmask = autoSwapCache["T_DialMask"];
                        
                        Sprite masksprite = Sprite.Create((Texture2D)dialmask, new Rect(0, 0, dialmask.width, dialmask.height), new Vector2(0.5f, 0.5f));
                        meterMask.sprite = masksprite;
                    }



                    Debug.Log($"[UpdateMeter] Current Tier: {__instance}");
                    for (int i = 0; i < meterEmissives.Length; i++)
                    {
                        Debug.Log($"[UpdateMeter] Texture at index {i}: {meterEmissives[i]?.name}");
                    }
                }

                private static Texture2D CreateSolidColorTexture(UnityEngine.Color color)
                {
                    var texture = new Texture2D(128, 128);
                    var pixels = new UnityEngine.Color[128 * 128];
                    for (int i = 0; i < pixels.Length; i++)
                    {
                        pixels[i] = color;
                    }
                    texture.SetPixels(pixels);
                    texture.Apply();
                    return texture;
                }
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
                            if(DW)
                            {
                                Renderer[] renderers = DW.GetComponentsInChildren<Renderer>(true);
                                foreach(Renderer renderer in renderers)
                                {
                                    if(renderer && renderer.gameObject.layer == 13 && !renderer.gameObject.GetComponent<ParticleSystemRenderer>() && !renderer.gameObject.GetComponent<CanvasRenderer>())
                                    {
                                        if(!renderer.gameObject.GetComponent<TextureOverWatch>())
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
               if(__instance.sawblade)
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

            public static void AddTOWs(GameObject gameobject, bool toself = true, bool tochildren = false , bool toparent = false, bool refresh = false)
            {
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

        private void Start()
        {
            
        }

		private void SceneManagerOnsceneLoaded(Scene scene, Scene mode)
		{
            Logger.LogInfo("BATON PASS: WE ARE IN SceneManagerOnsceneLoaded()");
            swapped = false;

            Logger.LogInfo("Checking for Null CCE");
            if (CCE == null)
            {
                Logger.LogInfo("ITS NULL, CORRECTING");
                CCE = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Special/ULTRAKILL-vertexlit-customcolors-emissive.shader").WaitForCompletion();
            }
            Logger.LogInfo("Checking for Null DE");
            if (DE == null)
            {
                Logger.LogInfo("ITS NULL, CORRECTING");
                DE = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit-emissive.shader").WaitForCompletion();
            }

            //DE = Addressables.LoadAssetAsync<Shader>("psx/vertexlit/emissive").WaitForCompletion();
            Logger.LogInfo("Checking for Null CUBEMAP");
            if (cubemap == null)
            {
                Logger.LogInfo("ITS NULL, CORRECTING");
                cubemap = Addressables.LoadAssetAsync<Cubemap>("Assets/Textures/studio_06.exr").WaitForCompletion();
            }
            //CreateSkinGUI();
            Logger.LogInfo("INIT BATON PASS: REFRESHSKINS()");
            refreshskins();
        }

        


        public static Texture ResolveTheTextureProperty(Material mat, string property, string propertyfallback = "_MainTex")
        {
           
            if (mat != null && mat.mainTexture == null)
                return null;
            if (DE == null)
                DE = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit-emissive.shader").WaitForCompletion();
            mat.EnableKeyword("EMISSIVE");
            string textureToResolve = "";
            if (mat && !mat.mainTexture.name.StartsWith("TNR_") && property != "_Cube")
            {
                switch (property)
                {
                    case "_MainTex":
                        textureToResolve = mat.mainTexture.name;
                        break;
                    case "_EmissiveTex":
                        switch (mat.mainTexture.name)
                        {
                            case "T_NailgunNew_NoGlow":
                                textureToResolve = "T_Nailgun_New_Glow";
                                break;
                            case "T_RocketLauncher_Desaturated":
                                textureToResolve = "T_RocketLauncher_Emissive";
                                break;
                            case "T_ImpactHammer":
                                textureToResolve = "T_ImpactHammer_Glow";
                                break;
                            default:
                                textureToResolve = mat.mainTexture.name + "_Emissive";
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
                        textureToResolve = (mat.name.Contains("Swapped_AltarUnlitRed") && !mat.mainTexture.name.StartsWith("T_")) ? "skull2rocketbonus" : mat.mainTexture.name.Contains("T_Sakuya") ? "" : "skull2rocket";
                        break;
                    case "THROWITBACK":
                        textureToResolve = "skull2 compressed";
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

        public static void PerformTheSwap(Material mat, bool forceswap = false, TextureOverWatch TOW = null, string swapType = "weapon")
        {
            if (mat && (!mat.name.StartsWith("Swapped_")|| forceswap))
            {

                if (!mat.name.StartsWith("Swapped_"))
                {
                    mat.name = "Swapped_" + mat.name;
                }
                if (mat.shader.name == "psx/vertexlit/vertexlit-customcolors" && CCE)
                {
                    mat.shader = CCE;
                }
                else if (mat.shader.name == "psx/vertexlit/vertexlit")
                {
                    mat.shader = DE;
                }
                forceswap = false;
                Texture resolvedTexture = new Texture();
                if (swapType == "weapon")
                {
                    
                    string[] textureProperties = mat.GetTexturePropertyNames();
                    foreach (string property in textureProperties)
                    {
                        
                        //Debug.Log("Attempting to swap " + property + " of " + mat.name.ToString());
                        
                        
                        resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, property, property);
                        if (resolvedTexture && resolvedTexture != null && mat.HasProperty(property) && mat.GetTexture(property) != resolvedTexture)
                        {
                            //Debug.Log("swapping " + property + " of " + mat.name.ToString());
                            mat.SetTexture(property, resolvedTexture);
                        }

                        if (TOW != null && mat.HasProperty("_EmissiveColor"))
                        {
                            
                            if (mat.name.ToString() == "Swapped_ImpactHammerDial (Instance)")
                            {
                                break;
                            }
                            else { 
                            //Debug.Log("swapping " + property + " of " + mat.name.ToString());
                            Color VariantColor = GetVarationColor(TOW);
                            Color VariantColor2 = new Color(255, 255, 255, 255);
                            mat.SetColor("_EmissiveColor", VariantColor2);
                            }
                        }
                        
                    }
                }
                else if (swapType == "projectile")
                {
                    resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, "_MainTex");
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {
                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }
                else if (swapType == "grenade")
                {
                    resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, "THROWITBACK");
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {
                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }
                else if (swapType == "rocket")
                {
                    resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, "ROCKIT");
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {
                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
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

        public string ReloadTextures(bool firsttime = false, string path = "")
        {
            Logger.LogInfo("BATON PASS: WE ARE IN ReloadTextures() We have variables \n FIRSTTIME:" + firsttime + "\n PATH:" + path);
            Logger.LogInfo("Start Comparing");
            if (firsttime && serializedSet != "")
            {
                Logger.LogInfo("SerializedSet is not empty, and firsttime is true - path will equal serialized set");
                path = serializedSet;
            }
            else if (firsttime && serializedSet == "")
            {
                Logger.LogInfo("SerializedSet is empty, and firsttime is true - path should equal path");
                path = path;
            }
            if (path == "")
            {
                Logger.LogInfo("path is empty - path should equal path");
                path = path;
            }
            Logger.LogInfo("INIT BATON PASS: INITOWGAMEOBJECTS(" + firsttime +")");
            InitOWGameObjects(firsttime);
            Logger.LogInfo("BATON PASS: WELCOME BACK TO RELOADTEXTURES()");
            Logger.LogInfo("INIT BATON PASS: LOADTEXTURES(" + path + ")");
            return LoadTextures(path);
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

		public string LoadTextures(string fpath = "")
		{
            Logger.LogInfo("BATON PASS: WE ARE IN LOADTEXTURES() we have the variable FPATH, " + fpath);
            try
            {
                FileLog.Log("ULTRASKINS IS SEARCHING " + fpath.ToString());
                Debug.Log("ULTRASKINS IS SEARCHING " + fpath.ToString());
                Logger.LogInfo("ULTRASKINS IS SEARCHING " + fpath.ToString());
                autoSwapCache.Clear();
                bool failed = false;
                DirectoryInfo dir = new DirectoryInfo(fpath);
                if (!dir.Exists)
                    return "failed";
                FileInfo[] Files = dir.GetFiles("*.png");
                Logger.LogInfo("Begining loop");
                if (Files.Length > 0)
                {
                    foreach (FileInfo file in Files)
                    {
                        if (file.Exists)
                        {
                            byte[] data = File.ReadAllBytes(file.FullName);
                            string name = Path.GetFileNameWithoutExtension(file.FullName);

                            Logger.LogInfo("Reading " + file.FullName.ToString());

                            Texture2D texture2D = new Texture2D(2, 2);
                            texture2D.name = name;

                            Logger.LogInfo("creating " + texture2D.name.ToString());

                            texture2D.filterMode = FilterMode.Point;
                            texture2D.LoadImage(data);
                            Logger.LogInfo("Loading Image Data");
                            texture2D.Apply();
                            if (file.Name == "Railgun_Main_AlphaGlow.png")
                            {
                                Logger.LogInfo("Its the railgun");

                                Texture2D texture2D2 = new Texture2D(2, 2);
                                byte[] data2 = File.ReadAllBytes(Path.Combine(file.DirectoryName, "Railgun_Main_Emissive.png"));
                                texture2D2.filterMode = FilterMode.Point;
                                texture2D2.LoadImage(data2);
                                texture2D2.Apply();

                                Logger.LogInfo("Applying Changes");

                                Color[] pixels = texture2D.GetPixels();
                                Color[] pixels2 = texture2D2.GetPixels();
                                for (int k = 0; k < pixels.Length; k++)
                                {
                                    pixels[k].a = pixels2[k].r;
                                }
                                texture2D.SetPixels(pixels);
                                Logger.LogInfo("Setting Colors");
                                texture2D.Apply();
                                Logger.LogInfo("Applying Colors");
                            }
                            Texture texture = new Texture();
                            Logger.LogInfo("Setting texture ");
                            texture = texture2D;
                            Logger.LogInfo("Setting Variable " + texture.name);
                            autoSwapCache.Add(name, texture);
                            Logger.LogInfo("Adding to Cache " + texture.name + " " + name);
                        }
                        else
                        {
                            Logger.LogError("HEAR YE HEAR YE, They got away, Error -\"USHAND-LOADTEXTURES-CACHEFAIL\"");
                            
                            failed = true;
                        }
                    }
                    if (!failed)
                    {
                        
                        Logger.LogInfo("We Got the Textures ");
                        return "Successfully loaded all Textures from " + Path.GetFileName(fpath) + "!";
                    }
                }
                Logger.LogInfo("HEAR YE HEAR YE The length of the files is zero, Error-\"USHAND-LOADTEXTURES-0INDEX\"" + Files.Length.ToString() + "\n" + Files.ToString());
                return "Failed to load all textures from " + Path.GetFileName(fpath) + ".\nPlease ensure all of the Texture Files names are Correct, refer to the README file for the correct names and more info.";
            }
            catch (Exception ex)
            {
                Debug.Log("HEAR YE HEAR YE, The Search Was Fruitless, Error-\\\"USHAND-LOADTEXTURES-EX\\\"\"" + ex.Message);
                Debug.Log("HEAR YE HEAR YE, The Search Was Fruitless, Error-\\\"USHAND-LOADTEXTURES-EX\\\"\"" + ex.ToString());
                Logger.LogError("HEAR YE HEAR YE, The Search Was Fruitless, Error-\\\"USHAND-LOADTEXTURES-EX\\\"\"" + ex.Message);
                Logger.LogError("HEAR YE HEAR YE, The Search Was Fruitless, Error-\\\"USHAND-LOADTEXTURES-EX\\\"\"" + ex.ToString());
                return "Failed to load all textures from " + Path.GetFileName(fpath) + ".\nPlease ensure all of the Texture Files names are Correct, refer to the README file for the correct names and more info.";

            }
        }
	}
}
