using BatonPassLogger;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraSkins.Fractal
{
    public class ChainsawFractal : BaseFractal
    {
        bool UseIDTex = true;

        MeshRenderer ChainBladeRend;
        MeshRenderer ChainSawRend;
        MeshRenderer ChainsawModel;
        private static readonly AccessTools.FieldRef<ShotgunHammer, ScrollingTexture> hammerBlade = AccessTools.FieldRefAccess<ShotgunHammer, ScrollingTexture>("chainsawBladeScroll");
        private static readonly AccessTools.FieldRef<ShotgunHammer, MeshRenderer> hammerSaw = AccessTools.FieldRefAccess<ShotgunHammer, MeshRenderer>("chainsawRenderer");
        private static readonly AccessTools.FieldRef<ShotgunHammer, Chainsaw> HammerChainsawModel = AccessTools.FieldRefAccess<ShotgunHammer, Chainsaw>("chainsaw");
        private static readonly AccessTools.FieldRef<Shotgun, ScrollingTexture> shotgunBlade = AccessTools.FieldRefAccess<Shotgun, ScrollingTexture>("chainsawBladeScroll");
        private static readonly AccessTools.FieldRef<Shotgun, MeshRenderer> shotgunSaw = AccessTools.FieldRefAccess<Shotgun, MeshRenderer>("chainsawRenderer");
        private static readonly AccessTools.FieldRef<Shotgun, Chainsaw> ShotgunChainsawModel = AccessTools.FieldRefAccess<Shotgun, Chainsaw>("chainsaw");
        public void Init(ShotgunHammer chainsaw)
        {
            subType = SubType.Hammer;
            swapType = SwapType.Chainsaw;
            ChainBladeRend = hammerBlade(chainsaw).GetComponent<MeshRenderer>();
            ChainSawRend = hammerSaw(chainsaw);
        }
        public void Init(Shotgun chainsaw)
        {
            swapType = SwapType.Chainsaw;
            ChainBladeRend = shotgunBlade(chainsaw).GetComponent<MeshRenderer>();
            ChainSawRend = shotgunSaw(chainsaw);
        }


        protected void GetChainsawModelRend()
        {
          
        }

        protected override void setupRenderer()
        {
            try
            {
                if (ChainSawRend)
                {

                    string swapname;
                    foreach (Material mat in ChainSawRend.materials)
                    {
                        iChange = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        swapname = "Swapped_" + swapType + "_" + mat.name;
                        if (!HoldEm.Instance.MaterialNames.ContainsKey(swapname))
                        {
                            string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                            HoldEm.Instance.MaterialNames.Add(swapname, textureName);
                        }
                    }
                }
                else
                {
                    BatonPass.Debug("chainsawrend is null");
                }
                if (ChainBladeRend)
                {

                    string swapname;
                    foreach (Material mat in ChainBladeRend.materials)
                    {
                        iChange = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                        swapname = "Swapped_" + swapType + "_" + mat.name;
                        if (!HoldEm.Instance.MaterialNames.ContainsKey(swapname))
                        {
                            string textureName = (mat.HasProperty("_MainTex") && mat.mainTexture != null) ? mat.mainTexture.name : null;
                            HoldEm.Instance.MaterialNames.Add(swapname, textureName);
                        }
                    }
                }
                else
                {
                    BatonPass.Debug("chainbladerend is null");
                }
            }
            catch (Exception ex)
            {
                BatonPass.Error("Renderer could not be set up, Code-\"CHAINSAWFRACTAL-RENDERERSETUP-EX\" ");
                BatonPass.Error(ex.ToString());
            }


        }

        public override void UpdateMaterials()
        {
            BatonPass.Debug("attempting to update ChainSawFractal mat");
            if (ChainBladeRend)
            {
                Material[] materials = ChainBladeRend.materials;
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
            if (ChainSawRend)
            {
                Material[] materials = ChainSawRend.materials;
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


    }
}
