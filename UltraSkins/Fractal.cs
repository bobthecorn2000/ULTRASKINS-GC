using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UltraSkins.API;
using UltraSkins.UI;
using UnityEngine;
using static UltraSkins.ULTRASKINHand;

namespace UltraSkins
{
    public class Fractal : MonoBehaviour
    {
        public Material[] cachedMaterials;
        public Renderer renderer;
        public bool forceswap;
        SwapType swapType = SwapType.Unknown;
        SubType subType = SubType.Generic;
        public string iChange;
        private int GCGnum = -1;
        private bool GCGAlt = false;
        GunColorGetter GCGref;
        private bool HasDoneColorSwap = false;

        public enum SwapType {
            Unknown,
            Weapon,
            Arm,
            Nail,
            Grenade,
            Rocket,
            Coin,
            Magnet,
        }

        public enum SubType
        {
            Generic,
            FB,
            KB,
            WL,
            Hammer,
        }

        
        public void Init(GunColorGetter GCG)
        {
            swapType = SwapType.Weapon;
            //not how i would have done it but whatever
            //1:Pistol 2:Shotgun 3:Nailgun 4:Railcannon 5:RocketLauncher
            //1+Alt:SlabRevolver 2+Alt:JackHammer 3+Alt:SawbladeLauncher
            GCGnum = GCG.weaponNumber;
            GCGAlt = GCG.altVersion;
            if (GCGnum == 2 && GCGAlt == true)
            {
                ShotgunHammer hammerInstance = GetComponentInParent<ShotgunHammer>();



                if (hammerInstance != null)
                {
                    subType = SubType.Hammer;
                    ULTRASKINHand.ReadOut.SwapTheDial(this);
                    ReadOut.updateMeter(hammerInstance, true);
                }
            }
            GCGref = GCG;

        }

        public void Init(Punch P)
        {
            swapType = SwapType.Arm;
            switch (P.type)
            {
                case FistType.Standard:
                    subType = SubType.FB;
                    break;
                case FistType.Heavy:
                    subType = SubType.KB;
                    break;
            }
            
        }
        public void Init(HookArm HA)
        {
            swapType = SwapType.Arm;
            subType = SubType.WL;
        }

        public void Init(Magnet M)
        {
            swapType = SwapType.Magnet;
        }

        public void Init(Nail nail)
        {
            swapType = SwapType.Nail;
        }

        public void Init(Grenade grenade)
        {
            swapType = grenade.rocket ? SwapType.Rocket : SwapType.Grenade;
            if (swapType == SwapType.Rocket)
            {
                // perhaps at some point when this is called we could cache it after the first one?

                Material[] chargemats = GetComponent<ChangeMaterials>().materials;
                if (chargemats != null)
                {
                    Material newrocketmat = new Material(chargemats[0]);
                    chargemats[0] = newrocketmat;
                    if (ULTRASKINHand.HoldEm.Check("skull2rocketcharge"))
                    {
                        chargemats[0].mainTexture = ULTRASKINHand.HoldEm.Call("skull2rocketcharge");
                    }
                    if (ULTRASKINHand.HoldEm.Check("skull2rocketbonuscharge"))
                    {
                        chargemats[1].mainTexture = ULTRASKINHand.HoldEm.Call("skull2rocketbonuscharge");
                    }
                }
            }
        }

        public void Init(Coin coin)
        {
            swapType = SwapType.Coin;
            if (HoldEm.Check("coin01_3"))
            {
                coin.uselessMaterial.mainTexture = ULTRASKINHand.HoldEm.Call("coin01_3");
            }
        }



        void Awake()
        {
            USAPI.RefreshFractals += PrepareSwap;
        }




        public void PrepareSwap(object sender, USAPI.FractalTextureUpdateArgs args)
        {
            if (args.doAll)
            {
                
                setupRenderer();
                UpdateMaterials();
            }




        }
        public void PrepareSwap(bool fs = false) {
            forceswap = fs;
            setupRenderer();
            UpdateMaterials();
        }
        

        void setupRenderer()
        {
            if (!renderer)
            {
                renderer = GetComponent<Renderer>();
                string swapname;

                foreach (Material mat in renderer.materials)
                {
                    /*                    if (mat.name == "Pistol New (Instance)")
                                        {
                                            renderer.SetMaterial(PrismManager.PrismMan.toon);
                                        }*/
                    iChange = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                    swapname = "Swapped_" + swapType + "_" + mat.name;
                    if (!ULTRASKINHand.HandInstance.MaterialNames.ContainsKey(swapname))
                    {
                        string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        ULTRASKINHand.HandInstance.MaterialNames.Add(swapname, textureName);
                    }
                }
                if (swapType == SwapType.Weapon)
                {
                    foreach (Material mat in GCGref.defaultMaterials)
                    {
                        swapname = "Swapped_" + swapType + "_" + mat.name;
                        if (!ULTRASKINHand.HandInstance.MaterialNames.ContainsKey(swapname))
                        {
                            string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                            ULTRASKINHand.HandInstance.MaterialNames.Add(swapname, textureName);
                        }
                    }
                    foreach (Material mat in GCGref.coloredMaterials)
                    {
                        swapname = "Swapped_" + swapType + "_" + mat.name;
                        if (!ULTRASKINHand.HandInstance.MaterialNames.ContainsKey(swapname))
                        {
                            string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                            ULTRASKINHand.HandInstance.MaterialNames.Add(swapname, textureName);
                        }
                    }

                }
            }
            
        }


