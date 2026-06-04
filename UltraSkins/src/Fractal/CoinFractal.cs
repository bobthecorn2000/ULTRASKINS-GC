using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BatonPassLogger;
using static UltraSkins.ULTRASKINHand;

namespace UltraSkins.Fractal
{
    public class CoinFractal : BaseFractal
    {
        protected Coin c;
        protected MeshRenderer CoinRenderer;
        public void Init(Coin coin)
        {
            swapType = SwapType.Coin;
            c = coin;
        }


        protected override void setupRenderer()
        {
            try
            {
                if (!CoinRenderer)
                {
                    CoinRenderer = GetComponentInChildren<MeshRenderer>();
                    string swapname;
                    foreach (Material mat in CoinRenderer.materials)
                    {
                        if (!Resolver.CheckExist(mat.name))
                        {
                            Resolver.CacheMaterialState(mat);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                BatonPass.Error("Renderer could not be set up, Code-\"COINFRACTAL-RENDERERSETUP-EX\" ");
                BatonPass.Error(ex.ToString());
            }

            
        }
        public override void UpdateMaterials()
        {
            if (CoinRenderer)
            {
                Material[] materials = CoinRenderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    PerformTheSwap(materials[i], forceswap);
                }
    ;
            }
        }

        protected override void DoSwapLogic(Material mat, string texturename)
        {
            base.SimpleSwap(mat, texturename);
            if (HoldEm.Check("coin01_3"))
            {
                c.uselessMaterial.mainTexture = HoldEm.Call("coin01_3");
            }
        }

 



    }
}
