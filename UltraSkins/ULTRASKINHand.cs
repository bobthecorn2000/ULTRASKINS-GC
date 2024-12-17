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





namespace UltraSkins
{
   [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
   [BepInDependency(PluginConfig.PluginConfiguratorController.PLUGIN_GUID, "1.7.0")]
    
    public class ULTRASKINHand : BaseUnityPlugin
    {
        public PluginConfigurator config;
        public const string PLUGIN_NAME = "UltraSkins";
        public const string PLUGIN_GUID = "ultrakill.UltraSkins.bobthecorn";
        public const string PLUGIN_VERSION = "5.1.1";
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
            System.Diagnostics.Debugger.Break();
            config = PluginConfigurator.Create("Ultraskins", "ultrakill.ultraskins.bobthecorn");
            
            
            pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string skinfolderdir = Path.Combine(appDataPath, AppDataLoc);
            try { 
            string[] subfolders = Directory.GetDirectories(skinfolderdir);
                Dictionary<string, ButtonField> objects = new Dictionary<string, ButtonField>();
                ConfigPanel skinfolders = new ConfigPanel(config.rootPanel, "skinfolders", "skinfolders");
                foreach (string subfolder in subfolders)
                {
                    string folder = Path.GetFileName(subfolder);
                    folderupdater = folder;
                    var button = new ButtonField(skinfolders, folder, folder);
                    objects[subfolder] = button;

                    button.onClick += () => refreshskins(skinfolders, button.guid);

                }
            }
            catch
            {
                new ButtonField(config.rootPanel, "if your seeing this restart the game things need to finish setting up, if you keep seeing this message something is wrong with your folder setup, Error-\"USHAND-AWAKE\"","USHAND-AWAKE" );
            }
            
            
                SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
            
            config.SetIconWithURL("https://github.com/bobthecorn2000/ULTRASKINS-GC/blob/main/UltraSkins/icon.png?raw=true");
            BoolField debuggermode = new BoolField(config.rootPanel, "Debug Mode", "DebugMode", false);
            
            // The GUID of the configurator must not be changed as it is used to locate the config file path
            OnModLoaded();

        }
        public void OnModLoaded(){

            UKSHarmony = new Harmony("Gcorn.UltraSkins");
            UKSHarmony.PatchAll(typeof(HarmonyGunPatcher));
            UKSHarmony.PatchAll(typeof(HarmonyProjectilePatcher));
            UKSHarmony.PatchAll();
            try
            {
                SkinEventHandler skinEventHandler = new SkinEventHandler();
                string modFolderPath = skinEventHandler.GetModFolderPath();
                
            }
            catch (Exception ex)
            {
                ButtonField exmessage = new ButtonField(config.rootPanel, ex.Message, ex.Message);
                ButtonField exerror = new ButtonField(config.rootPanel, ex.ToString(), ex.ToString());
            }


            

        }

         void refreshskins(ConfigPanel folderpage, string clickedButton)
        {
            
            Debug.Log("pannel close:" + clickedButton);
            StringSerializer serializer = new StringSerializer();
            string dlllocation = Assembly.GetExecutingAssembly().Location.ToString();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);

            string filepath = Path.Combine(dir + "\\" + clickedButton);
            Debug.Log("folderis: " + filepath);
            serializer.SerializeStringToFile(filepath, Path.Combine(dir + "\\data.USGC"));
            ReloadTextures(true, filepath);
            folderpage.ClosePanel();
            //LoadTextures(filepath);

        }
        void refreshskins()
        {

            
            StringSerializer serializer = new StringSerializer();
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string AppDataLoc = "bobthecorn2000\\ULTRAKILL\\ultraskinsGC";
            string dir = Path.Combine(appDataPath, AppDataLoc);



            string filepath = serializer.DeserializeStringFromFile(Path.Combine(dir + "\\data.USGC"));
            ReloadTextures(true, filepath);

            //LoadTextures(filepath);

        }

        [HarmonyPatch]
        public class HarmonyGunPatcher
        {
            [HarmonyPatch(typeof(GunControl), "SwitchWeapon", new Type[] { typeof(int), typeof(List<GameObject>), typeof(bool), typeof(bool), typeof(bool), typeof(bool) })]
            [HarmonyPostfix]
            public static void SwitchWeaponPost(GunControl __instance, int target, List<GameObject> slot, bool lastUsedSlot = false, bool useRetainedVariation = false, bool scrolled = false, bool isNextVarBind = false)
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

