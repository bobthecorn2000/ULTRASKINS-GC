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
        protected bool NewFract = true;
        //SubTarget renderers will need to be inited the first time its used
        protected Dictionary<GameObject, Renderer> SubTargetRenderers;
        protected bool usesSubRenderers;

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
            SawBlade,
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
            Silver,
        }

        


        public void Init()
        {
            BatonPass.Info("A Fractal has been inited with no paramaters. unexpected functionality may occur");
        }


        public void Init(Magnet M)
        {
            swapType = SwapType.Magnet;
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
                    cachedMaterials = renderer.materials;
                    foreach (Material mat in cachedMaterials)
                    {
                            
                        if (!Resolver.CheckExist(mat.name))
                        {
                            Resolver.CacheMaterialState(mat);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                BatonPass.Error("Renderer could not be set up, Code-\"BASEFRACTAL-RENDERERSETUP-EX\" ");
            }

            
        }

        protected virtual void setupSubTargetRenderer(GameObject rendObject)
        {
            SubTargetRenderers ??= new Dictionary<GameObject, Renderer>();
            try
            {

                //WEEWOO WEEWOO OLD CODE THATS HERE FOR REFERENCE
                Renderer renderthing = rendObject.GetComponent<Renderer>();
                if (renderthing != null)
                {
                    if (!SubTargetRenderers.ContainsKey(rendObject))
                    {
                        SubTargetRenderers.Add(rendObject,renderthing);
                        //cachedMaterials = renderer.materials;
                        foreach (Material mat in renderthing.materials)
                        {

                            if (!Resolver.CheckExist(mat.name))
                            {
                                Resolver.CacheMaterialState(mat);
                            }
                        }
                    }
                }
                else
                {
                    BatonPass.Warn($"a Fractal of type {swapType} {subType} has attempted to setup a subtarget renderer with a gameobject that doesnt contain a renderer");
                }

            }
            catch (Exception ex)
            {
                BatonPass.Error("Renderer could not be set up, Code-\"BASEFRACTAL-SUBRENDERERSETUP-EX\" ");
            }


        }

        //protected int WorkingIndex = 0;
        public virtual void UpdateMaterials()
        {
            BatonPass.Debug("attempting to update fractal mat");
            if (renderer)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    //WorkingIndex = i;
                    PerformTheSwap(materials[i], forceswap);
                }
                ;
            }
            if (usesSubRenderers)
            {
                foreach (KeyValuePair<GameObject,Renderer> kvp in SubTargetRenderers)
                {
                    Material[] materials = kvp.Value.materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        //WorkingIndex = i;
                        PerformTheSwap(materials[i], forceswap);
                    }
                ;
                }
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

    
        public static Texture ResolveTheTextureProperty(Material mat, string property, string texturename)
        {

            if (mat != null && texturename == null)
            {
                BatonPass.Warn("the material " + mat.name + " has no valid texture name");
                return null;
            }
                

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
                        textureToResolve = (mat.name.Contains("AltarUnlitRed") && !texturename.StartsWith("T_")) ? "skull2rocketbonus" : texturename.Contains("T_Sakuya") ? "" : "skull2rocket";
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
            bool matdirty = Resolver.CheckDirty(mat.name);
            if (mat && (matdirty || forceswap || NewFract))
            {
                HasDoneColorSwap = false;
                string texturename = USC.NullTextureName;
                forceswap = false;
                
                if (matdirty)
                {
                    Resolver.SetDirty(mat.name, false);
                    
                }
                texturename = Resolver.RecallSingle(mat.name, "_MainTex");

                BatonPass.Debug("requested " + mat.name + " got " + texturename + " State was " + matdirty + " Fractal new?:" + NewFract);
                NewFract = false;
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



                BatonPass.Debug("Resolving " + property);
                resolvedTexture = ResolveTheTextureProperty(mat, property, texturename);
                //BatonPass.Info("Attempting to swap " + property + " of " + mat.name + " with " + resolvedTexture.name);
                if (resolvedTexture != null && mat.HasProperty(property) && mat.GetTexture(property) != resolvedTexture)
                {
                    BatonPass.Debug("swapping " + property + " of " + mat.name);

                    mat.SetTexture(property, resolvedTexture);
                    BatonPass.Debug("set");
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
                        BatonPass.Error("Unable to get the variation color. CODE - \"FRACTAL-PTSWAP-GETVARCOLOR-EX\"");
                        BatonPass.Error(EX.ToString());
                    }

                
                HasDoneColorSwap = true;
            }
        }

        [Obsolete]
        static string GetTextureName(string materialName)
        {
            if (HoldEm.Instance.MaterialNames.TryGetValue(materialName, out string textureName))
            {
                // If the material name exists, return the texture name
                return textureName;
            }
            else
            {
                // If the material name does not exist, return a default value (e.g., "Texture Not Found")
                BatonPass.Warn(materialName + " not found in cache");
                return null;
            }
        }









    } 






    public class FractalStorage : MonoBehaviour
    {
        [SerializeField] public List<Legacy.TextureOverWatch> TOWS;
    }
}

