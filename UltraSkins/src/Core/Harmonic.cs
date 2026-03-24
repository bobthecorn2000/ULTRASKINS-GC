using BatonPassLogger;
using HarmonyLib;
using System;
using UltraSkins.API;
using UltraSkins.Fractal;
using UltraSkins.Prism;
using UltraSkins.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using static UltraSkins.ULTRASKINHand;

namespace UltraSkins.Harmonic
{
    public class HarmonicService
    {
        public static HarmonicService Instance { get; private set; }
        public Harmony Harmony { get; private set; }  
        public static ServiceStartPackage StartService(HarmonicService SelfObject)
        {
            if (Instance != null)
            {
                BatonPass.Warn("Harmonic has already started and cannot be started again!");
                return new ServiceStartPackage(false, "Harmonic has already started and cannot be started again");
            }


            SelfObject.Harmony = new Harmony("Gcorn.UltraSkins");
            SelfObject.Harmony.PatchAll(typeof(HarmonyGunPatcher));
            SelfObject.Harmony.PatchAll(typeof(HarmonyProjectilePatcher));
            SelfObject.Harmony.PatchAll(typeof(HarmonyUIPatcher));

            BatonPass.Info("Harmonic Service has started");
            Instance = SelfObject;
            return new ServiceStartPackage(true, "HarmonicService was started Correctly");
        }


    }



    [HarmonyPatch]
    public class HarmonyUIPatcher : MonoBehaviour
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
    public class HarmonyGunPatcher : MonoBehaviour
    {

        [HarmonyPatch(typeof(ColorBlindSettings), "UpdateWeaponColors")]
        [HarmonyPostfix]
        public static void UWCPost()
        {
            USAPI.BroadcastDynEmSwap();
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
            catch (Exception Ex)
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
    public class HarmonyProjectilePatcher : MonoBehaviour
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
            if (!__instance.GetComponent<BaseFractal>())
            {

                BaseFractal fract = __instance.gameObject.AddComponent<BaseFractal>();
                fract.Init(__instance);
                fract.PrepareSwap();
            }
        }

    }
}
