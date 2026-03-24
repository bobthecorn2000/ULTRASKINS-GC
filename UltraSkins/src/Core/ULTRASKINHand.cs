using BatonPassLogger;
using BatonPassLogger.GUI;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UltraSkins.API;
using UltraSkins.Harmonic;
using UltraSkins.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UltraSkins.API.USAPI;
using static UltraSkins.HoldEm;


//using UltraSkins.Prism;







//using UnityEngine.UIElements;






namespace UltraSkins
{
    [BepInPlugin(USC.PLUGIN_GUID, USC.PLUGIN_NAME, USC.VERSION)]


    public partial class ULTRASKINHand : BaseUnityPlugin
    {





        private string modFolderPath;
        public bool loadTextureLock = false;
        public BPGUIManager BPGUI;

        string skinfolderdir;




        private List<Sprite> _default;
        private List<Sprite> _edited;
        private bool firstmenu = true;


        public string folderupdater;



        public string[] directories;
        public string serializedSet = "";
        public bool swapped = false;
        public UltraSkins.UI.SettingsManager settingsmanager;
        public Legacy.TowStorage PtowStorage;

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
            BatonPass.Info("HAND service started");
            if (HandInstance == null)
            {
                HandInstance = this;
            }
            else
            {
                Destroy(this);
            }


            BatonPass.Info("Starting Bootstrapper");
            BatonPass.Debug("Plugin path set to " + USC.MODPATH);
            BatonPass.Debug("NAVI: APPDATAPATH AT: " + USC.GCDIR);
            BatonPass.Info("Loading UI Code");
            Assembly.Load(File.ReadAllBytes(Path.Combine(USC.MODPATH, "usUI.dll")));
            BatonPass.Info("Loading Content Catalog");
            var catalog = Addressables.LoadContentCatalogAsync(Path.Combine(USC.MODPATH, "catalog.json"), true).WaitForCompletion();



            BatonPass.Info("Starting Harmonic Service");

            HarmonicService HS = new HarmonicService();
            HarmonicService.StartService(HS);

            BatonPass.Info("Starting Holdem Service");
            HoldEm HE = new HoldEm();
            HoldEm.StartService(HE);

            BatonPass.Info("Starting Textile Service");
            TextileService Tex = new TextileService();
            TextileService.StartService(Tex);

            BatonPass.Info("Starting Profile Service");
            ProfileService ps = new ProfileService();
            ProfileService.StartService(ps);


            BatonPass.Info("Starting BatonPassGUI");
            if (BPGUIManager.BPGUIinstance == null)
            {
                GameObject managerObject = new GameObject("BPGUIManager");
                managerObject.AddComponent<BPGUIManager>();
                BPGUI = BPGUIManager.BPGUIinstance;
            }
            BatonPass.Info("Starting Compatibility Layer");
            UsingCompatLayer = compatlayer.PluginConfigCompatBoot();


            BatonPass.Info("Creating Settings Manager");
            MenuCreator.CreateSMan();
            BatonPass.Info("Suppressing Logs to user chosen level");
            BootstrapLoggerConfig.LoadUserLogPref(SettingsManager.Instance.GetSettingValue<string>("LogLevel"));

            if (Prism.PrismManager.Instance == null)
            {
                GameObject PrismManagerObject = new GameObject("Prism");
                PrismManagerObject.AddComponent<Prism.PrismManager>();
            }

            SceneManager.activeSceneChanged += SceneManagerOnsceneLoaded;
            BatonPass.Info("Scenemanager Created");
            USAPI.OnTexLoadFinished += RANKTITLESWAPPER.MakeTheStyleRank;
            BatonPass.Info("Subscribing to the API");
            // The GUID of the configurator must not be changed as it is used to locate the config file path
            BatonPass.Debug("INIT BATON PASS: ONMODLOADED()");
            OnModLoaded();

        }


        public void OnModLoaded()
        {
            //Harmony.DEBUG = true;
            // HarmonyFileLog.Enabled = true;




            BatonPass.Success("BATON PASS: Welcome To Ultraskins, We are on the ONMODLOADED() STEP");


            try
            {
                BatonPass.Debug("Creating the SkinEvent Handler");

                if (ProfileService.Instance.MMI != null)
                {
                    ThunderStoreMode = true;
                }

                BatonPass.Debug("INIT BATON PASS: GETMODFOLDERPATH()");

                BatonPass.Debug("BATON PASS: WELCOME BACK TO ONMODLOADED() WE RECIEVED " + modFolderPath);



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
                    TextileService.Instance.RefreshSkins();
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
            else
            {

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
                    if (HoldEm.Bluff(HoldEm.HoldemType.SC, spritelookup))
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

        bool AssetInUse(UnityEngine.Object asset)
        {
            return Resources.FindObjectsOfTypeAll<Component>().Any(c =>
                c is Renderer r && r.sharedMaterials.Any(m =>
                    m && m.HasProperty("_MainTex") && m.mainTexture == asset) ||
                c is Image img && img.sprite && img.sprite.texture == asset ||
                c is RawImage rawImg && rawImg.texture == asset);
        }






    }
}