		private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
		{
			swapped = false;
            if (CCE == null)
			    CCE = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Special/ULTRAKILL-vertexlit-customcolors-emissive.shader").WaitForCompletion();
			if (DE == null)
                DE = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit-emissive.shader").WaitForCompletion();
                //DE = Addressables.LoadAssetAsync<Shader>("psx/vertexlit/emissive").WaitForCompletion();
            if (cubemap == null)
                cubemap = Addressables.LoadAssetAsync<Cubemap>("Assets/Textures/studio_06.exr").WaitForCompletion();
            //CreateSkinGUI();
            refreshskins();
        }

        


        public static Texture ResolveTheTextureProperty(Material mat, string property, string propertyfallback = "_MainTex")
        {
            if (mat != null && mat.mainTexture == null)
                return null;
            if (DE == null)
                DE = Addressables.LoadAssetAsync<Shader>("Assets/Shaders/Main/ULTRAKILL-vertexlit-emissive.shader").WaitForCompletion();
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
                        
                        Debug.Log("Attempting to swap " + property + " of " + mat.name.ToString());
                        
                        
                        resolvedTexture = ULTRASKINHand.ResolveTheTextureProperty(mat, property, property);
                        if (resolvedTexture && resolvedTexture != null && mat.HasProperty(property) && mat.GetTexture(property) != resolvedTexture)
                        {
                            Debug.Log("swapping " + property + " of " + mat.name.ToString());
                            mat.SetTexture(property, resolvedTexture);
                        }

                        if (TOW != null && mat.HasProperty("_EmissiveColor"))
                        {
                            
                            if (mat.name.ToString() == "Swapped_ImpactHammerDial (Instance)")
                            {
                                break;
                            }
                            else { 
                            Debug.Log("swapping " + property + " of " + mat.name.ToString());
                            Color VariantColor = GetVarationColor(TOW);
                            Color VariantColor2 = new Color(255, 255, 255, 255);
                            mat.SetColor("_EmissiveColor", VariantColor);
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



        private void Update(SkinEventHandler skinEventHandler)
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

        public static bool CheckTextureInCache(string name)
        {
            if (autoSwapCache.ContainsKey(name))
                return true;
            return false;
        }

		public string ReloadTextures(bool firsttime = false, string path = "")
		{
			if(firsttime && serializedSet != "")
			{
				path = serializedSet;
            }
			else if (firsttime && serializedSet == "")
            {
                path = path;
            }
            if(path == "")
            {
                path = path;
            }
            InitOWGameObjects(firsttime);
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
            Debug.Log(fpath.ToString());
            autoSwapCache.Clear();
			bool failed = false;
            DirectoryInfo dir = new DirectoryInfo(fpath);
            if (!dir.Exists)
                return "failed";
            FileInfo[] Files = dir.GetFiles("*.png");
			if (Files.Length > 0)
			{
				foreach (FileInfo file in Files)
				{
					if (file.Exists)
					{
						byte[] data = File.ReadAllBytes(file.FullName);
						string name = Path.GetFileNameWithoutExtension(file.FullName);
						Texture2D texture2D = new Texture2D(2, 2);
						texture2D.name = name;
						texture2D.filterMode = FilterMode.Point;
						texture2D.LoadImage(data);
						texture2D.Apply();
						if (file.Name == "Railgun_Main_AlphaGlow.png")
						{
							Texture2D texture2D2 = new Texture2D(2, 2);
							byte[] data2 = File.ReadAllBytes(Path.Combine(file.DirectoryName, "Railgun_Main_Emissive.png"));
							texture2D2.filterMode = FilterMode.Point;
							texture2D2.LoadImage(data2);
							texture2D2.Apply();
							Color[] pixels = texture2D.GetPixels();
							Color[] pixels2 = texture2D2.GetPixels();
							for (int k = 0; k < pixels.Length; k++)
							{
								pixels[k].a = pixels2[k].r;
							}
							texture2D.SetPixels(pixels);
							texture2D.Apply();
						}
						Texture texture = new Texture();
						texture = texture2D;
						autoSwapCache.Add(name, texture);
					}
					else
					{
						failed = true;
					}
				}
				if (!failed)
				{
					return "Successfully loaded all Textures from " + Path.GetFileName(fpath) + "!";
				}
			}
			return "Failed to load all textures from " + Path.GetFileName(fpath) + ".\nPlease ensure all of the Texture Files names are Correct, refer to the README file for the correct names and more info.";
		}
	}
}
