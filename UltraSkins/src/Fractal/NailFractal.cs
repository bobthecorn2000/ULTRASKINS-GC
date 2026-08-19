using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltraSkins.Fractal
{
    public class NailFractal : BaseFractal
    {
        Nail nailref;
        public void Init(Nail nail)
        {

            swapType = nail.sawblade ? SwapType.SawBlade : SwapType.Nail;
            subType = nail.fodderDamageBoost ? SubType.Silver : SubType.Generic;
            nailref = nail;
        }

        protected override void setupRenderer()
        {
            if (swapType == SwapType.SawBlade)
            {
                base.setupRenderer();
                GameObject quad = this.transform.GetChild(0).gameObject;
                usesSubRenderers = true;
                if (subType == SubType.Silver)
                {
                    Renderer rend = quad.GetComponent<Renderer>();
                    rend.material.name = rend.material.name + " Silver";
                }
                setupSubTargetRenderer(quad);
            }
        }

        protected override void DoSwapLogic(Material mat, string texturename)
        {
            SimpleSwap(mat, texturename);
        }
    }
}
