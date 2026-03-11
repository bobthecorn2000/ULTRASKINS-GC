using BatonPassLogger;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraSkins.Fractal
{
    public class GrenadeFractal : BaseFractal
    {
        
        public void Init(Grenade grenade)
        {
            swapType = grenade.rocket ? SwapType.Rocket : SwapType.Grenade;

        }





        protected override void setupRenderer()
        {
            
            if (swapType == SwapType.Grenade)
            {
                base.setupRenderer();
            }
            else if (swapType == SwapType.Rocket)
            {

                ChangeMaterials CM = GetComponentInChildren<ChangeMaterials>();
                

                if (CM != null)
                {
                    Material[] chargemats = CM.materials;
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

                    try
                    {
                        if (!renderer)
                        {
                            renderer = CM.GetComponent<MeshRenderer>();
                            string swapname;
                            cachedMaterials = renderer.materials;
                            foreach (Material mat in cachedMaterials)
                            {
                                /*                    if (mat.name == "Pistol New (Instance)")
                                                    {
                                                        renderer.SetMaterial(PrismManager.PrismMan.toon);
                                                    }*/
                                iChange = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;


                                if (!mat.name.StartsWith("Swapped_"))
                                {
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
                        BatonPass.Error("Renderer could not be set up, Code-\"GRENADEFRACTAL-ROCKETMODE-RENDERERSETUP-EX\" ");
                    }







                }
            }
        }


        protected override void DoSwapLogic(Material mat, string texturename)
        {
            if (swapType == SwapType.Grenade)
            {
                SimpleSwap(mat, texturename, "THROWITBACK");
            }
            else if (swapType == SwapType.Rocket)
            {
                SimpleSwap(mat, texturename, "ROCKIT");
            }
            else
            {
                BatonPass.Warn("Swaptype is not set properly");
            }
            
        }

    }
}
