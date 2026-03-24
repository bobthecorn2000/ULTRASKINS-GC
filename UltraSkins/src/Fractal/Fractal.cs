using BatonPassLogger;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UltraSkins.API;
using UltraSkins.UI;
using UnityEngine;
using static UltraSkins.ULTRASKINHand;

namespace UltraSkins.Fractal
{
    public class BaseFractal : MonoBehaviour
    {
        public Material[] cachedMaterials;
        public Renderer renderer;
        public bool forceswap;
        protected SwapType swapType = SwapType.Unknown;
        protected SubType subType = SubType.Generic;
        public string iChange;

        protected bool HasDoneColorSwap = false;

        public enum SwapType {
            Unknown,
            Weapon,
            Arm,
            Nail,
            Grenade,
            Rocket,
            Coin,
            Magnet,
            Chainsaw,
        }

        public enum SubType
        {
            Generic,
            FB,
            KB,
            WL,
            Hammer,
            SandBox,
            RightArm,
        }

        


        public void Init()
        {

        }


        public void Init(Magnet M)
        {
            swapType = SwapType.Magnet;
        }

        public void Init(Nail nail)
        {
            swapType = SwapType.Nail;
        }







        protected virtual void Awake()
        {
            USAPI.RefreshFractals += PrepareSwap;
        }




        public virtual void PrepareSwap(object sender, USAPI.FractalTextureUpdateArgs args)
        {
            
            if (args.doAll)
            {
                forceswap = true;
                setupRenderer();
                UpdateMaterials();
            }




        }
        public virtual void PrepareSwap(bool fs = false) {
            forceswap = fs;
            setupRenderer();
            UpdateMaterials();
        }
        

        protected virtual void setupRenderer()
        {
            try
            {
                if (!renderer)
                {
                    renderer = GetComponent<Renderer>();
                    string swapname;
                    cachedMaterials = renderer.materials;
                    foreach (Material mat in cachedMaterials)
                    {
                        /*                    if (mat.name == "Pistol New (Instance)")
                                            {
                                                renderer.SetMaterial(PrismManager.PrismMan.toon);
                                            }*/
                        iChange = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;

                        if (!mat.name.StartsWith("Swapped_")) {
                            swapname = "Swapped_" + swapType + "_" + mat.name;
                        }
                        else
                        {
                            swapname = mat.name;
                        }
                            

                        if (!ULTRASKINHand.HandInstance.MaterialNames.ContainsKey(swapname))
                        {
                            string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                            ULTRASKINHand.HandInstance.MaterialNames.Add(swapname, textureName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BatonPass.Error("Renderer could not be set up, Code-\"BASEFRACTAL-RENDERERSETUP-EX\" ");
            }

            
        }


        public virtual void UpdateMaterials()
        {
            BatonPass.Debug("attempting to update fractal mat");
            if (renderer)
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
            
        }

        protected virtual void OnDestroy()
        {
            USAPI.RefreshFractals -= PrepareSwap;
        }




        
        public virtual Color GetVarationColor()
        {
            Color VariantColor = new Color(0, 0, 0, 0);
            BatonPass.Debug("Variation Color on a Base Fractal is not supported");
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
                
                string texturename = GetTextureName(mat.name);
            
                BatonPass.Debug("requested " + mat.name + " got " + texturename);

                DoSwapLogic(mat, texturename);

            }

        }


        protected virtual void DoSwapLogic(Material mat,string texturename)
        {
            DeepSwap(mat, texturename);
        }




        /// <summary>
        /// Swap all params in a Mat
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="texturename"></param>
        protected void DeepSwap(Material mat,string texturename)
        {
            string[] textureProperties = mat.GetTexturePropertyNames();
            Texture resolvedTexture;
            
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

        /// <summary>
        /// Swap 1 param in a Mat
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="texturename"></param>
        /// <param name="paramName"></param>
        protected void SimpleSwap(Material mat,string texturename,string paramName = "_MainTex")
        {
            Texture resolvedTexture;
            resolvedTexture = ResolveTheTextureProperty(mat, paramName, texturename);
            if (resolvedTexture && resolvedTexture != null && mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != resolvedTexture)
            {

                mat.SetTexture("_MainTex", resolvedTexture);

            }
        }

        protected void DoEmissiveSwap(Material mat)
        {
            if (mat.HasProperty("_EmissiveColor") && HasDoneColorSwap == false)
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

