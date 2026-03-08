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
            base.setupRenderer();
            if (swapType == SwapType.Rocket)
            {
                

                Material[] chargemats = GetComponentInChildren<ChangeMaterials>().materials;
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