        public void UpdateMaterials()
        {
            BatonPass.Debug("attempting to update fractal mat");
            if (renderer && swapType != SwapType.Weapon)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    PerformTheSwap(materials[i], forceswap);
                }
                ;
            }
            else
            {
                //BatonPass.Warn($"Fractal cannot find renderer, Code-\"FRACTAL-{swapType.ToString()}-MISSING_RENDERER\"");
            }
            if (swapType == SwapType.Weapon)
            {

                for (int i = 0; i < GCGref.defaultMaterials.Length; i++)
                {
                    PerformTheSwap(GCGref.defaultMaterials[i], forceswap);
                    
                }
                for (int i = 0; i < GCGref.coloredMaterials.Length; i++)
                {
                    PerformTheSwap(GCGref.coloredMaterials[i], forceswap);

                }
                
            }
            
        }

       void OnDestroy()
        {
            USAPI.RefreshFractals -= PrepareSwap;
        }




        //TODO Replace this with a system that grabs this from the fract and not at runtime
        public Color GetVarationColor()
        {
            Color VariantColor = new Color(0, 0, 0, 0);
            
            if (swapType == SwapType.Weapon)
            {

                WeaponIcon WPI = transform.GetComponentInParent<WeaponIcon>();
                if (WPI != null)
                {

                    VariantColor = new Color(ColorBlindSettings.Instance.variationColors[(int)WPI.weaponDescriptor.variationColor].r,
                        ColorBlindSettings.Instance.variationColors[(int)WPI.weaponDescriptor.variationColor].g,
                        ColorBlindSettings.Instance.variationColors[(int)WPI.weaponDescriptor.variationColor].b, 1f);
                }
                else
                {
                    BatonPass.Warn("Couldnt find WeaponDescriptior Code-\"FRACTAL-GETVARCOLOR-WPI_NULLREF\" ");
                }

            }
            else if (subType == SubType.FB)
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[0].r,
                ColorBlindSettings.Instance.variationColors[0].g,
                ColorBlindSettings.Instance.variationColors[0].b, 1f);

            }
            else if (subType == SubType.KB)
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[2].r,
                ColorBlindSettings.Instance.variationColors[2].g,
                ColorBlindSettings.Instance.variationColors[2].b, 1f);
            }
            else if (subType == SubType.WL)
            {
                VariantColor = new Color(ColorBlindSettings.Instance.variationColors[1].r,
                    ColorBlindSettings.Instance.variationColors[1].g,
                    ColorBlindSettings.Instance.variationColors[1].b, 1f);
            }
            return VariantColor;
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
                    case "_ReflectionMask":
                        textureToResolve = mat.mainTexture.name + "_Ref";
                        break;
                    case "ROCKIT":
                        textureToResolve = (mat.name.Contains("Swapped_Rocket_AltarUnlitRed") && !texturename.StartsWith("T_")) ? "skull2rocketbonus" : texturename.Contains("T_Sakuya") ? "" : "skull2rocket";
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
            return null;
        }
        public void PerformTheSwap(Material mat, bool forceswap = false)
        {
            if (mat && (!mat.name.StartsWith("Swapped_") || forceswap))
            {
                HasDoneColorSwap = false;
                if (!mat.name.StartsWith("Swapped_"))
                {
                    mat.name = "Swapped_" + swapType + "_" + mat.name;
                }

                forceswap = false;
                Texture resolvedTexture;
                string texturename = GetTextureName(mat.name);
            
                BatonPass.Debug("requested " + mat.name + " got " + texturename);

                if (swapType == SwapType.Weapon || swapType == SwapType.Arm)
                {

                    string[] textureProperties = mat.GetTexturePropertyNames();

                    foreach (string property in textureProperties)
                    {



                       // BatonPass.Debug("Resolving " + property);
                        resolvedTexture = ResolveTheTextureProperty(mat, property, texturename, property);
                        //BatonPass.Info("Attempting to swap " + property + " of " + mat.name.ToString() + " with " + resolvedTexture.name.ToString());
                        if (resolvedTexture != null && mat.HasProperty(property) && mat.GetTexture(property) != resolvedTexture)
                        {
                            //BatonPass.Debug("swapping " + property + " of " + mat.name.ToString());

                            mat.SetTexture(property, resolvedTexture);
                            //BatonPass.Debug("set");
                        }


                    }

                    


                }
                else if (swapType == SwapType.Nail || swapType == SwapType.Coin)
                {
                    resolvedTexture = ResolveTheTextureProperty(mat, "_MainTex", texturename);
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {

                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }
                else if (swapType == SwapType.Grenade)
                {
                    resolvedTexture = ResolveTheTextureProperty(mat, "THROWITBACK", texturename);
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {

                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }
                else if (swapType == SwapType.Rocket)
                {
                    resolvedTexture = ResolveTheTextureProperty(mat, "ROCKIT", texturename);
                    if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
                    {

                        mat.SetTexture("_MainTex", resolvedTexture);

                    }
                }

            }
            if (mat.HasProperty("_EmissiveColor") && HasDoneColorSwap == false)
            {

                if (subType == SubType.Hammer)
                {
                    BatonPass.Debug("Skipping emissive color for this specific material.");
                }
                else
                {


                    try
                    {
                        Color VariantColor = GetVarationColor();
                        Color VariantColor2 = new Color(255, 255, 255, 255);
                        BatonPass.Debug("Got Color:" + VariantColor.r + VariantColor.g + VariantColor.b + VariantColor.a);
                        mat.SetColor("_EmissiveColor", VariantColor);
                    }
                    catch (Exception EX)
                    {
                        BatonPass.Error("Unable to get the variantion color. CODE - \"FRACTAL-PTSWAP-GETVARCOLOR-EX\"");
                        BatonPass.Error(EX.ToString());
                    }

                }
                HasDoneColorSwap = true;
            }
        }
        static string GetTextureName(string materialName)
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








    } 






    public class FractalStorage : MonoBehaviour
    {
        [SerializeField] public List<TextureOverWatch> TOWS;
    }
}

